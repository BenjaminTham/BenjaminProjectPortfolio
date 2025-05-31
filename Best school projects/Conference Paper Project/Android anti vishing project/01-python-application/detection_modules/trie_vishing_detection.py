import pandas as pd
import string
import re
from collections import Counter
import csv
import time

# Preprocessing start time
preprocessing_start_time = time.time()

class TrieNode:
    def __init__(self):
        self.children = {}
        self.end_of_word = False

class Trie:
    def __init__(self):
        self.root = TrieNode()

    def insert(self, word):
        current = self.root
        for char in word:
            if char not in current.children:
                current.children[char] = TrieNode()
            current = current.children[char]
        current.end_of_word = True

    def search(self, word):
        current = self.root
        for char in word:
            if char not in current.children:
                return False
            current = current.children[char]
        return current.end_of_word

    def count_words(self, node=None):
        if node is None:
            node = self.root
        count = 1 if node.end_of_word else 0
        for child in node.children.values():
            count += self.count_words(child)
        return count

def tokenize_text(text):
    text = text.lower()
    text = re.sub(r'(mr|mrs|ms|miss|dr|prof|officer)\.\s+\w+', '', text)
    text = text.translate(str.maketrans('', '', string.punctuation))
    tokens = text.split()
    with open('resources/stopwords_greetings_pronouns.txt', 'r') as f:
        stop_words = f.read().splitlines()
    words = [w for w in tokens if not w in stop_words]
    return words, len(tokens)

fraud_df = pd.read_csv('resources/fraud_dataset.csv')
nonfraud_df = pd.read_csv('resources/non_fraud_dataset.csv')

fraud_df['Transcript'], fraud_df['Word_Count'] = zip(*fraud_df['Transcript'].apply(tokenize_text))
nonfraud_df['Non_Frauds'], nonfraud_df['Word_Count'] = zip(*nonfraud_df['Non_Frauds'].apply(tokenize_text))

fraud_word_freq = Counter()
nonfraud_word_freq = Counter()
for index, row in fraud_df.iterrows():
    fraud_word_freq.update(row['Transcript'])
for index, row in nonfraud_df.iterrows():
    nonfraud_word_freq.update(row['Non_Frauds'])

for word in nonfraud_word_freq:
    if word in fraud_word_freq:
        del fraud_word_freq[word]

top_n = 30
fraud_types = fraud_df['Fraud_Type'].unique()
top_words = {fraud_type: Trie() for fraud_type in fraud_types}

sum_freq_top_words = 0

for fraud_type in fraud_types:
    fraud_type_words = fraud_df[fraud_df['Fraud_Type'] == fraud_type]['Transcript'].explode()
    top_fraud_type_words = fraud_type_words.value_counts().head(top_n)
    for word, freq in top_fraud_type_words.items():
        top_words[fraud_type].insert(word)
        sum_freq_top_words += freq

total_words_fraud = fraud_df['Word_Count'].sum()
perc_top_words_fraud = (sum_freq_top_words / total_words_fraud) * 100

def detect_vishing(input_sample: str):
    text = re.sub(r'\W+', ' ', input_sample.lower())
    total_matches = 0
    fraud_type_matches = {fraud_type: 0 for fraud_type in fraud_types}

    for word in text.split():
        for fraud_type in fraud_types:
            if top_words[fraud_type].search(word):
                total_matches += 1
                fraud_type_matches[fraud_type] += 1

    perc_matches = (total_matches / len(text.split())) * 100 if total_matches else 0
    fraud_likelihood = (perc_matches / perc_top_words_fraud) * 100
    fraud_likelihood = max(0, min(100, fraud_likelihood))

    most_likely_fraud_type = max(fraud_type_matches, key=fraud_type_matches.get)

    predicted_label = "Vishing" if fraud_likelihood >= 50 else "Not Vishing"

    return predicted_label, fraud_likelihood, most_likely_fraud_type

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
    processing_time_sum = 0

    for idx, test_case in enumerate(test_cases, 1):
        processing_start_time = time.time()

        input_sample, expected_label = test_case[1], test_case[2]
        predicted_label, fraud_likelihood, most_likely_fraud_type = detect_vishing(input_sample)

        processing_end_time = time.time()
        processing_time = processing_end_time - processing_start_time
        # Increment total processing time
        processing_time_sum += processing_time

        # Compare the predicted and expected labels
        if predicted_label == expected_label:
            accuracy_sum += 1

        print(f"Test Case {idx}:")
        print(f"Text: {input_sample}")
        print(f"Expected Label: {expected_label}")
        print(f"Predicted Label: {predicted_label}")
        if predicted_label == "Vishing":
            print(f"Predicted Fraud Type: {most_likely_fraud_type}")
        print(f"Fraud Likelihood: {fraud_likelihood:.2f}%")
        print(f"Processing Time: {processing_time:.4f} seconds")
        print()

    # Calculate average accuracy
    avg_accuracy = (accuracy_sum / total_datasets) * 100
    print(f"Average Accuracy: {avg_accuracy:.2f}%")

    # Calculate average processing time per test case
    avg_processing_time = processing_time_sum / total_datasets
    print(f"Preprocessing Time: {PREPROCESSING_TIME:.4f} seconds")
    print(f"Average Processing Time per test case: {avg_processing_time:.4f} seconds")
    print()

