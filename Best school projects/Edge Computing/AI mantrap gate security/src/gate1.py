import logging

from gate.enum.gate_types import GateType
from gate.gate import Gate

mqtt_config = {
    "broker": "localhost", # TODO: CHANGE THIS
    "port": 1883,
    "username": "mosquitto",
    "password": "mosquitto",
}

update_config = {
    "url": "localhost:5000/api/biometrics/embeddings/2e048d59-cbfb-4444-a8b9-7d90430fa6ce/", # TODO: CHANGE THIS
    "save_path": "update",
}

motion_detector_config = {
    "camera_id": 0, # TODO: CHANGE THIS
    "resolution": (320, 240),
    "threshold": 25,
    "min_area": 500,
    "blur_size": 5,
    "check_interval": 0.5, # Check every 0.5s
    "fps": 10, # Use 10fps for lower CPU usage, but still responsive
}

face_verification_config = {
    "model_path": "model/mobilefacenet.tflite",
    "database_path": "update/face_embeddings.pkl",
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
    "camera_id": 0, # TODO: CHANGE THIS
}

intruder_detection_config = {
    "camera_index": 0, # TODO: CHANGE THIS
    "model": "model/yolov5n-fp16.tflite",
    "confidence": 0.2,
    "iou_threshold": 0.2,
    "directions": {"total": None, "inside": "bottom", "outside": "top"},
    "duration": 30,
    "sensitivity": 0.05
}

driver_config = {
    "buzzer": {
        "pin": 17
    },
    "lcd": {
        "address": 0x27,
        "port": 1,
        "cols": 16,
        "rows": 2,
        "dotsize": 8,
    },
    "servo": {
        "pin": 4,
        "open_angle": 180,
        "close_angle": 0,
    },
    "ultrasonic": {
        "echo": 23,
        "trigger": 18,
        "max_distance": 4,
        "window_size": 10,
        "calibration_step": 30,
    }
}

gate1 = Gate(
    gate_type=GateType.GATE1,
    mqtt_config=mqtt_config,
    update_config=update_config,
    voice_auth_config=None,  # No voice auth for gate1
    motion_detector_config=motion_detector_config,
    face_verification_config=face_verification_config,
    intruder_detection_config=intruder_detection_config,
    driver_config=driver_config,
    logging_level=logging.DEBUG,
)
gate1.run()
