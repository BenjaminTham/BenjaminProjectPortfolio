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

voice_auth_config = {
    "voiceprints_file": "update/voiceprints.pkl",
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

driver_config = {
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
        "echo": 17,
        "trigger": 27,
        "max_distance": 4,
        "window_size": 10,
        "calibration_step": 30,
    }
}

gate2 = Gate(
    gate_type=GateType.GATE2,
    mqtt_config=mqtt_config,
    update_config=update_config,
    voice_auth_config=voice_auth_config,
    motion_detector_config=None,  # No motion detection for gate2
    face_verification_config=face_verification_config,
    intruder_detection_config=None,
    driver_config=driver_config,
    logging_level=logging.DEBUG,
)
gate2.run()
