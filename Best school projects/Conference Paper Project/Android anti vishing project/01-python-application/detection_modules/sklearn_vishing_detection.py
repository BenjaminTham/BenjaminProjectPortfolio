import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
from sklearn.pipeline import Pipeline
from sklearn.base import BaseEstimator, TransformerMixin
from nltk.corpus import stopwords
from nltk.tokenize import word_tokenize
import time
import re
import string
import csv

# Preprocessing start time
preprocessing_start_time = time.time()

class TextPreprocessor(BaseEstimator, TransformerMixin):
    def __init__(self):
        self.stop_words = set(stopwords.words('english'))

    def fit(self, X, y=None):
        return self

    def transform(self, X):
        preprocessed_X = []
        for sentence in X:
            sentence = re.sub(r"[^\w\s']", "", sentence)
            tokens = word_tokenize(sentence.lower())
            tokens = [token for token in tokens if token not in self.stop_words and token not in string.punctuation]
            preprocessed_X.append(' '.join(tokens))
        return preprocessed_X

class CosineSimilarityTransformer(BaseEstimator, TransformerMixin):
    def fit(self, X, y=None):
        return self

    def transform(self, X):
        return cosine_similarity(X)

def detect_vishing(statement):
    preprocessor = TextPreprocessor()
    pipeline = Pipeline([
        ('tfidf', TfidfVectorizer()),
        ('cosine_sim', CosineSimilarityTransformer())
    ])

    preprocessed_statement = preprocessor.transform([statement])

    fraud_dataset = pd.read_csv('resources/fraud_dataset.csv')
    non_fraud_dataset = pd.read_csv('resources/non_fraud_dataset.csv')

    preprocessed_fraud_dataset = preprocessor.transform(fraud_dataset['Transcript'].tolist())
    preprocessed_non_fraud_dataset = preprocessor.transform(non_fraud_dataset['Non_Frauds'].tolist())

    pipeline.fit(preprocessed_fraud_dataset + preprocessed_non_fraud_dataset)

    fraud_similarity_scores = pipeline.transform(preprocessed_statement)
    non_fraud_similarity_scores = pipeline.transform(preprocessed_statement)

    avg_fraud_similarity = fraud_similarity_scores.mean()
    avg_non_fraud_similarity = non_fraud_similarity_scores.mean()

    normalized_fraud_similarity = avg_fraud_similarity / (avg_fraud_similarity + avg_non_fraud_similarity)
    normalized_non_fraud_similarity = avg_non_fraud_similarity / (avg_fraud_similarity + avg_non_fraud_similarity)

    predicted_label = "Vishing" if normalized_fraud_similarity > normalized_non_fraud_similarity else "Not Vishing"

    return predicted_label, normalized_fraud_similarity, normalized_non_fraud_similarity

# Preprocessing end time
preprocessing_end_time = time.time()
PREPROCESSING_TIME = preprocessing_end_time - preprocessing_start_time

if __name__ == "__main__":
    test_cases = []
    with open('resources/test_cases.csv', 'r') as file:
        reader = csv.reader(file)
        for row in reader:
            test_cases.append(row)

    accuracy_sum = 0
    total_datasets = len(test_cases)
    proccess_time_sum = 0

    for idx, test_case in enumerate(test_cases, 1):
        processing_start_time = time.time()

        input_sample, expected_label = test_case[1], test_case[2]
        predicted_label, normalizedFraudSimilarity, normalizedNonFraudSimilarity = detect_vishing(input_sample)
        
        processing_end_time = time.time()
        processing_time = processing_end_time - processing_start_time
        proccess_time_sum += processing_time

        if predicted_label == expected_label:
            accuracy_sum += 1

        print(f"Test Case {idx}:")
        print(f"Text: {input_sample}")
        print(f"Expected Label: {expected_label}")
        print(f"Predicted Label: {predicted_label}")
        print(f"Norm Fraud Sim: {normalizedFraudSimilarity}")
        print(f"Norm Non-Fraud Sim: {normalizedNonFraudSimilarity}")
        print(f"Processing Time: {processing_time:.4f}s")        
        print()

    avg_accuracy = (accuracy_sum / total_datasets) * 100
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {PREPROCESSING_TIME:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")
