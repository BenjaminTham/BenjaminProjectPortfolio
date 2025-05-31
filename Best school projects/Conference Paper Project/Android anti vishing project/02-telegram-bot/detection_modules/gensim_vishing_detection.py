import pandas as pd
from gensim import corpora, models, similarities
import nltk
nltk.download('stopwords')
from nltk.corpus import stopwords
from nltk.tokenize import word_tokenize
import time
import csv

# Preprocessing start time
preprocessing_start_time = time.time()

# Load the fraud dataset from CSV
fraud_dataset = pd.read_csv('resources/fraud_dataset.csv')

# Load the non-fraud dataset from CSV
nonfraud_dataset = pd.read_csv('resources/non_fraud_dataset.csv')

def preprocess_sentence(sentence):
    # Define the stopwords
    stop_words = set(stopwords.words('english'))
    # Tokenize the sentence into words
    tokens = [token.lower() for token in word_tokenize(sentence) if token.lower() not in stop_words and token.isalpha()]
    return tokens

def detect_vishing(statement):
    # Preprocess the reference sentence
    reference_tokens = preprocess_sentence(statement)
    
    # Transform the reference sentence to the TF-IDF vector representation
    reference_vec = fraud_dictionary.doc2bow(reference_tokens)
    tfidf = models.TfidfModel(fraud_corpus + nonfraud_corpus)
    reference_tfidf = tfidf[reference_vec]

    # Create similarity index for fraud and nonfraud sentences
    fraud_index = similarities.MatrixSimilarity(tfidf[fraud_corpus])
    nonfraud_index = similarities.MatrixSimilarity(tfidf[nonfraud_corpus])

    # Calculate the similarity scores between the reference sentence and fraud and nonfraud sentences
    fraud_similarity_scores = fraud_index[reference_tfidf]
    nonfraud_similarity_scores = nonfraud_index[reference_tfidf]

    # Get the maximum similarity score for fraud and nonfraud dataset
    max_similarity_fraud = max(fraud_similarity_scores)
    max_similarity_nonfraud = max(nonfraud_similarity_scores)

    # Determine if the reference sentence is a potential vishing attempt or a genuine phone call
    if max_similarity_fraud > max_similarity_nonfraud:
        return "Vishing", max_similarity_fraud, max_similarity_nonfraud
    else:
        return "Not Vishing", max_similarity_fraud, max_similarity_nonfraud


# Extract the text column from the fraud and nonfraud dataset and preprocess them
fraud_sentences = fraud_dataset['Transcript'].apply(preprocess_sentence).tolist()
nonfraud_sentences = nonfraud_dataset['Non_Frauds'].apply(preprocess_sentence).tolist()

# Create a dictionary and corpus for the fraud and nonfraud sentences
fraud_dictionary = corpora.Dictionary(fraud_sentences)
nonfraud_dictionary = corpora.Dictionary(nonfraud_sentences)
fraud_corpus = [fraud_dictionary.doc2bow(tokens) for tokens in fraud_sentences]
nonfraud_corpus = [nonfraud_dictionary.doc2bow(tokens) for tokens in nonfraud_sentences]

# preprocessing time end
preprocessing_end_time = time.time()
PREPROCESSING_TIME  = preprocessing_end_time - preprocessing_start_time

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
        predicted_label, fraud_sim, nonfraud_sim = detect_vishing(input_sample)
        processing_end_time = time.time()
        processing_time = processing_end_time - processing_start_time
        proccess_time_sum += processing_time
        if predicted_label == expected_label:
            accuracy_sum += 1

        print(f"Test Case {idx}:")
        print(f"Text: {input_sample}")
        print(f"Expected Label: {expected_label}")
        print(f"Predicted Label: {predicted_label}")
        print(f"Fraud Sim: {fraud_sim}")
        print(f"Non-Fraud Sim: {nonfraud_sim}")
        print(f"Processing Time: {processing_time:.4f}s")        
        print()

    avg_accuracy = (accuracy_sum / total_datasets) * 100
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {PREPROCESSING_TIME:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")
