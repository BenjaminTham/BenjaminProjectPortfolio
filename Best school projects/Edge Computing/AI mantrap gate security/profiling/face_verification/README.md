# Face Verification

This repository contains various scripts and models for face verification using different methods. The primary focus is on leveraging TFLite models for efficient face verification, suitable for deployment on edge devices.

## Additional Models for Download
VGG16 Models: https://drive.google.com/drive/folders/1eMMwx83z2gOaQSYb-AWKj05oLTAsBKvx?usp=drive_link

## Folder Structure

```
face_verification/
├── archive/
│   ├── compare_vgg_tflite.py
│   ├── convert_to_tflite.py
│   ├── face_verification_old.py
│   ├── face_verification_only_tflite.py
│   └── face_verification.py
├── saved_faces/
├── debug_mobilefacenet.py
├── face_embeddings.pkl
├── face_profiling.py
├── face_verification_mobilefacenet.py
├── mobilefacenet_tf.pb
├── mobilefacenet.tflite
├── motion_detection.py
├── vgg16_feature_extractor.h5 (to download if profiling)
├── vgg16_feature_extractor.tflite (to download if profiling)
└── requirements.txt
```

## Files Description

### Archive

-   **compare_vgg_tflite.py**: Script to compare the performance and output similarity between VGG16 and TFLite models.
-   **convert_to_tflite.py**: Script to convert the VGG16 model to TFLite format and analyze tensor types before and after optimization.
-   **face_verification_old.py**: Old version of the face verification system using DeepFace.
-   **face_verification_only_tflite.py**: Face verification system using only TFLite model.
-   **face_verification.py**: Face verification system using VGG16 and TFLite models.

### Main Files

-   **debug_mobilefacenet.py**: Script to debug and get details on the TensorFlow and TFLite MobileFaceNet model.
-   **face_profiling.py**: Performance profiling tool for face detection and embedding generation pipelines.
-   **face_verification_mobilefacenet.py**: Main script for face verification using MobileFaceNet TFLite model and MediaPipe for face detection.
-   **motion_detection.py**: Script for motion detection using OpenCV.

### Models and Data

-   **mobilefacenet_tf.pb**: MobileFaceNet model in TensorFlow PB format.
-   **mobilefacenet.tflite**: MobileFaceNet model in TFLite format.
-   **face_embeddings.pkl**: Pickle file containing face embeddings.
-   **vgg16_feature_extractor.h5**: VGG16 model.
-   **vgg16_feature_extractor.tflite**: VGG16 TFLite quantized format.

### Directories

