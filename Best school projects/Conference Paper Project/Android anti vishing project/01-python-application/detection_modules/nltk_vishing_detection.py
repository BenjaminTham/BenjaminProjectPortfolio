import csv
import nltk
import time
from nltk.corpus import stopwords
from nltk.metrics.distance import edit_distance
from nltk.tokenize import word_tokenize
from nltk.stem import WordNetLemmatizer

preprocessing_start_time = time.time()

nltk.download('stopwords')
nltk.download('punkt')
nltk.download('wordnet')

def preprocess_text(text):
    # Tokenize the text
    tokens = word_tokenize(text.lower())

    # Remove stopwords
    stop_words = set(stopwords.words('english'))
    tokens = [token for token in tokens if token not in stop_words]

    # Lemmatize the tokens
    lemmatizer = WordNetLemmatizer()
    tokens = [lemmatizer.lemmatize(token) for token in tokens]

    # Join the tokens back into a string
    preprocessed_text = ' '.join(tokens)
    return preprocessed_text

def calculate_similarity(reference_sentence, dataset):
    reference_sentence = preprocess_text(reference_sentence)
    reference_tokens = word_tokenize(reference_sentence)

    similarity_scores = []

    for transcript in dataset:
        transcript = preprocess_text(transcript)
        transcript_tokens = word_tokenize(transcript)

        distance = edit_distance(reference_tokens, transcript_tokens)
        similarity_score = 1 - (distance / max(len(reference_tokens), len(transcript_tokens)))
        similarity_scores.append(similarity_score)

    average_similarity = sum(similarity_scores) / len(similarity_scores)
    return average_similarity * 100

def load_dataset(file_path):
    dataset = []
    with open(file_path, 'r') as file:
        reader = csv.reader(file)
        for row in reader:
            dataset.append(row[0])  # assuming the text is in the first column
    return dataset

def detect_vishing(reference_sentence):
    fraud_similarity = calculate_similarity(reference_sentence, fraud_dataset)
    nonfraud_similarity = calculate_similarity(reference_sentence, nonfraud_dataset)
    
    return 'Vishing' if fraud_similarity > nonfraud_similarity else 'Not Vishing', fraud_similarity, nonfraud_similarity

fraud_dataset = load_dataset('resources/fraud_dataset.csv')
nonfraud_dataset = load_dataset('resources/non_fraud_dataset.csv')

preprocessing_end_time = time.time()
PREPROCESSING_TIME  = preprocessing_end_time - preprocessing_start_time

if __name__ == '__main__':
    
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
        predicted_label, fraud_similarity, nonfraud_similarity = detect_vishing(input_sample)
        
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
        print(f"Fraud Similarity: {fraud_similarity}")
        print(f"Non-Fraud Similarity: {nonfraud_similarity}")
        print(f"Processing Time: {processing_time:.4f}s")        
        print()

    # Calculate average accuracy
    avg_accuracy = (accuracy_sum / total_datasets) * 100
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {PREPROCESSING_TIME:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")
