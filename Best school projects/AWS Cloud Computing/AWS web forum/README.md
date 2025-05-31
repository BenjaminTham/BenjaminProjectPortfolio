
---

# Big Data Project: Social Media Analysis

## Table of Contents
1. [Introduction](#introduction)  
2. [Objectives](#objectives)  
3. [Dataset](#dataset)  
4. [Key Analyses & Pipelines](#key-analyses--pipelines)  
5. [Architecture Overview](#architecture-overview)  
6. [Team Contributions](#team-contributions)  
7. [Limitations](#limitations)  
8. [References](#references)

---

## Introduction
Traditional feedback methods (e.g., surveys, interviews) often fail to capture timely and diverse user sentiments. In this project, we analyzed **65 million** multilingual social media posts from December 1–7, 2024, providing a near real-time, global perspective on user discussions, sentiments, and emotions.

### Why Social Media?
- **Rich, unfiltered data**: Users share posts in real time, capturing genuine opinions and trends.  
- **Diverse perspectives**: Multi-language posts from worldwide audiences.  
- **High-volume insights**: Large data size (65M+ entries) enables robust statistical and machine learning applications.  

---

## Objectives
1. **Analyze Social Media Trends**  
   - Identify sentiment, emotion, and theme patterns from raw text.  
   - Visualize insights to reveal patterns in user behavior and content.  
   - Predict future sentiment or emotion classifications using ML.

2. **Process Big Data Efficiently**  
   - Integrate **Hadoop**, **MapReduce**, **HDFS**, **YARN**, **Spark/PySpark** to handle large datasets.  
   - Utilize **AWS EMR** for scalable, distributed data processing with minimal cluster-management overhead.

---

## Dataset
- **Source**: [Exorde Labs](https://huggingface.co/datasets/Exorde/exorde-social-media-december-2024-week1)  
- **Description**:  
  - **Multi-source, multi-language** social media data  
  - **Time range**: 1 week (December 1–7, 2024)  
  - **Attributes**: Date, original_text, language, sentiment (numeric), main_emotion, english_keywords, etc.  
  - Ideal for analyzing emotions, trends, and sentiment patterns on a large scale.

---

## Key Analyses & Pipelines

### 1. **Sentiment Word Cloud Analysis**
- **Goal**: Identify top words by sentiment (positive vs. negative)  
- **MapReduce**:  
  - Mapper: Cleans and tokenizes `original_text`; filters English language posts; classifies words by positive or negative sentiment.  
  - Reducer: Aggregates counts of each word.  
- **Spark**:  
  - Sorts aggregated data and filters top frequent words for each sentiment.  
  - Outputs lists for **WordCloud** visualizations.

### 2. **Emotions Trend Analysis**
- **Goal**: Track how emotions (e.g., joy, sadness, anger) fluctuate daily over a week and detect outliers.  
- **MapReduce**:  
  - Extracts timestamps and emotions, grouping them by date.  
- **Spark**:  
  - Time-series analysis to see emotion frequencies across days.  
  - Identifies top three emotions per day + outlier detection via Z-scores.  
  - Visualized with line charts and bar plots for daily emotion trends and anomalies.

### 3. **Language & Theme Analysis**
- **Goal**: Explore relationships between user language and the themes of their posts (e.g., Business, Entertainment).  
- **MapReduce**:  
  - Merges language codes (ISO 639-1) with post data.  
  - Aggregates frequency of each theme per language.  
- **Spark**:  
  - Determines popular languages overall, popular themes overall, and which themes have highest linguistic diversity.  
  - Visualized via pie charts and tables, revealing how certain themes dominate in certain languages.

### 4. **Sentiment Prediction & Emotion Classification** (Machine Learning)
- **Goal**: Predict sentiment or classify emotions based on keywords.  
- **MapReduce**:  
  - Pre-processes data into **LIBSVM** format for PySpark MLlib (handling multi-keyword cells with commas).  
- **Spark**:  
  - **Linear Regression** (Sentiment Prediction):  
    - Uses `english_keywords` as features to predict sentiment.  
    - Evaluates performance (RMSE, R²).  
  - **Logistic Regression** (Emotion Classification):  
    - Uses `english_keywords` to classify into main_emotion.  
    - Evaluates accuracy, F1-score, confusion matrix, etc.  

---

## Architecture Overview

1. **Data Ingestion & Storage**  
   - Raw dataset stored in **AWS S3** → Loaded into **HDFS** on EMR.

2. **Validation & Cleaning (MapReduce)**  
   - Filters out malformed rows or missing fields.  

3. **Task-Specific MapReduce Jobs**  
   - Each analysis (Sentiment, Emotion, Language, ML) has its own **MapReduce** job to aggregate/format data.  

4. **Spark Analysis**  
   - Reads MapReduce output → In-memory transformations/analyses (sorting, grouping, ML pipelines).  

5. **Visualization**  
   - **Matplotlib**, **WordCloud**, etc. for final charts and word clouds.  

6. **AWS EMR Cluster Management**  
   - Automated resource scaling (YARN)  
   - Separate EMR steps scheduled sequentially:  
     1. Validation  
     2. Aggregation (MapReduce)  
     3. Analytics (Spark)  
     4. Visualization  

---

## Team Contributions
| Name                   | Contributions                                                           |
|------------------------|-------------------------------------------------------------------------|
| **Lam Jun Xian, Aaron**   | Sentiment Word Cloud Analysis & AWS Integration                      |
| **Low Yue Qian**          | Language and Theme Analysis                                          |
| **Kong Teng Foong, Victor** | Emotions Trend Analysis                                              |
| **Tan Heng Joo**          | Sentiment Prediction (Linear Regression)                             |
| **Tham Yu Bin Benjamin**   | Main Emotion Classification (Logistic Regression)                    |

---

## Limitations
1. **Data Quality & Noise**  
   - Slang, abbreviations, emojis, missing data → affects sentiment/emotion accuracy.  

2. **Scalability & Performance**  
   - 65+ million entries still require significant computation time and resources.  
   - Cloud costs can escalate with further scaling.  

3. **ML Accuracy & Bias**  
   - Skewed or incomplete data can bias results.  
   - Linear & logistic regression may not capture complex contextual nuances.  

4. **Integration & Maintenance**  
   - Multi-layered pipeline (MapReduce + Spark + AWS) requires careful coordination.  
   - Adapting to new social media sources/formats requires ongoing maintenance.  

---

## References
- ExordeLabs. (2024). exorde-social-media-december-2024-week1. Huggingface.co. [Link](https://huggingface.co/datasets/Exorde/exorde-social-media-december-2024-week1)
