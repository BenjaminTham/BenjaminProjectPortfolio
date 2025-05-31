!pip install pyspark

from pyspark.sql import SparkSession
from pyspark.sql.functions import split, col, countDistinct, sum as _sum, desc, row_number
import plotly.express as px
import pandas as pd

"""# Init Spark"""

# Initialize Spark
spark = SparkSession.builder.appName("Theme-Frequency-By-Language").getOrCreate()

# 1. Load and parse the data (replace with your actual file)
df = spark.read.text("part-r-00000")

# 2. Parse into columns (Language, Theme, Count)
parsed_df = df.withColumn("temp", split(col("value"), " - ")) \
              .withColumn("language", col("temp")[0]) \
              .withColumn("theme_count", split(col("temp")[1], "\t")) \
              .withColumn("theme", col("theme_count")[0]) \
              .withColumn("count", col("theme_count")[1].cast("integer")) \
              .drop("value", "temp", "theme_count")

# Register the parsed DataFrame as a temporary view for easier querying
parsed_df.createOrReplaceTempView("theme_language_counts")

"""# Analysis

## 1. Top 3 languages of all themes
"""

top_languages_df = spark.sql("""
    SELECT
        language,
        SUM(count) AS total_count
    FROM theme_language_counts
    GROUP BY language
    ORDER BY total_count DESC
    LIMIT 3
""")

top_languages = [row.language for row in top_languages_df.collect()]

"""## 2. Themes with the largest % of the top languages

"""

theme_language_percentage_df = spark.sql("""
    WITH ThemeTotalCounts AS (
        SELECT
            theme,
            SUM(count) AS theme_total
        FROM theme_language_counts
        GROUP BY theme
    ),
    ThemeLanguageCounts AS (
        SELECT
            theme,
            language,
            SUM(count) AS language_count
        FROM theme_language_counts
        WHERE language IN ('{}', '{}', '{}') -- Insert top 3 languages
        GROUP BY theme, language
    )
    SELECT
        tlc.theme,
        tlc.language,
        tlc.language_count,
        ttc.theme_total,
        (tlc.language_count / ttc.theme_total) * 100 AS percentage
    FROM ThemeLanguageCounts tlc
    JOIN ThemeTotalCounts ttc ON tlc.theme = ttc.theme
""".format(top_languages[0], top_languages[1], top_languages[2]))

theme_top_3_percentage_df = theme_language_percentage_df.groupBy("theme") \
    .agg(_sum("percentage").alias("top_3_percentage")) \
    .orderBy(desc("top_3_percentage"))

top_3_themes_df = theme_top_3_percentage_df.limit(3)
top_3_themes = [row.theme for row in top_3_themes_df.collect()]

"""## Theme with the MOST & LEAST number of Languages"""

theme_language_count_df = parsed_df.groupBy("theme") \
    .agg(countDistinct("language").alias("num_languages"))

top_theme_df = theme_language_count_df.orderBy(desc("num_languages"))
top_theme_languages = top_theme_df.first()
max_language_count = top_theme_languages.num_languages
themes_with_max = top_theme_df.filter(col("num_languages") == max_language_count).collect()

least_theme_df = theme_language_count_df.orderBy("num_languages")
least_theme_languages = least_theme_df.first()
min_language_count = least_theme_languages.num_languages
themes_with_min = least_theme_df.filter(col("num_languages") == min_language_count).collect()

"""## Final Output Analysis"""

print("Top 3 Languages Across All Themes:")
print(top_languages)

print("\nTop 3 themes with the highest percentage of the Top 3 languages:")
for theme in top_3_themes:
    print(f"\nTheme: {theme}")
    theme_language_percentages = theme_language_percentage_df.filter(col("theme") == theme).orderBy(desc("percentage")).collect()
    for row in theme_language_percentages:
        print(f"  Language: {row.language}, Percentage: {row.percentage:.2f}%")

print("\nTheme(s) with the Most Number of Languages:")
for row in themes_with_max:
    print(f"Theme: {row.theme}, with {row.num_languages} Languages!")

print("\nTheme(s) with the Least Number of Languages:")
for row in themes_with_min:
    print(f"Theme: {row.theme}, with {row.num_languages} Languages.")

"""# Visualization (Piecharts)"""

# Group by Theme and aggregate counts
theme_groups = parsed_df.groupBy("theme").agg(_sum("count").alias("total_count"))

# Get list of themes
themes = [row.theme for row in theme_groups.select("theme").collect()]

# Create pie charts for each theme
for theme in themes:
    # Get the data for this theme
    theme_data = parsed_df.filter(col("theme") == theme) \
                         .select("language", "count") \
                         .toPandas()

    # Sort by count descending
    theme_data = theme_data.sort_values('count', ascending=False)

    if len(theme_data) > 5:  # Only group if more than 5 languages
        top5 = theme_data.head(5)
        others_row = pd.DataFrame({
            'language': ['Others'],
            'count': [theme_data['count'][5:].sum()]
        })
        theme_data = pd.concat([top5, others_row])

    # Create the pie chart
    fig = px.pie(theme_data,
                 values='count',
                 names='language',
                 title=f'Top Languages for "{theme}" Theme',
                 hover_data=['count'],
                 hole=0.3)

    fig.update_traces(
        textposition='inside',
        textinfo='percent+label',
        insidetextfont=dict(size=12, color='white'),
        hovertemplate="<b>%{label}</b><br>Count: %{value}<br>Percent: %{percent}"
    )

    fig.update_layout(
        uniformtext_minsize=10,
        uniformtext_mode='hide',
        height=600,
        showlegend=False
    )

    fig.show()

# Show overall theme distribution
total_counts = theme_groups.orderBy("total_count", ascending=False).toPandas()
fig = px.pie(total_counts,
             values='total_count',
             names='theme',
             title='Overall Theme Distribution',
             hover_data=['total_count'])

fig.update_traces(
    textposition='inside',
    textinfo='percent+label',
    insidetextfont=dict(size=12, color='white'),
    hovertemplate="<b>%{label}</b><br>Total Count: %{value}<br>Percent: %{percent}"
)

fig.update_layout(
    height=700,
    showlegend=False
)

fig.show()

# Stop Spark
spark.stop()

