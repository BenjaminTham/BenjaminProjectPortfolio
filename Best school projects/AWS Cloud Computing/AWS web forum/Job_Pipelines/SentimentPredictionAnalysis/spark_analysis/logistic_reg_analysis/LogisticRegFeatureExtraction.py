from pyspark.sql import SparkSession
from pyspark.sql.functions import col, regexp_extract, udf
from pyspark.sql.types import ArrayType, StringType
from pyspark.ml.feature import Word2Vec, StringIndexer
from pyspark.ml.linalg import SparseVector

spark = SparkSession.builder.appName("Word2VecToLibSVM").getOrCreate()

# 1. Read data (2 columns: mapperKey, mainemotion_and_keywords)
df_raw = spark.read \
    .option("sep", "\t") \
    .option("header", "false") \
    .csv("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/mr_preprocessing/logistic_reg_mr/output/") 

df_stage = df_raw.select(
    df_raw["_c0"].alias("mapperKey"), 
    df_raw["_c1"].alias("mainemotion_and_keywords")
)

# 2. Extract the "main_emotion_str" and "keywords_str"
df_parsed = df_stage \
    .withColumn("main_emotion_str", regexp_extract(col("mainemotion_and_keywords"), r"^(\S+)", 1)) \
    .withColumn("keywords_str", regexp_extract(col("mainemotion_and_keywords"), r"^\S+\s+(.*)", 1))

# 3. Convert main_emotion_str to a numeric label using StringIndexer
#    This automatically maps e.g. "admiration" -> 0.0, "neutral" -> 1.0, etc.
indexer = StringIndexer(inputCol="main_emotion_str", outputCol="main_emotion_label")
df_indexed = indexer.fit(df_parsed).transform(df_parsed)
# Now df_indexed has a column "main_emotion_label" which is numeric.

# 4. Parse the bracketed keywords
@udf(returnType=ArrayType(StringType()))
def parse_bracketed_str(bracketed_str):
    if not bracketed_str:
        return []
    s = bracketed_str.strip()
    if s.startswith("[") and s.endswith("]"):
        s = s[1:-1].strip()
    if not s:
        return []
    tokens = [t.strip() for t in s.split(",")]
    tokens = [t for t in tokens if t]
    return tokens

df_final = df_indexed.withColumn("keywords", parse_bracketed_str(col("keywords_str")))

# Drop intermediate columns
df_final = df_final.drop("main_emotion_str", "keywords_str", "mainemotion_and_keywords")

# 5. Train Word2Vec on the "keywords" to get a features vector
word2Vec = Word2Vec(vectorSize=50, minCount=5, inputCol="keywords", outputCol="features")
w2v_model = word2Vec.fit(df_final)
df_with_vectors = w2v_model.transform(df_final)

# 6. Convert to LibSVM format
def to_libsvm_format(label, features):
    arr = features.toArray() if isinstance(features, SparseVector) else features
    sb = [str(label)]  # numeric label from StringIndexer
    for i, val in enumerate(arr):
        sb.append(f"{i+1}:{val}")
    return " ".join(sb)

# 7. Build the RDD of LibSVM lines, using "main_emotion_label" as the numeric label
libsvm_rdd = df_with_vectors.rdd.map(
    lambda row: to_libsvm_format(row["main_emotion_label"], row["features"])
)

# 8. Save to text files in LibSVM format
libsvm_rdd.saveAsTextFile("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/logistic_reg_analysis/logistic_feature_output")

spark.stop()
