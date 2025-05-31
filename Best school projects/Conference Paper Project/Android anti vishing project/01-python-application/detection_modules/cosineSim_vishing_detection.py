from nltk.tokenize import word_tokenize
from nltk.corpus import stopwords
import spacy
import re
import pandas as pd
import string
import csv
import time

# Preprocessing start time
preprocessing_start_time = time.time()

def preprocess_sentence(sentence):
    # Remove special characters
    sentence = re.sub(r"[^\w\s']", "", sentence)
    # Tokenize the sentence into words
    tokens = word_tokenize(sentence)
    # Remove stopwords
    stop_words = set(stopwords.words("english"))
    tokens = [token for token in tokens if token.lower() not in stop_words]
    # Remove punctuations
    tokens = [token for token in tokens if token not in string.punctuation]
    # Join the tokens back into a sentence string
    preprocessed_sentence = " ".join(tokens)
    # Manually remove single quotation marks
    preprocessed_sentence = preprocessed_sentence.replace("'", "")
    return preprocessed_sentence


def calculate_similarity(sentence1, sentence2):
    # Preprocess the sentences
    sentence1 = preprocess_sentence(sentence1)
    sentence2 = preprocess_sentence(sentence2)

    # Tokenize the sentences into words
    words1 = sentence1.split()
    words2 = sentence2.split()

    # Create a set of unique words from both sentences
    unique_words = set(words1).union(set(words2))

    # Initialize word frequency dictionaries
    frequency1 = dict.fromkeys(unique_words, 0)
    frequency2 = dict.fromkeys(unique_words, 0)

    # Calculate word frequencies in the first sentence
    for word in words1:
        frequency1[word] += 1

    # Calculate word frequencies in the second sentence
    for word in words2:
        frequency2[word] += 1

    # Calculate the dot product and magnitudes
    dot_product = sum(frequency1[word] * frequency2[word] for word in unique_words)
    magnitude1 = sum(frequency1[word] ** 2 for word in unique_words) ** 0.5
    magnitude2 = sum(frequency2[word] ** 2 for word in unique_words) ** 0.5

    # Calculate cosine similarity
    similarity = dot_product / (magnitude1 * magnitude2)

    return similarity

def detect_vishing(statement):
    # Define the reference sentence
    refSentence = statement
    preprocessedRefSentence = preprocess_sentence(refSentence)

    totalFraudSimilarity = 0.0
    numFraudSentences = 0
    # Iterate through each row of the fraud dataset
    for index, row in fraudDataSet.iterrows():
        sentence = row["Transcript"]
        similarity = calculate_similarity(sentence, preprocessedRefSentence)
        totalFraudSimilarity += similarity
        numFraudSentences += 1

    if numFraudSentences > 0:
        avgFraudSimilarity = totalFraudSimilarity / numFraudSentences
    else:
        avgFraudSimilarity = 0.0

    totalNonFraudSimilarity = 0.0
    numNonFraudSentences = 0
    # Iterate through each row of the non-fraud dataset
    for index, row in nonFraudDataSet.iterrows():
        sentence = row["Non_Frauds"]
        similarity = calculate_similarity(sentence, preprocessedRefSentence)
        totalNonFraudSimilarity += similarity
        numNonFraudSentences += 1

    if numNonFraudSentences > 0:
        avgNonFraudSimilarity = totalNonFraudSimilarity / numNonFraudSentences
    else:
        avgNonFraudSimilarity = 0.0

    # Normalize the similarity scores to the range [0, 1]
    similarity_range = max(avgFraudSimilarity, avgNonFraudSimilarity) - min(
        avgFraudSimilarity, avgNonFraudSimilarity
    )
    if similarity_range > 0:
        normalizedFraudSimilarity = (
            avgFraudSimilarity - min(avgFraudSimilarity, avgNonFraudSimilarity)
        ) / similarity_range
        normalizedNonFraudSimilarity = (
            avgNonFraudSimilarity - min(avgFraudSimilarity, avgNonFraudSimilarity)
        ) / similarity_range
    else:
        normalizedFraudSimilarity = 0.0
        normalizedNonFraudSimilarity = 0.0

    # Determine if the statement is vishing based on Cosine Similarity
    predicted_label = "Vishing" if normalizedFraudSimilarity > normalizedNonFraudSimilarity else "Not Vishing"

    return predicted_label, normalizedFraudSimilarity, normalizedNonFraudSimilarity

nlp = spacy.load("en_core_web_lg")

# Load the fraud dataset
fraudDataSet = pd.read_csv("resources/fraud_dataset.csv")
# Preprocess the "Transcript" column using the preprocess_sentence function
fraudDataSet["Transcript"] = fraudDataSet["Transcript"].apply(preprocess_sentence)
# Save the preprocessed dataset to a new CSV file
fraudDataSet.to_csv("resources/preprocessed_fraud_dataset.csv", index=False)

# Load the non-fraud dataset
nonFraudDataSet = pd.read_csv("resources/non_fraud_dataset.csv")
# Preprocess the "Non_Frauds" column using the preprocess_sentence function
nonFraudDataSet["Non_Frauds"] = nonFraudDataSet["Non_Frauds"].apply(preprocess_sentence)
# Save the preprocessed dataset to a new CSV file
nonFraudDataSet.to_csv("resources/preprocessed_non_fraud_dataset.csv", index=False)

# preprocessing time end
preprocessing_end_time = time.time()
PREPROCESSING_TIME  = preprocessing_end_time - preprocessing_start_time

if __name__ == "__main__":
    # This will only be executed when you run this script directly
    # and not when you import from another script.
    test_cases = []
    with open('resources/test_cases.csv', 'r') as file:
        reader = csv.reader(file)
        for row in reader:
            test_cases.append(row)

    # Initialize accuracy and total datasets
    accuracy_sum = 0
    total_datasets = len(test_cases)
    proccess_time_sum = 0

    # Loop through test cases
    for idx, test_case in enumerate(test_cases, 1):

        # Processing Start Time
        processing_start_time = time.time()

        input_sample, expected_label = test_case[1], test_case[2]
        predicted_label, normalizedFraudSimilarity, normalizedNonFraudSimilarity = detect_vishing(input_sample)
        
        # Processing End Time
        processing_end_time = time.time()
        processing_time = processing_end_time - processing_start_time
        proccess_time_sum += processing_time

        # Check if the predicted label matches the expected label
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

    # Calculate average accuracy
    avg_accuracy = (accuracy_sum / total_datasets) * 100
    preprocessing_time = preprocessing_end_time - preprocessing_start_time
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {preprocessing_time:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")