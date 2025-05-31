import pandas as pd
import string
import re
from collections import Counter
import csv
import pprint
import time

# Preprocessing start time
preprocessing_start_time = time.time()

# Function to preprocess and tokenize text
def tokenize_text(text):
    # Convert to lower case
    text = text.lower()

    # Remove salutations and names following them
    text = re.sub(r'(mr|mrs|ms|miss|dr|prof|officer)\.\s+\w+', '', text)

    # Remove punctuation
    text = text.translate(str.maketrans('', '', string.punctuation))

    # Tokenize - convert from string to list of words
    tokens = text.split()

    # Calculate total number of tokens before removing stop words
    word_count = len(tokens)

    # Filter out stopwords, greetings, pronouns
    with open('resources/stopwords_greetings_pronouns.txt', 'r') as f:
        stop_words = f.read().splitlines()
    words = [w for w in tokens if not w in stop_words]

    return words, word_count

# Function to calculate word frequencies
def calculate_frequencies(df, column):
    word_freq = Counter()
    for index, row in df.iterrows():
        word_freq.update(row[column])
    return word_freq

# Function to prepare failure function for KMP
def prepare_failure_function(word):
    failure = [0]
    j = 0
    for i in range(1, len(word)):
        while j > 0 and word[i] != word[j]:
            j = failure[j-1]
        if word[i] == word[j]:
            j += 1
        failure.append(j)
    return failure

# Load the datasets
fraud_df = pd.read_csv('resources/fraud_dataset.csv')
nonfraud_df = pd.read_csv('resources/non_fraud_dataset.csv')

# Preprocess and tokenize text
fraud_df['Transcript'], fraud_df['Word_Count'] = zip(*fraud_df['Transcript'].apply(tokenize_text))
nonfraud_df['Non_Frauds'], nonfraud_df['Word_Count'] = zip(*nonfraud_df['Non_Frauds'].apply(tokenize_text))

# Calculate total words
total_words_fraud = fraud_df['Word_Count'].sum()
total_words_nonfraud = nonfraud_df['Word_Count'].sum()

# Calculate word frequencies
fraud_word_freq = calculate_frequencies(fraud_df, 'Transcript')
nonfraud_word_freq = calculate_frequencies(nonfraud_df, 'Non_Frauds')

# Remove common words from the fraud word frequencies based on the non-fraud word frequencies
for word in nonfraud_word_freq:
    if word in fraud_word_freq:
        del fraud_word_freq[word]

# Get top 20 words for each fraud type
top_n = 30
fraud_types = fraud_df['Fraud_Type'].unique()
top_words = {fraud_type: {} for fraud_type in fraud_types}

for fraud_type in fraud_types:
    # Filter fraud words by fraud type
    fraud_type_words = fraud_df[fraud_df['Fraud_Type'] == fraud_type]['Transcript'].explode()
    # Count the word frequencies and get the top n words
    top_fraud_type_words = fraud_type_words.value_counts().head(top_n)

    for word, freq in top_fraud_type_words.items():
        # Prepare failure function for KMP
        failure_function = prepare_failure_function(word)
        top_words[fraud_type][word] = {'freq': freq, 'failure_function': failure_function}

# Remove shared words from the top_words and add them to the shared_fraud_words category
shared_fraud_words = {}

for fraud_type in fraud_types:
    for word in list(top_words[fraud_type].keys()):
        shared = False
        for other_fraud_type in fraud_types:
            if other_fraud_type != fraud_type and (word in top_words[other_fraud_type] or word in shared_fraud_words):
                shared_fraud_words[word] = top_words[fraud_type].pop(word)
                shared = True
                break
        if shared:
            top_words[fraud_type] = dict(top_words[fraud_type])  # Convert back to regular dictionary

# Add shared_fraud_words category to top_words
top_words['shared_fraud_words'] = shared_fraud_words

# Now the top_words dictionary contains the top 20 words for each fraud type and shared_fraud_words

# Print the top_words dictionary with formatting

pp = pprint.PrettyPrinter(indent=4)

for fraud_type, words_info in top_words.items():
    print(f"Fraud Type: {fraud_type}")
    print("Top Words:")
    for word in words_info.keys():
        print(word)
    print()

# Now you can use `top_words` dictionary in your further analysis

# KMP search function
def kmp_search(text, pattern, failure):
    i, j = 0, 0
    matches = 0

    while i < len(text):
        if pattern[j] == text[i]:
            i += 1
            j += 1
            if j == len(pattern):
                matches += 1
                j = failure[j - 1]
        else:
            if j != 0:
                j = failure[j - 1]
            else:
                i += 1
    return matches

# Calculate the sum of the word frequencies in top_words
sum_freq_top_words = sum(word_info['freq'] for fraud_type in top_words for word_info in top_words[fraud_type].values())

# Calculate the percentages
perc_top_words_fraud = (sum_freq_top_words / total_words_fraud) * 100

def detect_vishing(input_sample: str):
    # Preprocess and tokenize text
    # Don't forget to load your datasets here if you haven't done so globally
    # You should probably cache your datasets and top_words after initial load

    # Prepare text input
    text = re.sub(r'\W+', ' ', input_sample.lower())
    total_matches = 0
    fraud_type_matches = {fraud_type: 0 for fraud_type in fraud_types}

    # Apply KMP algorithm to each word in our `top_words` object
    for fraud_type in fraud_types:
        for word, word_info in top_words[fraud_type].items():
            matches = kmp_search(text, word, word_info['failure_function'])
            total_matches += matches
            fraud_type_matches[fraud_type] += matches

    # Calculate percentage of matches
    perc_matches = (total_matches / len(text.split())) * 100 if total_matches else 0

    # Determine fraud likelihood based on comparison with calculated percentage for top fraud words
    fraud_likelihood = (perc_matches / perc_top_words_fraud) * 120
    # Ensuring the percentage stays in the range [0, 100]
    fraud_likelihood = max(0, min(100, fraud_likelihood))

    # Identify most likely fraud type
    most_likely_fraud_type = max(fraud_type_matches, key=fraud_type_matches.get)

    # Determine the predicted label
    predicted_label = "Vishing" if fraud_likelihood >= 50 else "Not Vishing"

    return predicted_label, fraud_likelihood, most_likely_fraud_type

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
        predicted_label, fraud_likelihood, most_likely_fraud_type = detect_vishing(input_sample)

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
        if predicted_label == "Vishing":
            print(f"Predicted Fraud Type: {most_likely_fraud_type}")
        print(f"Fraud Likelihood: {fraud_likelihood}")
        print(f"Processing Time: {processing_time:.4f}s")        
        print()

    # Calculate average accuracy
    avg_accuracy = (accuracy_sum / total_datasets) * 100
    preprocessing_time = preprocessing_end_time - preprocessing_start_time
    avg_proccess_time = proccess_time_sum / total_datasets

    print(f"Average Accuracy: {avg_accuracy:.2f}%")
    print(f"Preprocessing Time: {preprocessing_time:.4f}s")
    print(f"Average Processing Time: {avg_proccess_time:.4f}s")