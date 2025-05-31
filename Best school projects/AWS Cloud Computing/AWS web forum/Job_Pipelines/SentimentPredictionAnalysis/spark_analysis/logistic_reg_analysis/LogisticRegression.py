from pyspark.sql import SparkSession
from pyspark.ml.classification import LogisticRegression
from pyspark.ml.evaluation import MulticlassClassificationEvaluator
from pyspark.conf import SparkConf
from pyspark.sql import SparkSession
from sklearn.metrics import confusion_matrix
from pyspark.ml.feature import PCA
import s3fs


conf = SparkConf()
conf.set("spark.driver.memory", "15g")   
conf.set("spark.executor.memory", "15g")  


spark = SparkSession.builder.appName("MainEmotionClassification").getOrCreate()

training = spark.read.format("libsvm").load("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/logistic_reg_analysis/logistic_feature_output")
training = training.sample(fraction=0.01, seed=42)


lr = LogisticRegression(maxIter=10, regParam=0.01, elasticNetParam=0.8)

# Fit the model
lrModel = lr.fit(training)

# Print the coefficients and intercept for logistic regression
print()
print()


predictions = lrModel.transform(training)

# Evaluate accuracy
accuracy_evaluator = MulticlassClassificationEvaluator(
    labelCol="label", predictionCol="prediction", metricName="accuracy"
)
accuracy = accuracy_evaluator.evaluate(predictions)

# Evaluate F1 score
f1_evaluator = MulticlassClassificationEvaluator(
    labelCol="label", predictionCol="prediction", metricName="f1"
)
f1_score = f1_evaluator.evaluate(predictions)



fs = s3fs.S3FileSystem()
with fs.open("sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/logistic_reg_analysis/logistic_results.txt", "a") as f:
    f.write("Intercept Vector:\n" + str(lrModel.interceptVector) + "\n")
    f.write("Accuracy = " + str(accuracy) + "\n")
    f.write("F1-score = " + str(f1_score) + "\n")


