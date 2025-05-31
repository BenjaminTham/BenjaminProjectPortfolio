import librosa
import hashlib as hash
import numpy as np
import speech_recognition as sr

def extract_features(filename) -> np.ndarray:
    y, sr = librosa.load(filename, sr=44100) # Load the audio file
    y = librosa.effects.preemphasis(y)  # Pre-emphasis filter
    mfccs = librosa.feature.mfcc(y=y, sr=sr, n_mfcc=20)  # Extract MFCC features
    return np.mean(mfccs.T, axis=0)  # Take mean to get a fixed-size feature vector

def extract_words(filename):
    recognizer = sr.Recognizer()

    with sr.AudioFile(filename) as source:
        audio = recognizer.record(source)

    try:
        rec = recognizer.recognize_google(audio)
        print("The audio file contains: " + rec)
        return rec
    except sr.UnknownValueError:
        print("Could not understand the audio")
        return ""
    except sr.RequestError as e:
        print("Could not request results; {0}".format(e))
        return ""


def hashString(string:str) -> str:  #
    return hash.sha256(string.encode()).hexdigest()