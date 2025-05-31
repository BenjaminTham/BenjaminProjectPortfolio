import pickle
from json import dumps
from os import makedirs, getenv
from os.path import join
from pickle import load, dump
from shutil import copy
from uuid import uuid4

from flask import request, send_file

from biometrics.face import FaceVerification
# from biometrics.voicev2 import VoiceAuth # CHANGE VOICE
from biometrics.voice import VoiceAuth
from utils.db import get_db, release_db
from utils.mqtt import publish_mqtt

raw_files_folder = "biometrics/files"
embeddings_folder = "biometrics/embeddings"
backups_folder = "biometrics/backups"
folders = [raw_files_folder, embeddings_folder, backups_folder]
for folder in folders:
    makedirs(folder, exist_ok=True)

# Get FaceVerification and VoiceAuth objects
face_verification_config = {
    "model_path": "biometrics/mobilefacenet.tflite",
    "database_path": "biometrics/embeddings/face_embeddings.pkl",
    # Face detection & preprocessing settings:
    "model_selection": 0,
    "min_detection_confidence": 0.7,
    "padding": 0.2,
    "face_required_size": (512, 512),
    "target_size": (112, 112),
    # Verification settings:
    "verification_threshold": 0.7,
    "verification_timeout": 30,
    "verification_max_attempts": 3,
    # Camera settings:
    "camera_id": 0,
}

voice_auth_config = {
    "voiceprints_file": "biometrics/embeddings/voiceprints.pkl",
    "sr_rate": 44100,
    "num_mfcc": 20,
    "linear_threshold": 100,
    "cos_threshold": 0.95,
}
# CHANGE VOICE
# voice_auth_config = {
#     "enrollment_file": "update/voiceprints.pkl",
#     "sample_rate": 44100,
#     "threshold": 0.70,
# }

face_verification = FaceVerification(face_verification_config)
voice_auth = VoiceAuth(voice_auth_config)


def handle_enroll_biometrics(user_id):
    db, cursor = get_db()

    # Get user
    cursor.execute("SELECT * FROM users WHERE id = %s", (user_id,))
    user = cursor.fetchone()

    # User does not exist
    if not user:
        release_db(db)
        return {"message": "User does not exist"}, 400

    files = request.files

    # Check for 5 faces and 1 voice
    faces = [value for key, value in files.items() if key.startswith("face")]
    voice = files.get("voice")

    if len(faces) < 5:
        return {"message": "Missing face data"}, 400
    if not voice:
        return {"message": "Missing voice data"}, 400

    # Save raw biometrics
    face_paths = [join(raw_files_folder, f"{uuid4().hex}.png") for _ in faces]
    voice_path = join(raw_files_folder, f"{uuid4().hex}.wav")
    for face_path, face in zip(face_paths, faces):
        face.save(face_path)
    voice.save(voice_path)

    # Backup current embeddings
    for _ in ("face_embeddings.pkl", "voiceprints.pkl"):
        try:
            copy(join(embeddings_folder, _), join(backups_folder, _))
        except FileNotFoundError:  # No embedding yet
            pass

    # Build face embeddings
    face_success = face_verification.build_embedding_from_images(face_paths, user.get("username"))
    voice_success = voice_auth.enroll_user(user.get("username"), [voice_path]) # TODO: Change this if using different voice auth class


    # Enrollment failed
    if not (face_success and voice_success):
        release_db(db)
        # Rollback embeddings
        for _ in ("face_embeddings.pkl", "voiceprints.pkl"):
            try:
                copy(join(backups_folder, _), join(embeddings_folder, _))
            except FileNotFoundError:  # No embedding yet
                pass
        errors = []
        if not face_success:
            errors.append("Face enrollment failed")
        if not voice_success:
            errors.append("Voice enrollment failed")
        return {"message": "Biometrics enrollment failed", "errors": errors}, 400

    # Update biometrics enrolled for user
    cursor.execute("UPDATE users SET biometrics_enrolled = true WHERE id = %s", (user_id,))
    db.commit()
    release_db(db)

    # Send mqtt message about updated biometrics
    publish_mqtt("update/embedding", dumps({"face": "face_embeddings.pkl", "voice": "voiceprints.pkl"}))

    return {"message": "Biometrics enrolled successfully"}


def handle_delete_biometrics(user_id):
    db, cursor = get_db()

    # Get user
    cursor.execute("SELECT * FROM users WHERE id = %s", (user_id,))
    user = cursor.fetchone()

    # User does not exist
    if not user:
        release_db(db)
        return {"message": "User does not exist"}, 400

    # User has not enrolled biometrics
    if not user.get("biometrics_enrolled"):
        release_db(db)
        return {"message": "User has not enrolled biometrics"}, 400

    # Backup current embeddings
    for _ in ("face_embeddings.pkl", "voiceprints.pkl"):
        try:
            copy(join(embeddings_folder, _), join(backups_folder, _))
        except FileNotFoundError:  # No embedding yet
            pass

    try:
        for _ in ("face_embeddings.pkl", "voiceprints.pkl"):
            with open(join(embeddings_folder, _), "rb") as file:
                data = load(file)
                data.pop(user.get("username"), None)
            with open(join(embeddings_folder, _), "wb") as file:
                dump(data, file)
    except (FileNotFoundError, EOFError, pickle.UnpicklingError):
        # Rollback embeddings
        for _ in ("face_embeddings.pkl", "voiceprints.pkl"):
            try:
                copy(join(backups_folder, _), join(embeddings_folder, _))
            except FileNotFoundError:  # No embedding yet
                pass
        release_db(db)
        return {"message": "Biometrics deletion failed"}, 500

    # Update biometrics enrollment status for user
    cursor.execute("UPDATE users SET biometrics_enrolled = false WHERE id = %s", (user_id,))
    db.commit()
    release_db(db)

    # Pull new embeddings
    publish_mqtt("update/embedding", dumps({"face": "face_embeddings.pkl", "voice": "voiceprints.pkl"}))

    return {"message": "Biometrics deleted successfully"}


def handle_get_embeddings(token, filename):
    # Pre-shared token for security
    if token != getenv("EMBEDDINGS_TOKEN"):
        return {"message": "You are not authorised to get the embeddings"}, 401

    # Check if filename is embedding
    if filename not in ("face_embeddings.pkl", "voiceprints.pkl"):
        return {"message": "Invalid filename"}, 400

    return send_file(join(embeddings_folder, filename), as_attachment=True)
