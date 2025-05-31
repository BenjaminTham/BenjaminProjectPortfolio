from pyspark.sql import SparkSession
from pyspark.sql.functions import col, regexp_extract, udf
from pyspark.sql.types import ArrayType, StringType, FloatType
from pyspark.ml.feature import Word2Vec
from pyspark.ml.linalg import SparseVector

spark = SparkSession.builder.appName("Word2VecToLibSVM").getOrCreate()

# 1. Read data
#    Your file has lines like:
#    MapperKey100000605   0.03 [cute]
#    => Spark sees two columns: _c0, _c1
df_raw = spark.read \
    .option("sep", "\t") \
    .option("header", "false") \
    .csv("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/mr_preprocessing/linear_reg_mr/output/")   # Adjust your path here

# Rename columns for clarity:
# _c0 => mapperKey (we might not need it)
# _c1 => sentiment_and_keywords (we need to split it)
df_stage = df_raw.select(
    df_raw["_c0"].alias("mapperKey"), 
    df_raw["_c1"].alias("sentiment_and_keywords")
)

# 2. Extract sentiment + bracketed keywords using regex.
#    Example line in sentiment_and_keywords: "0.03 [cute]"
#    - The sentiment is captured by ^(\S+)  => "0.03"
#    - The bracketed part is captured by  ^\S+\s+(.*)  => "[cute]"
df_parsed = df_stage \
    .withColumn("sentiment_str", regexp_extract(col("sentiment_and_keywords"), r"^(\S+)", 1)) \
    .withColumn("keywords_str", regexp_extract(col("sentiment_and_keywords"), r"^\S+\s+(.*)", 1))

# Convert sentiment_str to float
df_parsed = df_parsed.withColumn("sentiment", col("sentiment_str").cast(FloatType()))

# 3. Parse bracketed keywords into a string array
@udf(returnType=ArrayType(StringType()))
def parse_bracketed_str(bracketed_str):
    """
    Convert something like "[cute]" or "[football, nationalfootball]" to ["cute"] or ["football", "nationalfootball"].
    """
    if not bracketed_str:
        return []
    # Remove leading/trailing brackets
    s = bracketed_str.strip()
    if s.startswith("[") and s.endswith("]"):
        s = s[1:-1].strip()
    if not s:
        return []
    # Split by comma
    tokens = [t.strip() for t in s.split(",")]
    # Filter out empty tokens
    tokens = [t for t in tokens if t]
    return tokens

df_final = df_parsed.withColumn("keywords", parse_bracketed_str(col("keywords_str")))

# We no longer need these intermediate columns
df_final = df_final.drop("sentiment_str", "keywords_str", "sentiment_and_keywords")

# Now df_final has columns: mapperKey, sentiment (float), keywords (array<string>)
df_final.show(truncate=False)

# 4. Train Word2Vec
word2Vec = Word2Vec(
    vectorSize=50,  # example dimension
    minCount=5,
    inputCol="keywords",
    outputCol="features"
)
w2v_model = word2Vec.fit(df_final)
df_with_vectors = w2v_model.transform(df_final)

# 5. Convert to LibSVM format
def to_libsvm_format(sentiment, features):
    arr = features.toArray() if isinstance(features, SparseVector) else features
    sb = [str(sentiment)]
    for i, val in enumerate(arr):
        sb.append(f"{i+1}:{val}")
    return " ".join(sb)

libsvm_rdd = df_with_vectors.rdd.map(lambda row: to_libsvm_format(row["sentiment"], row["features"]))

# Save to text files (one line per record, in LibSVM format)
libsvm_rdd.saveAsTextFile("s3://sg.edu.sit.inf2006.aaronlam/SentimentPredictionAnalysisFolder/spark_analysis/linear_reg_analysis/linear_feature_output")

spark.stop()
