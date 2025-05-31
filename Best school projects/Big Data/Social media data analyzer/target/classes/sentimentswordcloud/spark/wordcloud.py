from pyspark.sql import SparkSession

# Create a SparkSession
spark = SparkSession.builder.appName("WordCountSort").getOrCreate()

# Read all output files from the MapReduce output directory
lines = spark.read.text("s3://sg.edu.sit.inf2006.aaronlam/SentimentWordCloudFolder/mr_preprocessing/output/").rdd.map(lambda line: line.value)

# Split each line by tab (expecting 3 fields: word, sentiment, count)
parts = lines.map(lambda line: line.split("\t"))

# Filter out any malformed lines (only keep lines with exactly 3 fields)
validParts = parts.filter(lambda arr: len(arr) == 3)

# Convert count to integer and form a tuple: (word, sentiment, count)
wordCounts = validParts.map(lambda arr: (arr[0], arr[1], int(arr[2])))

# Separate the records by sentiment
positiveCounts = wordCounts.filter(lambda record: record[1].strip().lower() == "positive")
negativeCounts = wordCounts.filter(lambda record: record[1].strip().lower() == "negative")

# Sort each group by count in descending order
sortedPos = positiveCounts.sortBy(lambda record: record[2], ascending=False).collect()
sortedNeg = negativeCounts.sortBy(lambda record: record[2], ascending=False).collect()

# Instead of printing, write the results to S3 using RDD's saveAsTextFile
sc = spark.sparkContext  # Get the SparkContext

# Write top 20 positive words
sc.parallelize(sortedPos[:20]) \
  .coalesce(1) \
  .saveAsTextFile("s3://sg.edu.sit.inf2006.aaronlam/SentimentWordCloudFolder/spark_analysis/output/top_20_positive_words")

# Write top 20 negative words
sc.parallelize(sortedNeg[:20]) \
  .coalesce(1) \
  .saveAsTextFile("s3://sg.edu.sit.inf2006.aaronlam/SentimentWordCloudFolder/spark_analysis/output/top_20_negative_words")


# Stop the Spark session
spark.stop()