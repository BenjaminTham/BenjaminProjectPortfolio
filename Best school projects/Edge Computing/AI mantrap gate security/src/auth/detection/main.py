# main.py
import base64
import logging
import time
from typing import Dict

import cv2
import numpy as np

from auth.detection.detect import Detect
from auth.detection.streams import VideoStream
from auth.detection.tracker import Tracker
from auth.detection.utils import direction_config
from utils.logger_mixin import LoggerMixin


class IntruderDetector(LoggerMixin):
    def __init__(self, human_traker_config: Dict, logging_level=logging.INFO):
        self.camera_index = human_traker_config["camera_index"]
        self.model = human_traker_config["model"]
        self.confidence = human_traker_config["confidence"]
        self.iou_threshold = human_traker_config["iou_threshold"]
        self.directions = human_traker_config["directions"]
        self.duration = human_traker_config["duration"]
        self.sensitivity = human_traker_config["sensitivity"]
        self.show_video = human_traker_config.get("show_video", False)
        self.logger = self.setup_logger(__name__, logging_level)

    def _detect_person(self, detect: Detect, frame: np.ndarray) -> np.ndarray:
        # Run the detector.
        boxes, scores, class_idx = detect.detect(frame)
        idx = cv2.dnn.NMSBoxes(boxes, scores, self.confidence, self.iou_threshold)
        boxes = boxes[idx]
        scores = scores[idx]
        class_idx = class_idx[idx]
        # Filter for person (class index 0).
        person_idx = np.where(class_idx == 0)[0]
        boxes = boxes[person_idx]
        scores = scores[person_idx]
        H, W = frame.shape[:2]
        boxes = detect.to_xyxy(boxes) * np.array([W, H, W, H])
        dets = np.concatenate([boxes.astype(int), scores.reshape(-1, 1)], axis=1)
        return dets

    def run_tracking(self) -> bool:

        # Allow src to be an integer if possible.
        try:
            src_val = int(self.camera_index)
        except ValueError:
            src_val = self.camera_index

        # Setup tracking components.
        border = [(0, 500), (1920, 500)]
        directions = {key: direction_config.get(d_str) for key, d_str in self.directions.items()}
        tracker = Tracker(border, directions)
        detect = Detect(self.model, self.confidence)
        stream = VideoStream(src_val)

        # Variable to track detections.
        human_counts = []
        start_time = time.time()

        while time.time() - start_time < self.duration:
            is_finish, frame = stream.next()
            if not is_finish:
                break

            # Get detections from the current frame.
            detected_people = self._detect_person(detect, frame)
            human_count = detected_people.shape[0]
            human_counts.append(human_count)

            # Overlay the count on the frame.
            if self.show_video:
                cv2.putText(frame, f"Humans detected: {human_count}", (10, 30),
                            cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
                self.logger.debug(f"Detected {human_count} humans in this frame.")

                # Update tracker with detections.
                frame = tracker.update(frame, detected_people)

                # Display the live tracking.
                cv2.imshow("Live Tracking", frame)
                if cv2.waitKey(1) & 0xFF == ord('q'):
                    break

        stream.release()
        cv2.destroyAllWindows()

        # Compute the average human count.
        avg_count = sum(human_counts) / len(human_counts) if human_counts else 0
        self.logger.info(f"Done! Average human count: {avg_count}")
        return abs(avg_count - 1) <= self.sensitivity

    def capture_intruder(self):
        # Open the camera
        cap = cv2.VideoCapture(self.camera_index)
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
    intruder_detection_config = {
        "camera_index": 0,
        "model": "./models/yolov5/yolov5n-fp16.tflite",
        "confidence": 0.2,
        "iou_threshold": 0.2,
        "directions": {"total": None, "inside": "bottom", "outside": "top"},
        "duration": 30,
        "sensitivity": 0.05,
        "show_video": True,
    }

    human_tracker = IntruderDetector(intruder_detection_config)
    human_tracker.run_tracking()
