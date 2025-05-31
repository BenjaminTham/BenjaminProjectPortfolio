import base64
import logging
import os
import pickle
import time

import cv2
import mediapipe as mp
import numpy as np
import tensorflow as tf
from scipy.spatial.distance import cosine

from utils.logger_mixin import LoggerMixin


class FaceVerification(LoggerMixin):
    """
    The FaceVerification class is designed for managing and conducting face verification tasks
    using a combination of MediaPipe for face detection and a MobileFaceNet TFLite model.

    The class helps in detecting faces from image frames, extracting and preprocessing face crops,
    computing face embeddings, saving embeddings to a database, and performing face recognition
    using similarity comparison against saved embeddings. Additionally, it supports building a
    face recognition database from individual image files.

    The class finds applications in systems requiring face-based authentication or recognition.

    :ivar mp_face_detection: MediaPipe face detection module.
    :type mp_face_detection: Any
    :ivar mp_drawing: MediaPipe drawing utilities.
    :type mp_drawing: Any
    :ivar model_path: Path to the MobileFaceNet TFLite model file.
    :type model_path: str
    :ivar database_path: Path to the face embedding database file.
    :type database_path: str
    :ivar model_selection: Model selection configuration for MediaPipe face detection.
    :type model_selection: int
    :ivar min_detection_confidence: Minimum confidence required for detection via MediaPipe.
    :type min_detection_confidence: float
    :ivar padding: Padding factor added around detected face bounding boxes.
    :type padding: float
    :ivar face_required_size: Target size (width, height) for extracted face images.
    :type face_required_size: tuple[int, int]
    :ivar target_size: Image size (width, height) required as model input.
    :type target_size: tuple[int, int]
    :ivar verification_threshold: Threshold value for similarity used during verification.
    :type verification_threshold: float
    :ivar verification_timeout: Timeout interval for the face verification process.
    :type verification_timeout: float
    :ivar verification_max_attempts: Maximum number of attempts allowed for verification.
    :type verification_max_attempts: int
    :ivar camera_id: ID of the camera used for capturing frames during verification.
    :type camera_id: int
    :ivar logger: Instance of logger used for logging messages.
    :type logger: logging.Logger
    :ivar _interpreter: TensorFlow Lite interpreter for executing the MobileFaceNet model.
    :type _interpreter: tf.lite.Interpreter or None
    """

    def __init__(
            self,
            face_verification_config,
            logging_level=logging.INFO,
    ):
        """
        Initializes the FaceVerification class with a specified configuration for face
        verification and sets up logging for the instance.

        The constructor takes in a configuration dictionary and logging level to set up
        various attributes required for face detection, verification, and logging. It
        loads paths and parameters from the configuration, prepares MediaPipe utilities
        for face detection, and initializes configurable properties of the system.

        :param face_verification_config: A dictionary containing face verification
            system configuration. This includes paths, detection parameters, and
            verification settings such as thresholds, timeout, and maximum attempts.
        :type face_verification_config: dict

        :param logging_level: The logging level for the instance logger. Defaults to
            ``logging.INFO``.
        :type logging_level: int or logging level
        """
        self.mp_face_detection = mp.solutions.face_detection
        self.mp_drawing = mp.solutions.drawing_utils

        self.model_path = face_verification_config["model_path"]
        self.database_path = face_verification_config["database_path"]

        # Set configurable properties as instance attributes.
        self.model_selection = face_verification_config["model_selection"]
        self.min_detection_confidence = face_verification_config[
            "min_detection_confidence"
        ]
        self.padding = face_verification_config["padding"]
        self.face_required_size = face_verification_config["face_required_size"]
        self.target_size = face_verification_config["target_size"]
        self.verification_threshold = face_verification_config["verification_threshold"]
        self.verification_timeout = face_verification_config["verification_timeout"]
        self.verification_max_attempts = face_verification_config[
            "verification_max_attempts"
        ]
        self.camera_id = face_verification_config["camera_id"]
        self.logger = self.setup_logger(__name__, logging_level)
        self._interpreter = None

    def get_tflite_interpreter(self):
        """
        Obtains the TensorFlow Lite interpreter for the MobileFaceNet model.

        This method initializes and returns a TensorFlow Lite Interpreter object
        for the MobileFaceNet model. If the interpreter is not already created, it
        loads the model from the specified path, allocates tensors, and logs the
        progress during the initialization.

        :return: TensorFlow Lite interpreter for the MobileFaceNet model.
        :rtype: tf.lite.Interpreter
        """
        if self._interpreter is None:
            self.logger.info("Loading MobileFaceNet TFLite model...")
            self._interpreter = tf.lite.Interpreter(model_path=self.model_path)
            self._interpreter.allocate_tensors()
            self.logger.info("Model loaded successfully.")
        return self._interpreter

    def detect_face(self, frame):
        """
        Detects a single face in the given frame using the Mediapipe face detection model
        and returns the bounding box coordinates of the detected face. The bounding box
        coordinates are computed based on the relative bounding box data provided by
        the detection results and scaled to the original frame dimensions. If no face
        is detected, None is returned.

        :param frame: A NumPy array representing the input video frame in BGR color
            format. Typically, this is a frame extracted from a video stream or a
            single image.

        :return: A tuple (x, y, width, height) indicating the top-left corner (x, y)
            and the width and height of the bounding box of the detected face in pixel
            coordinates. Returns None if no detection is made.
        """
        frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

        with self.mp_face_detection.FaceDetection(
                model_selection=self.model_selection,
                min_detection_confidence=self.min_detection_confidence,
        ) as face_detection:
            results = face_detection.process(frame_rgb)

            if results.detections:
                detection = results.detections[0]
                bbox = detection.location_data.relative_bounding_box

                h, w, _ = frame.shape
                x = max(0, int(bbox.xmin * w))
                y = max(0, int(bbox.ymin * h))
                width = min(int(bbox.width * w), w - x)
                height = min(int(bbox.height * h), h - y)

                return (x, y, width, height)
        return None

    def extract_face(self, frame, face_location):
        """
        Extracts a face region from the provided video frame, using the given face location and
        applies resizing with additional padding. This method determines the bounding box of
        the detected face based on face location coordinates, adjusts for padding, and ensures
        coordinates are within frame bounds. Finally, it resizes the extracted face to
        desired dimensions.

        :param frame: The video frame containing the face region.
        :param face_location: Tuple containing the (x, y) coordinates, width, and height
            of the detected face region within the frame.
        :return: Cropped and resized face image as a numpy array.
        """
        x, y, width, height = face_location
        h, w, _ = frame.shape

        padding_x = int(width * self.padding)
        padding_y = int(height * self.padding)

        x1 = max(0, x - padding_x)
        y1 = max(0, y - padding_y)
        x2 = min(w, x + width + padding_x)
        y2 = min(h, y + height + padding_y)

        face = frame[y1:y2, x1:x2]
        face = cv2.resize(face, self.face_required_size)

        return face

    def preprocess_face(self, face_img):
        """
        Preprocesses an input face image for further analysis. The function ensures
        the input image meets the target size requirements and applies normalization
        operations to prepare the image. These preprocessing steps include resizing,
        adjusting the color space, and normalization to a specific scale. The result
        is a numpy array suitable for processing in machine learning or deep learning
        pipelines.

        :param face_img: The input face image represented as a numpy array in BGR
            color space.
        :return: A preprocessed face image as a numpy array with normalized
            pixel values, suitable for further model input.
        """
        # Helper functions for image preprocessing
        def adjust_white_balance(img):
            result = cv2.cvtColor(img.astype('uint8'), cv2.COLOR_BGR2LAB)
            avg_a = np.average(result[:, :, 1])
            avg_b = np.average(result[:, :, 2])
            result[:, :, 1] = result[:, :, 1] - ((avg_a - 128) * (result[:, :, 0] / 255.0) * 1.1)
            result[:, :, 2] = result[:, :, 2] - ((avg_b - 128) * (result[:, :, 0] / 255.0) * 1.1)
            return cv2.cvtColor(result, cv2.COLOR_LAB2BGR).astype('float32')

        def adjust_gamma(img, gamma=1.0):
            return ((img / 255.0) ** gamma * 255.0).astype('uint8')

        def normalize_contrast(img):
            lab = cv2.cvtColor(img.astype('uint8'), cv2.COLOR_BGR2LAB)
            l, a, b = cv2.split(lab)
            l_norm = ((l - l.min()) * (255.0 / (l.max() - l.min()))).astype('uint8')
            return cv2.cvtColor(cv2.merge([l_norm, a, b]), cv2.COLOR_LAB2BGR).astype('float32')

        # Resize if necessary
        if face_img.shape[:2] != self.target_size:
            face_img = cv2.resize(face_img, self.target_size)

        # Calculate average brightness and determine gamma value
        avg_brightness = np.mean(cv2.cvtColor(face_img.astype('uint8'), cv2.COLOR_BGR2GRAY))
        if avg_brightness < 20:  # Very dark images
            gamma = 0.3  # Aggressive brightening
        elif avg_brightness < 128:  # Dark images
            gamma = 0.7  # Moderate brightening
        else:  # Bright images
            gamma = 1.2  # Slight darkening

        # Apply image corrections in sequence
        face_balanced = adjust_white_balance(face_img)
        face_gamma = adjust_gamma(face_balanced, gamma)
        face_normalized = normalize_contrast(face_gamma)

        face_rgb = cv2.cvtColor(face_img, cv2.COLOR_BGR2RGB)
        face_normalized = face_rgb.astype("float32")
        face_normalized = (face_normalized - 127.5) / 128.0
        face_normalized = np.expand_dims(face_normalized, axis=0)

        return face_normalized

    def get_face_embedding(self, face_img):
        """
        Computes the facial embedding for a given face image. The function utilizes a
        TensorFlow Lite interpreter to extract the embedding from the given image
        input, normalizes the embedding vector, and returns it as a flattened array.

        :param face_img: A numpy array representing the input face image. The array
            should adhere to the expected input format required by the TensorFlow Lite
            model. Its dimensions and data type should match the input details obtained
            from the model interpreter.
        :return: A 1D numpy array representing the normalized facial embedding vector.
        """
        interpreter = self.get_tflite_interpreter()
        input_details = interpreter.get_input_details()
        output_details = interpreter.get_output_details()

        interpreter.set_tensor(input_details[0]["index"], face_img)
        interpreter.invoke()
        embedding = interpreter.get_tensor(output_details[0]["index"])

        embedding = embedding / np.linalg.norm(embedding)
        return embedding.flatten()

    def save_face_embeddings(self, embeddings_dict):
        """
        Saves face embeddings to a file, merging them with any pre-existing embeddings.
        Updates the embeddings dictionary stored in a file at the given database path using
        serialization. Ensures old embeddings are preserved and new embeddings are added.

        :param embeddings_dict: A dictionary where keys are identifiers (e.g., names or IDs)
                                and values are their corresponding embeddings.
        :return: A boolean indicating whether the operation was successful.
        """
        try:
            existing_embeddings = {}
            if os.path.exists(self.database_path):
                with open(self.database_path, "rb") as f:
                    existing_embeddings = pickle.load(f)

            existing_embeddings.update(embeddings_dict)

            with open(self.database_path, "wb") as f:
                pickle.dump(existing_embeddings, f)

            self.logger.info(
                f"Saved {len(embeddings_dict)} embeddings to {self.database_path}"
            )
            return True
        except Exception as e:
            self.logger.error(f"Error saving embeddings: {str(e)}")
            return False

    def load_face_embeddings(self):
        """
        Loads face embeddings from a database file and returns the data.

        The method attempts to load a serialized embeddings database file
        from the specified path. If the file does not exist, a warning is logged
        and an empty dictionary is returned. If there is an error during the file
        loading or deserialization process, the error is logged and an empty
        dictionary is returned.

        :raises Exception: If an error occurs during file deserialization.

        :return: A dictionary containing loaded face embeddings or an empty dictionary
                 if the database file is not found or an error occurs.
        :rtype: dict
        """
        if not os.path.exists(self.database_path):
            self.logger.warning(f"Embedding database not found: {self.database_path}")
            return {}

        try:
            with open(self.database_path, "rb") as f:
                embeddings_db = pickle.load(f)
            return embeddings_db
        except Exception as e:
            self.logger.error(f"Error loading embeddings: {str(e)}")
            return {}

    def build_embedding_from_folder(self, base_folder):
        """
        Generates and stores face embeddings from a folder structure where each subfolder
        represents a person and contains their face images.

        The folder structure should be:
        base_folder/
            person1_name/
                image1.jpg
                image2.jpg
                ...
            person2_name/
                image1.jpg
                ...

        :param base_folder: Path to the base folder containing person subfolders
        :type base_folder: str
        :return: A boolean indicating whether the embedding computation and saving were
            successful.
        :rtype: bool
        """
        if not os.path.exists(base_folder):
            self.logger.error(f"Base folder does not exist: {base_folder}")
            return False

        embeddings_dict = {}

        # Iterate through person folders
        for person_name in os.listdir(base_folder):
            person_folder = os.path.join(base_folder, person_name)
            if not os.path.isdir(person_folder):
                continue

            self.logger.info(f"Processing images for person: {person_name}")
            person_embeddings = []

            # Process each image in person's folder
            for image_name in os.listdir(person_folder):
                if not image_name.lower().endswith(('.png', '.jpg', '.jpeg')):
                    continue

                img_path = os.path.join(person_folder, image_name)

                try:
                    self.logger.info(f"Processing {img_path}...")
                    img = cv2.imread(img_path)
                    if img is None:
                        self.logger.error(f"Could not load image {img_path}")
                        continue

                    face_location = self.detect_face(img)
                    if not face_location:
                        self.logger.warning(f"No face detected in {img_path}")
                        continue

                    face = self.extract_face(img, face_location)
                    preprocessed_face = self.preprocess_face(face)
                    embedding = self.get_face_embedding(preprocessed_face)
                    person_embeddings.append(embedding)

                except Exception as e:
                    self.logger.error(f"Error processing {img_path}: {str(e)}")
                    continue

            if person_embeddings:
                embeddings_dict[person_name] = person_embeddings
                self.logger.info(f"Successfully processed {len(person_embeddings)} images for {person_name}")
            else:
                self.logger.warning(f"No valid embeddings generated for {person_name}")

        if embeddings_dict:
            self.save_face_embeddings(embeddings_dict)
            self.logger.info(f"Saved embeddings for {len(embeddings_dict)} persons")
            return True

        self.logger.warning("No embeddings were generated")
        return False

    def build_embedding_from_images(self, image_paths, person_name):
        """
        Generates and stores face embeddings from a list of image paths for a single person.

        :param image_paths: List of paths to images of the person
        :type image_paths: List[str]
        :param person_name: Name of the person
        :type person_name: str
        :return: A boolean indicating whether the embedding computation and saving were successful
        :rtype: bool
        """
        if not image_paths:
            self.logger.error("No image paths provided")
            return False

        embeddings_dict = {}
        person_embeddings = []

        # Process each image path
        for img_path in image_paths:
            if not os.path.exists(img_path):
                self.logger.error(f"Image does not exist: {img_path}")
                continue

            try:
                self.logger.info(f"Processing {img_path}...")
                img = cv2.imread(img_path)
                if img is None:
                    self.logger.error(f"Could not load image {img_path}")
                    continue

                face_location = self.detect_face(img)
                if not face_location:
                    self.logger.warning(f"No face detected in {img_path}")
                    continue

                face = self.extract_face(img, face_location)
                preprocessed_face = self.preprocess_face(face)
                embedding = self.get_face_embedding(preprocessed_face)
                person_embeddings.append(embedding)

            except Exception as e:
                self.logger.error(f"Error processing {img_path}: {str(e)}")
                continue

        if person_embeddings:
            embeddings_dict[person_name] = person_embeddings
            self.save_face_embeddings(embeddings_dict)
            self.logger.info(f"Successfully processed {len(person_embeddings)} images for {person_name}")
            return True

        self.logger.warning(f"No valid embeddings generated for {person_name}")
        return False

    def verify_face(self, frame=None):
        """
        Performs face verification by detecting a face in an input frame or capturing one
        from a camera feed, and comparing it to a stored database of face embeddings.

        If a frame is not provided, the method attempts to capture one from the camera
        specified by the `camera_id` attribute. Each detected face is processed and
        compared to the embeddings stored in the internal database to find the best match.

        The comparison is done using cosine similarity between the face embedding of the
        query face and those of the stored embeddings. Only matches that exceed the
        `verification_threshold` are considered valid.

        :param frame:
            Optional input frame to verify. If not provided, a frame will be captured from
            the camera specified by `camera_id`.

        :return:
            The method returns a tuple `(best_match, best_similarity)` if a match is
            found, where `best_match` is the name of the recognized person and
            `best_similarity` is the similarity score. Returns `None` if no match
            is found or if any error occurs during the process.
        """
        embeddings_db = self.load_face_embeddings()
        if not embeddings_db:
            self.logger.warning("No faces in database to compare against")
            return None

        need_release = False
        if frame is None:
            self.logger.info(f"Initializing camera {self.camera_id}...")
            cap = cv2.VideoCapture(self.camera_id)
            if not cap.isOpened():
                self.logger.error(f"Error: Could not open camera {self.camera_id}")
                return None

            ret, frame = cap.read()
            if not ret:
                self.logger.error("Error capturing frame")
                cap.release()
                return None
            need_release = True

        try:
            face_location = self.detect_face(frame)
            if not face_location:
                self.logger.debug("No face detected in frame")
                if need_release:
                    cap.release()
                return None

            face = self.extract_face(frame, face_location)
            preprocessed_face = self.preprocess_face(face)
            query_embedding = self.get_face_embedding(preprocessed_face)

            best_match = None
            best_similarity = -1

            for name, stored_embeddings in embeddings_db.items():
                try:
                    # Compare with all embeddings for this person
                    for stored_embedding in stored_embeddings:
                        similarity = 1 - cosine(query_embedding, stored_embedding)
                        if similarity > best_similarity:
                            best_similarity = similarity
                            best_match = name
                except Exception as e:
                    self.logger.error(f"Error comparing with {name}: {str(e)}")
                    continue

            if best_similarity >= self.verification_threshold:
                self.logger.info(
                    f"Face recognized as {best_match} with similarity {best_similarity:.4f}"
                )
                result = (best_match, best_similarity)
            else:
                self.logger.info(
                    f"No match found (best similarity: {best_similarity:.4f})"
                )
                result = None

        except Exception as e:
            self.logger.error(f"Error during face verification: {str(e)}")
            result = None

        if need_release:
            cap.release()

        return result

    def wait_for_face_and_verify(self):
        """
        Waits for a face to appear in the camera's view and verifies the detected face against
        a predefined criterion. The process continues until a successful verification, the
        maximum number of verification attempts is reached or the timeout expires.

        This method initializes the camera, continuously captures frames, detects faces, and
        executes a verification process. If a face is successfully verified, the recognized
        identity is returned. On failure, an appropriate log is generated, and the method
        returns None.

        :param self: Instance of the class that includes logger, camera parameters,
            and verification configurations.

        :returns: The recognized identity as a string if verification is successful. If
            no face is detected within the timeout or the verification process fails multiple
            times, it returns None.
        :rtype: Optional[str]
        """
        self.logger.info(f"Initializing camera {self.camera_id}...")
        cap = cv2.VideoCapture(self.camera_id)

        if not cap.isOpened():
            self.logger.error(f"Error: Could not open camera {self.camera_id}")
            return None

        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

        self.logger.info("Waiting for face...")
        start_time = time.time()
        attempts = 0

        try:
            while (
                    time.time() - start_time < self.verification_timeout
                    and attempts < self.verification_max_attempts
            ):
                ret, frame = cap.read()
                if not ret:
                    self.logger.error("Error capturing frame")
                    continue

                face_location = self.detect_face(frame)
                if face_location:
                    attempts += 1
                    self.logger.info(
                        f"Face detected! Verification attempt {attempts}/{self.verification_max_attempts}..."
                    )
                    result = self.verify_face(frame)
                    if result:
                        self.logger.info("Verification successful!")
                        self.logger.info(
                            f"Face recognized as {result[0]} with similarity {result[1]:.4f}"
                        )
                        return result[0]
                    elif attempts == self.verification_max_attempts:
                        self.logger.warning("Max attempts reached.")
                        return None
                    else:
                        self.logger.info("Verification failed. Trying again...")
                        time.sleep(0.5)
                time.sleep(0.1)
        finally:
            cap.release()

    def capture_mismatch(self):
        # Open the camera
        cap = cv2.VideoCapture(self.camera_id)
        if not cap.isOpened():
            self.logger.error("Error: Could not open camera.")
            return

        # Capture a single frame
        ret, frame = cap.read()
        if ret:
            # Encode the image in memory as JPEG
            _, buffer = cv2.imencode('.jpg', frame)

            # Convert the buffer to Base64
            image_base64 = base64.b64encode(buffer).decode('utf-8')

            # Release the camera
            cap.release()

            # Return the Base64-encoded image
            return image_base64
        else:
            # Release the camera
            cap.release()

            self.logger.error("Failed to capture image.")
            return None


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

    face_verification = FaceVerification(face_verification_config)


    def detect_and_capture_face(face_verifier, output_folder, person_name=None):
        """
        Capture an image using the webcam and save it.

        Args:
            face_verifier (FaceVerification): Instance of FaceVerification class.
            output_folder (str): Folder to save the captured image.
            person_name (str): Name of the person in the image.

        Returns:
            Path to saved image or None if capture failed.
        """
        # Create the output folder (base) if it doesn't exist
        if not os.path.exists(output_folder):
            os.makedirs(output_folder)

        # If person name is provided, create the person's folder
        if person_name:
            output_folder = os.path.join(output_folder, person_name)

            if not os.path.exists(output_folder):
                os.makedirs(output_folder)

        # Use timestamp as filename
        filename = str(int(time.time()))

        output_path = os.path.join(output_folder, f"{filename}.jpg")

        face_verifier.logger.info(f"Initializing camera {face_verifier.camera_id}...")
        cap = cv2.VideoCapture(face_verifier.camera_id)
        if not cap.isOpened():
            face_verifier.logger.error(
                f"Error: Could not open camera {face_verifier.camera_id}"
            )
            return None

        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

        face_verifier.logger.info("Capturing image...")
        # Give a little time for camera to initialize
        time.sleep(1)

        ret, frame = cap.read()
        cap.release()

        if not ret:
            face_verifier.logger.error("Error capturing frame")
            return None

        cv2.imwrite(output_path, frame)
        face_verifier.logger.info(f"Image saved to {output_path}")

        return output_path


    while True:
        if input("Press Enter to capture face or 'q' to quit...").lower() == "q":
            break

        name = input("Enter your name: ")
        base_folder = "faces"

        face_verification.logger.info("Capturing face image...")
        img_path = detect_and_capture_face(face_verification, "faces", name)
        if img_path:
            face_verification.logger.info(f"Face image saved to {img_path}")
            face_verification.logger.info("Building face embeddings from images...")
            face_verification.build_embedding_from_images([img_path], name)

    face_verification.logger.info("Running face verification demo...")
    result = face_verification.wait_for_face_and_verify()
    face_verification.logger.info("Face verification result:", result)
