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

training = spark.read.format("libsvm").load("s3://sg.edu.sit.bigdataprojectlinearregression.ben/output_libsvm")
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
with fs.open("sg.edu.sit.bigdataprojectlinearregression.ben/results.txt", "a") as f:
    f.write("Intercept Vector:\n" + str(lrModel.interceptVector) + "\n")
    f.write("Accuracy = " + str(accuracy) + "\n")
    f.write("F1-score = " + str(f1_score) + "\n")


# Reduce features to 2 principal components
pca = PCA(k=2, inputCol="features", outputCol="pcaFeatures")
pcaModel = pca.fit(training)
result = pcaModel.transform(training).select("label", "pcaFeatures")

# Convert to Pandas for plotting
result_pd = result.toPandas()
result_pd["pc1"] = result_pd["pcaFeatures"].apply(lambda x: x[0])
result_pd["pc2"] = result_pd["pcaFeatures"].apply(lambda x: x[1])

# Scatter plot
plt.figure(figsize=(10, 8))
sns.scatterplot(x="pc1", y="pc2", hue="label", data=result_pd, palette="deep")
plt.title("PCA Scatter Plot of Feature Space")
plt.xlabel("Principal Component 1")
plt.ylabel("Principal Component 2")

with fs.open("s3://sg.edu.sit.bigdataprojectlinearregression.ben/logisticregressiongraph.csv", "w") as f:
    result_pd.to_csv(f, index=False)

