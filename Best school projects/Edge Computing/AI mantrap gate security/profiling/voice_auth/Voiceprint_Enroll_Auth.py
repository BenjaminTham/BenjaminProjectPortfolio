import pickle
from record_audio import record_audio
from audio_feature_extraction import extract_features, extract_words, hashString
from scipy.spatial.distance import cosine
from sklearn.metrics.pairwise import cosine_similarity
import os
import numpy as np
import time


def enroll_user(user_name, num_samples=3):
    enrollment_data = []

    for i in range(num_samples):
        filename = f"{user_name}_sample{i}.wav"
        record_audio(filename)
        features = extract_features(filename)
        hashed_words = hashString(extract_words(filename))
        print(hashed_words)
        enrollment_data.append({
            "features": features,
            "hashed_words": hashed_words
        })

    # Delete the wav files
    for i in range(num_samples):
        filename = f"{user_name}_sample{i}.wav"
        os.remove(filename)

    # If all hashed words are not the same, ask the user to re-enroll
    if len(set([sample["hashed_words"] for sample in enrollment_data])) != 1:
        print("Enrollment failed. Please try again with the same word.")
        return


    # Save user voiceprint
    with open(f"{user_name}_voiceprint.pkl", "wb") as f:
        pickle.dump(enrollment_data, f)

    print(f"Enrollment complete for {user_name}.")


        

def authenticate_user(user_name, linear_threshold=20, cos_threshold=0.95):

    # Load enrolled voiceprints
    try:
        with open(f"{user_name}_voiceprint.pkl", "rb") as f:
            enrolled_data = pickle.load(f)
    except FileNotFoundError:
        print("User not found!")
        return False

    # Record test sample
    test_filename = "test_sample.wav"
    record_audio(test_filename)

    # Extract features from test sample
    test_features = extract_features(test_filename)
    test_hashed_words = hashString(extract_words(test_filename))


    # Compare test features with stored voiceprints
    # Calculate Euclidean distance of test features with each enrolled voiceprint
    distances = [np.linalg.norm(test_features - sample["features"]) for sample in enrolled_data]
    similarities = cosine_similarity([test_features], [sample["features"] for sample in enrolled_data])

    # Calculate average distance
    avg_distance = np.mean(distances)

    # Calculate average similarity
    avg_similarity = np.mean(similarities)
    
    # Check if the hashed words from the test sample match any enrolled sample's hash.
    hash_matches = [test_hashed_words == sample["hashed_words"] for sample in enrolled_data]

    print(f"Distance: {avg_distance}")
    print(f"Similarity: {avg_similarity}")

    os.remove(test_filename)

    if avg_distance < linear_threshold and avg_similarity > cos_threshold and any(hash_matches):
        print("Authentication successful!")
        return True
    else:
        print("Authentication failed.")
        return False
    

if __name__ == "__main__":
    USER_TO_ENROLL = "Krabby"
    USER_TO_AUTH = "Krabby"

    # print(f"Enrolling now {USER_TO_ENROLL} now")
    # enroll_user(USER_TO_ENROLL)

    time.sleep(2)
    print(f"Authenticating {USER_TO_AUTH} now")
    authenticate_user(USER_TO_AUTH)