-   **saved_faces/**: Directory to store saved face images.

### Requirements

-   **requirements.txt**: List of required Python packages.

## Setup

To get the environment ready, install the required packages:

```
pip install -r requirements.txt
```

## Functionality

### 1. Face Verification with MobileFaceNet TFLite and MediaPipe

This section details how the face verification system works using the `face_verification_mobilefacenet.py` script. It uses the MobileFaceNet TFLite model for generating face embeddings and MediaPipe for face detection.

#### Configuration

The system can be configured through a dictionary containing various parameters:

```python
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
```

#### Enhanced Image Preprocessing

The system includes advanced image preprocessing techniques to improve face recognition accuracy:

1. **White Balance Correction**: Adjusts color balance using LAB color space
2. **Adaptive Gamma Correction**: Dynamically adjusts image brightness based on average luminance
3. **Contrast Normalization**: Enhances image contrast using LAB color space
4. **Model-Specific Normalization**: Applies final normalization based on model requirements

#### Workflow:

1. **Face Detection**: Utilizes MediaPipe's face detection model to locate faces with configurable confidence threshold
2. **Face Extraction**: Extracts the detected face with configurable padding and resizing
3. **Preprocessing**: Applies enhanced image preprocessing pipeline
4. **Embedding Generation**: Generates a 512-dimensional face embedding vector using MobileFaceNet
5. **Verification**: Compares embeddings using cosine similarity with configurable threshold

#### Key Functions:

- `detect_face()`: Face detection with MediaPipe
- `extract_face()`: Face extraction with padding
- `preprocess_face()`: Enhanced image preprocessing pipeline
- `get_face_embedding()`: Face embedding generation
- `verify_face()`: Face verification against database
- `wait_for_face_and_verify()`: Interactive verification with timeout and retry
- `build_embedding_from_folder()`: Build embeddings database from folder structure
- `build_embedding_from_images()`: Build embeddings from image list
- `capture_mismatch()`: Capture and encode mismatched faces

#### Embeddings Storage and Database Management

The system uses a flexible database structure for storing face embeddings:

```python
# Example embeddings database structure
{
    "person1_name": [
        embedding1,  # numpy array of shape (512,)
        embedding2,  # Multiple embeddings per person
        ...
    ],
    "person2_name": [
        embedding1,
        ...
    ]
}
```

Key features:
- **Multiple Embeddings**: Stores multiple embeddings per person for better recognition
- **Persistent Storage**: Saves embeddings to disk using pickle serialization
- **Database Management**:
  - `save_face_embeddings()`: Merges new embeddings with existing database
  - `load_face_embeddings()`: Loads embeddings from disk with error handling
  - `build_embedding_from_folder()`: Builds database from folder structure
  - `build_embedding_from_images()`: Builds database from image list

Folder structure for training data:
```
faces/
├── person1_name/
│   ├── image1.jpg
│   ├── image2.jpg
│   └── ...
├── person2_name/
│   ├── image1.jpg
│   └── ...
```

#### Usage:

To run the face verification script:

```
python face_verification_mobilefacenet.py
```

The script performs the following actions:

1.  Captures a face image using the webcam and saves it to the `saved_faces` folder with the filename "junxian.jpg".
2.  Builds face embeddings from the images in the `saved_faces` folder.
3.  Waits for a face to appear in the webcam feed and attempts to verify it against the generated embeddings.

#### Key Functions:

-   `detect_face()`: Detects faces in a frame using MediaPipe.
-   `extract_face()`: Extracts a face from a frame with padding.
-   `preprocess_face()`: Preprocesses the face image for the MobileFaceNet model.
-   `get_face_embedding()`: Generates the face embedding vector using the MobileFaceNet TFLite model.
-   `build_embeddings_from_folder()`: Builds a database of face embeddings from images in a folder.
-   `verify_face()`: Verifies a face against the embeddings database.
-   `wait_for_face_and_verify()`: Waits for a face to appear and then verifies it against the database.

### 2. Motion Detection

The `motion_detection.py` script implements motion detection using OpenCV.

#### Workflow:

1.  **Frame Capture**: Captures video frames from a specified camera.
2.  **Preprocessing**: Converts each frame to grayscale and applies a Gaussian blur to reduce noise.
3.  **Motion Detection**: Compares the current frame with the previous frame to identify differences. Thresholding is applied to highlight significant changes.
4.  **Contour Detection**: Identifies contours in the thresholded image, representing areas of motion.
5.  **Motion Indication**: If the area of any contour exceeds a predefined minimum area, motion is considered detected, and a message is printed.

#### Usage:

To run the motion detection script:

```
python motion_detection.py
```

This will start capturing video from the default camera and print a message to the console whenever motion is detected.

#### Key Parameters:

-   `camera_id`: ID of the camera to use.
-   `resolution`: Resolution of the captured video.
-   `threshold`: Threshold value for motion detection.
-   `min_area`: Minimum area of a contour to be considered motion.
-   `blur_size`: Size of the Gaussian blur kernel.
-   `check_interval`: Time interval between motion checks.

### 3. Profiling

The `face_profiling.py` script is designed to profile the performance of different face detection and embedding methods. It measures the time taken for each step in the face processing pipeline and collects system metrics such as CPU usage, memory usage, and I/O operations.

#### Workflow:

1.  **Initialization**: Initializes the FaceProfiler with specified models and configurations.
2.  **Image Loading**: Loads an image for processing.
3.  **Face Detection**: Detects faces in the image using either MediaPipe or OpenCV.
4.  **Face Extraction**: Extracts the detected face from the image.
5.  **Preprocessing**: Preprocesses the extracted face to match the input requirements of the chosen embedding model.
6.  **Embedding Generation**: Generates a face embedding vector using the specified model (MobileFaceNet, VGG16, etc.).
7.  **Metrics Collection**: Collects timing information for each step and system metrics such as CPU usage, memory usage, and I/O operations.
8.  **Reporting**: Prints a detailed report of the profiling results, including timing information and system metrics.

#### Usage:

To run the face profiling script:

```
python face_profiling.py
```

The script allows you to select different embedding methods and face detectors to compare their performance.

#### Key Components:

-   `FaceProfiler` class: Manages the face detection, extraction, preprocessing, and embedding generation processes.
-   `profile_pipeline()` function: Executes the face processing pipeline and collects timing and system metrics.
-   `print_profiling_results()` function: Prints a detailed report of the profiling results.

### 4. Profiling Results:
The following results were obtained when performing profiling on a Raspberry Pi 5.

#### 4.1. Model Loading
| Model              | Model Size (MB) | Memory Usage (MB) | CPU Usage (%) | Load Time (seconds) |
| ------------------ | --------------- | ----------------- | ------------- | ------------------- |
| MobileFaceNet (TFLite) | 5.0             | 17.5              | 98.9          | 0.01                |
| VGG (TFLite)       | 128.2           | 283.5             | 100.9         | 0.38                |
| MobileFaceNet (PB)   | 5.2             | 52.73             | 99.9          | 1.48                |
| VGG (Keras)        | 512.2           | 585.83            | 91.7          | 5.4                 |

#### 4.2. Pipeline
| Framework | Model | Detection (seconds) | Extraction (seconds) | Preprocessing (seconds) | Embedding (seconds) | Total Pipeline Time (seconds) | Function Calls | Primitive Calls |
|---|---|---|---|---|---|---|---|---|
| MediaPipe | MobileFaceNet (TFLite) | 0.0207 | 0 | 0.0038 | 0.0246 | 0.049 | 8003 | 7768 |
| MediaPipe | MobileFaceNet (PB) | 0.0213 | 0 | 0.0039 | 0.0357 | 0.0609 | 8604 | 8203 |
| OpenCV | MobileFaceNet (TFLite) | 0.0465 | 0.0001 | 0.0037 | 0.0246 | 0.0749 | 377 | 372 |
| OpenCV | MobileFaceNet (PB) | 0.0452 | 0.0001 | 0.0039 | 0.0353 | 0.0845 | 682 | 669 |
| MediaPipe | VGG (TFLite) | 0.0211 | 0 | 0.0094 | 0.2534 | 0.2839 | 8004 | 7769 |
| OpenCV | VGG (TFLite) | 0.0545 | 0.0001 | 0.0095 | 0.2516 | 0.3156 | 378 | 373 |
| MediaPipe | VGG (Keras) | 0.0227 | 0 | 0.0101 | 0.7241 | 0.7569 | 473804 | 454125 |
| OpenCV | VGG (Keras) | 0.0552 | 0.0001 | 0.0099 | 0.7708 | 0.836 | 71530 | 68144 |

Based on the profiling results:

*   **Model Loading:** VGG (Keras) has the largest model size and load time, while MobileFaceNet (TFLite) is the smallest and fastest to load.
*   **Pipeline Performance:** MobileFaceNet (TFLite) with MediaPipe offers the fastest total pipeline time. VGG (Keras) is significantly slower, especially in the embedding generation step.
*   **Framework Impact:** MediaPipe generally results in faster face detection compared to OpenCV.
*   **Model Format Matters:** TFLite models are generally faster than their TensorFlow Frozen Graph (PB) or Keras counterparts.
*   **Function Calls:** VGG (Keras) has a significantly higher number of function calls, indicating greater complexity or overhead.