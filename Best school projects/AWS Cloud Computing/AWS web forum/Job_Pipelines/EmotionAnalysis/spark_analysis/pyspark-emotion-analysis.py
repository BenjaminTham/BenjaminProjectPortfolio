from pyspark.sql import SparkSession
from pyspark.sql.functions import col, split, explode, trim, avg, stddev, row_number, date_format
from pyspark.sql.window import Window
from pyspark.sql import functions as F



# Initialize Spark session
spark = SparkSession.builder.appName('EmotionAnalysis').getOrCreate()

# Load the MapReduce output into a DataFrame
df = spark.read.text("s3://sg.edu.sit.inf2006.aaronlam/EmotionAnalysisFolder/mr_preprocessing/output/part-r-00000")


# Parse the data into timestamp and emotion columns
df = df.withColumn('timestamp', split(df['value'], '\t').getItem(0)) \
       .withColumn('emotions', split(df['value'], '\t').getItem(1))

# Exploding emotions into individual rows
emotion_df = df.select('timestamp', explode(split(df['emotions'], ',')).alias('emotion_count'))

# Split the emotion and count into separate columns
emotion_df = emotion_df.withColumn('emotion', split(emotion_df['emotion_count'], ':').getItem(0)) \
                       .withColumn('count', split(emotion_df['emotion_count'], ':').getItem(1))

# Trim the 'emotion' column and cast 'count' to integer
emotion_df = emotion_df.withColumn('emotion', trim(col('emotion'))) \
                       .withColumn('count', col('count').cast('int'))


# Calculate mean and standard deviation of the 'count'
mean_stddev = emotion_df.select(avg('count').alias('mean'), stddev('count').alias('stddev')).first()

mean = mean_stddev['mean']
stddev_value = mean_stddev['stddev']

# Add the Z-score column (this one is to show how far the outliers are from each other)
emotion_df = emotion_df.withColumn('z_score', (col('count') - mean) / stddev_value)

# Filter to identify outliers based on Z-score (threshold = 3) [3 is quite standard but can adjust accordingly]
outliers_df = emotion_df.filter((col('z_score') > 3) | (col('z_score') < -3))

# --- Code to get top 3 emotions per timestamp ---

# Create a window partitioned by 'timestamp' and ordered by 'count' in descending order
windowSpec = Window.partitionBy('timestamp').orderBy(col('count').desc())

# Add a rank column based on the window specification
ranked_emotions_df = emotion_df.withColumn('rank', row_number().over(windowSpec))

# Filter for the top 3 emotions for each timestamp
top_3_emotions_df = ranked_emotions_df.filter(col('rank') <= 3).orderBy('timestamp', col('count').desc())


########## Addding Day Columns to Dataframe ##################
# Convering for normal emotions dataframe
emotions_df_with_day = emotion_df \
    .withColumn("day_of_week", date_format(col("timestamp"), "EEEE"))

# Converting for Outliers Dataframe
outliers_df_with_day = outliers_df \
    .withColumn("day_of_week", date_format(col("timestamp"), "EEEE"))


######### Processing for Time Series Analysis ###########
# Aggregate counts by day_of_week and emotion
aggregated_df = emotions_df_with_day.groupBy("day_of_week", "emotion") \
    .agg(F.sum("count").alias("total_count"))

# Pivot the DataFrame
pivot_df = aggregated_df.groupBy("day_of_week") \
    .pivot("emotion") \
    .agg(F.sum("total_count"))

# Define the order of days
day_order = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]

# Reorder the DataFrame by day_of_week
time_series_df = pivot_df.withColumn("day_of_week", F.expr("array_position(array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'), day_of_week)")) \
    .orderBy("day_of_week")



# ############ Export Dataframes into CSV #################
# # Output path in S3
output_path = "s3://sg.edu.sit.inf2006.aaronlam/EmotionAnalysisFolder/spark_analysis/output/"

# Save DataFrame as CSV
# Write DataFrames into separate subdirectories
time_series_df.coalesce(1).write.mode("overwrite").option("header", "true").csv(output_path + "time_series/")
outliers_df_with_day.coalesce(1).write.mode("overwrite").option("header", "true").csv(output_path + "outliers/")
top_3_emotions_df.coalesce(1).write.mode("overwrite").option("header", "true").csv(output_path + "top_emotions/")


# Stop Spark session
spark.stop()