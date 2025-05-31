from pyspark.sql import SparkSession
from pyspark.ml.regression import LinearRegression
import s3fs



# Read the entire output_libsvm folder (which contains multiple part files)
spark = SparkSession.builder.appName("SentimentPrediction").getOrCreate()

training = spark.read.format("libsvm").load("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/linear_reg_analysis/linear_feature_output")

lr = LinearRegression(maxIter=10, regParam=0.01, elasticNetParam=0.8)

# Fit the model
lrModel = lr.fit(training)

# Print the coefficients and intercept for linear regression
print("Coefficients: %s" % str(lrModel.coefficients))
print("Intercept: %s" % str(lrModel.intercept))

# Summarize the model over the training set and print out some metrics
trainingSummary = lrModel.summary
print("numIterations: %d" % trainingSummary.totalIterations)
print("objectiveHistory: %s" % str(trainingSummary.objectiveHistory))
trainingSummary.residuals.show()
print("RMSE: %f" % trainingSummary.rootMeanSquaredError)
print("r2: %f" % trainingSummary.r2)

    
fs = s3fs.S3FileSystem()
with fs.open("sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/linear_reg_analysis/linear_results.txt", "a") as f:
    f.write("Coefficients: %s" % str(lrModel.coefficients))
    f.write("Intercept: %s" % str(lrModel.intercept))
    f.write("numIterations: %d" % trainingSummary.totalIterations)
    f.write("objectiveHistory: %s" % str(trainingSummary.objectiveHistory))
    f.write("RMSE: %f" % trainingSummary.rootMeanSquaredError)
    f.write("r2: %f" % trainingSummary.r2)