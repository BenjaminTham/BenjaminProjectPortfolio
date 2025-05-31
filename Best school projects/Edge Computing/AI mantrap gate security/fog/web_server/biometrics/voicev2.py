import logging
import os
import pickle
import time
from pathlib import Path

import noisereduce as nr
import numpy as np
import sounddevice as sd
import speech_recognition as sr  # For voice code extraction
from resemblyzer import VoiceEncoder, preprocess_wav
from scipy.io.wavfile import write
from sklearn.metrics.pairwise import cosine_similarity


class VoiceAuth():
    """A class for voice verification using voice embeddings and similarity comparison."""

    def __init__(self, voice_config: dict):
        """
        Initialize the VoiceAuth with configurable parameters.

        Args:
            sample_rate: Audio sampling rate in Hz.
            threshold: Similarity threshold for verification (0-1).
            enrollment_file: Filename for storing enrollment data.
        """
        self.sample_rate = voice_config["sample_rate"]
        self.threshold = voice_config["threshold"]
        self.encoder = VoiceEncoder()
        self.enrollment_file = voice_config["enrollment_file"]

        # Create output directory if needed
        os.makedirs("verification", exist_ok=True)

    def record_audio(self, filename, duration=5):
        """
        Record audio from the microphone.

        Args:
            filename: Output filename.
            duration: Recording duration in seconds.

        Returns:
            Path object to the recorded audio file.
        """
        print(f"\nRecording {duration} seconds of audio...")
        print("Please speak now...")

        # Start recording
        recording = sd.rec(int(duration * self.sample_rate),
                           samplerate=self.sample_rate,
                           channels=1)

        # Progress bar simulation
        for i in range(duration):
            time.sleep(1)
            print(f"Recording: {i + 1}/{duration} seconds")

        sd.wait()
        print("Recording finished")

        # Normalize and convert to int16 for PCM format
        norm_recording = recording / np.max(np.abs(recording)) * 0.9
        recording_int16 = np.int16(norm_recording * 32767)

        # Save the recording in PCM WAV format
        write(filename, self.sample_rate, recording_int16)
        print(f"Audio saved to {filename}")
        return Path(filename)

    def preprocess_audio(self, audio_file):
        """
        Load and preprocess audio with noise reduction.

        Args:
            audio_file: Path to audio file.

        Returns:
            Preprocessed audio data.
        """
        try:
            # Load the audio file using resemblyzer's preprocessing function
            wav_raw = preprocess_wav(audio_file)
            # Apply noise reduction
            wav_processed = nr.reduce_noise(y=wav_raw, sr=self.sample_rate)
            return wav_processed
        except Exception as e:
            raise Exception(f"Error preprocessing audio: {str(e)}")

    def generate_embedding(self, audio_data):
        """
        Generate voice embedding from preprocessed audio.

        Args:
            audio_data: Preprocessed audio data.

        Returns:
            Voice embedding vector.
        """
        return self.encoder.embed_utterance(audio_data)

    def extract_voice_code(self, audio_file):
        """
        Extract a voice code (text) from an audio file using speech recognition.

        Args:
            audio_file: Path to audio file.

        Returns:
            Recognized text (voice code).
        """
        recognizer = sr.Recognizer()
        with sr.AudioFile(str(audio_file)) as source:
            audio_data = recognizer.record(source)
        try:
            # Using Google's speech recognition engine for demonstration
            voice_code = recognizer.recognize_google(audio_data)
            print(f"Extracted voice code: {voice_code}")
            return voice_code
        except sr.UnknownValueError:
            print("Speech Recognition could not understand audio")
            return ""
        except sr.RequestError as e:
            print(f"Could not request results; {e}")
            return ""

    def load_enrollments(self):
        """
        Load enrollments from the pickle file.

        Returns:
            Dictionary of enrollments.
        """
        if os.path.exists(self.enrollment_file):
            with open(self.enrollment_file, "rb") as f:
                enrollments = pickle.load(f)
            return enrollments
        else:
            return {}

    def save_enrollments(self, enrollments):
        """
        Save enrollments to the pickle file.

        Args:
            enrollments: Dictionary of enrollment data.
        """
        with open(self.enrollment_file, "wb") as f:
            pickle.dump(enrollments, f)
        print(f"Enrollments saved to {self.enrollment_file}")

    def enroll_user(self, username, wav_file):
        """
        Enroll a new user using an existing WAV file.

        Args:
            username: The user's name.
            wav_file: Path to the WAV file to use for enrollment.
        """
        wav_path = Path(wav_file)
        if not wav_path.exists():
            print(f"Error: File {wav_file} does not exist.")
            return False

        # Preprocess audio and generate embedding from the file
        wav_processed = self.preprocess_audio(wav_path)
        embedding = self.generate_embedding(wav_processed)

        # Extract voice code using speech recognition
        voice_code = self.extract_voice_code(wav_path)

        # Load existing enrollments
        enrollments = self.load_enrollments()

        # Save new enrollment data
        enrollments[username] = {"voice_code": voice_code, "embedding": embedding}
        self.save_enrollments(enrollments)
        print(f"User '{username}' enrolled successfully using file '{wav_file}'.")
        return True

    def authenticate_user(self, username, duration=5):
        """
        Verify a user's identity by comparing a new voice sample to the enrolled data.
        Both voice code (text) and embedding similarity are used for verification.

        Args:
            username: The username to verify.
            duration: Recording duration in seconds.

        Returns:
            Boolean indicating whether the user is verified.
        """
        # Load enrollment data
        enrollments = self.load_enrollments()
        if username not in enrollments:
            print(f"User '{username}' not found in enrollments.")
            return False

        # Record a new audio sample for verification
        verify_filename = f"verification/{username}_verify.wav"
        audio_path = self.record_audio(verify_filename, duration)

        # Preprocess audio and generate embedding
        wav_processed = self.preprocess_audio(audio_path)
        embedding_verify = self.generate_embedding(wav_processed)

        # Extract voice code from the verification sample
        voice_code_verify = self.extract_voice_code(audio_path)

        # Retrieve stored enrollment data
        stored_voice_code = enrollments[username]["voice_code"]
        stored_embedding = enrollments[username]["embedding"]

        # Compare voice codes (simple case-insensitive string comparison)
        code_match = voice_code_verify.strip().lower() == stored_voice_code.strip().lower()
        if not code_match:
            print("Voice code does not match.")
            return False

        # Calculate cosine similarity between the stored and current embeddings
        similarity = cosine_similarity(stored_embedding.reshape(1, -1), embedding_verify.reshape(1, -1))[0][0]
        print(f"Cosine similarity: {similarity:.3f}")

        if similarity > self.threshold:
            print("User verified successfully!")
            return True
        else:
            print("Voice embedding similarity below threshold. Verification failed.")
            return False


# Example usage:
if __name__ == "__main__":
    verifier = VoiceAuth()

    # Enroll a user using an existing WAV file
    username = "Choonkeat"
    wav_file_path = "mac.wav"
    verifier.enroll_user(username, wav_file_path)

    # Later, verify the same user (this will record a new sample for verification)
    result = verifier.authenticate_user(username, duration=5)
    print("Verification result:", result)
