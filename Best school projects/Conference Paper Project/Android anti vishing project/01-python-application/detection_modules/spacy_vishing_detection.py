import pandas as pd
import spacy
import time
import csv

# Preprocessing start time
preprocessing_start_time = time.time()

# Load the spaCy English model
nlp = spacy.load("en_core_web_lg")

def preprocess_sentence(sentence):
    doc = nlp(sentence)
    tokens = [token.lemma_.lower() for token in doc if not token.is_stop and token.is_alpha]
    preprocessed_sentence = " ".join(tokens)
    return preprocessed_sentence

def calculate_similarity(sentence1, sentence2):
    doc1 = nlp(sentence1)
    doc2 = nlp(sentence2)
    similarity = doc1.similarity(doc2)
    return similarity

def detect_vishing(statement):
    reference_sentence = preprocess_sentence(statement)

    # Calculate the similarity between the reference sentence and fraud sentences
    similarity_scores_fraud = []
    for sentence in fraud_sentences:
        similarity = calculate_similarity(sentence, reference_sentence)
        similarity_scores_fraud.append(similarity)

    # Calculate the similarity between the reference sentence and non-fraud sentences
    similarity_scores_nonfraud = []
    for sentence in nonfraud_sentences:
        similarity = calculate_similarity(sentence, reference_sentence)
        similarity_scores_nonfraud.append(similarity)

    avg_similarity_fraud = sum(similarity_scores_fraud) / len(similarity_scores_fraud)
    avg_similarity_nonfraud = sum(similarity_scores_nonfraud) / len(similarity_scores_nonfraud)

    return "Vishing" if avg_similarity_fraud > avg_similarity_nonfraud else "Non-Vishing", avg_similarity_fraud, avg_similarity_nonfraud

# Load the fraud dataset from CSV
fraud_dataset = pd.read_csv('resources/fraud_dataset.csv')
# Extract the text column from the fraud dataset and preprocess
fraud_sentences = [preprocess_sentence(x) for x in fraud_dataset['Transcript'].tolist()]

# Load the non-fraud dataset from CSV
nonfraud_dataset = pd.read_csv('resources/non_fraud_dataset.csv')
# Extract the text column from the non-fraud dataset and preprocess
nonfraud_sentences = [preprocess_sentence(x) for x in nonfraud_dataset['Non_Frauds'].tolist()]

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
        predicted_label, avg_similarity_fraud, avg_similarity_nonfraud = detect_vishing(input_sample)

        processing_end_time = time.time()
        processing_time = processing_end_time - processing_start_time
        proccess_time_sum += processing_time

        if predicted_label == expected_label:
            accuracy_sum += 1

        print(f"Test Case {idx}:")
        print(f"Text: {input_sample}")
        print(f"Expected Label: {expected_label}")
        print(f"Predicted Label: {predicted_label}")
        print(f"Fraud Similarity: {avg_similarity_fraud}")
        print(f"Non-Fraud Similarity: {avg_similarity_nonfraud}")
        print(f"Processing Time: {processing_time:.4f}s")        
        print()

    avg_accuracy = (accuracy_sum / total_datasets) * 100
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {PREPROCESSING_TIME:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")
