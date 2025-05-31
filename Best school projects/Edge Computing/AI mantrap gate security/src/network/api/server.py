import os
from typing import List

import uvicorn
from fastapi import FastAPI, File, UploadFile

from auth.face import FaceVerification
from auth.voice import VoiceAuth


class FileUploadAPI():
    """
    Handles file uploading and user authentication using face verification and
    voice authentication systems.

    The `FileUploadAPI` class sets up routes for handling file uploads and processes
    the uploaded files for user enrollment in a face verification system and a voice
    authentication system.

    :ivar app: Instance of FastAPI used to handle HTTP requests and define routes.
    :type app: FastAPI
    :ivar face_verification: Instance of the FaceVerification system for
        processing and managing face authentication.
    :type face_verification: FaceVerification
    :ivar voice_auth: Instance of the VoiceAuth system for processing and managing
        voice authentication.
    :type voice_auth: VoiceAuth
    """

    def __init__(self, face_verification_config, voice_auth_config):
        """
        Initializes the application with given configurations for face verification
        and voice authentication. Sets up required routes and initializes external
        dependencies.

        Attributes:
            app (FastAPI): FastAPI instance used as the web framework.
            face_verification (FaceVerification): Instance of the FaceVerification
                class initialized with the provided configuration.
            voice_auth (VoiceAuth): Instance of the VoiceAuth class initialized
                with the provided configuration.

        :param face_verification_config: Provides configuration details for the
            face verification system.
        :type face_verification_config: dict
        :param voice_auth_config: Provides configuration details for the voice
            authentication system.
        :type voice_auth_config: dict
        """
        self.app = FastAPI()
        self.setup_routes()
        self.face_verification = FaceVerification(face_verification_config)
        self.voice_auth = VoiceAuth(voice_auth_config)

    def setup_routes(self):
        """
        Handles file upload routes and performs necessary processing for uploaded files.

        The `upload_files` route facilitates the asynchronous upload of multiple .wav files
        and a single .jpg file by users. It processes the provided files uploaded via an HTTP
        POST method, associating them with the specified username. The corresponding detailed
        operations performed on the files during the upload are handled by the `handle_upload`
        method.

        Parameters
        ----------
        username : str
            The username identifying the person performing the file upload.
        wav_files : List[UploadFile]
            A list of uploaded .wav files provided as part of the request payload.
        jpg_file : UploadFile
            A single .jpg file provided as part of the request payload.

        Returns
        -------
        Awaitable
            Returns the result of the `handle_upload` method after processing the uploaded
            files.

        """

        @self.app.post("/upload/")
        async def upload_files(
                username: str,
                wav_files: List[UploadFile] = File(...),
                jpg_file: UploadFile = File(...)
        ):
            return await self.handle_upload(username, wav_files, jpg_file)

    async def handle_upload(self, username: str, wav_files: List[UploadFile], jpg_file: UploadFile):
        """
        Processes the uploaded user data, saves the files to the specified directory, and performs operations
        to build embeddings for face verification and voice authentication. The function processes .jpg and
        .wav files and integrates with the respective subsystems for further authentication-related tasks.

        :param username: The username of the person to whom the uploaded files are associated.
        :type username: str
        :param wav_files: A list of audio files in .wav format uploaded by the user.
        :type wav_files: List[UploadFile]
        :param jpg_file: An image file in .jpg format uploaded by the user.
        :type jpg_file: UploadFile
        :return: A dictionary containing a message indicating the success or failure of the upload and processing operations.
        :rtype: dict
        """
        save_path = "uploads"
        os.makedirs(save_path, exist_ok=True)
        jpg_path = os.path.join(save_path, jpg_file.filename)
        wav_files_path = []
        for wav_file in wav_files:
            wav_files_path.append(os.path.join(save_path, wav_file.filename))
            with open(os.path.join(save_path, wav_file.filename), "wb") as buffer:
                buffer.write(wav_file.file.read())
        with open(jpg_path, "wb") as buffer:
            buffer.write(jpg_file.file.read())

        if not self.face_verification.build_embedding_from_image(jpg_path, username):
            return {"message": "Failed to build embedding from image"}
        if not self.voice_auth.enroll_user(username, wav_files_path):
            return {"message": "Failed to build embedding from voice"}

        return {"message": "Upload successful!"}


if __name__ == "__main__":
    face_verification_config = {
        "model_path": "mobilefacenet.tflite",
        "database_path": "face_embeddings.pkl",
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
        "voiceprints_file": "voiceprints.pkl",
        "sr_rate": 44100,
        "num_mfcc": 20,
        "linear_threshold": 100,
        "cos_threshold": 0.95,
    }
    file_upload_api = FileUploadAPI(face_verification_config, voice_auth_config)
    uvicorn.run(file_upload_api.app, host="0.0.0.0", port=8000)
