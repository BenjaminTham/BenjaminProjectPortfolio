import cv2
import numpy as np
import time
from datetime import datetime

class MotionDetector:
    def __init__(self, camera_id=0, resolution=(320, 240), threshold=25, 
                 min_area=500, blur_size=5, check_interval=1.0, fps=1):
        self.camera_id = camera_id
        self.resolution = resolution
        self.threshold = threshold
        self.min_area = min_area
        self.blur_size = blur_size
        self.check_interval = check_interval
        self.fps = fps  # Target frames per second
        
        self.camera = None
        self.previous_frame = None
        self.motion_detected = False
        self.running = False
        
    def start(self):
        self.camera = cv2.VideoCapture(self.camera_id)
        if not self.camera.isOpened():
            raise Exception(f"Could not open camera {self.camera_id}")
        
        self.camera.set(cv2.CAP_PROP_FRAME_WIDTH, self.resolution[0])
        self.camera.set(cv2.CAP_PROP_FRAME_HEIGHT, self.resolution[1])
        
        ret, frame = self.camera.read()
        if not ret:
            raise Exception("Failed to capture initial frame")
            
        self.previous_frame = cv2.GaussianBlur(
            cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
            (self.blur_size, self.blur_size), 0
        )
        return self.camera
        
    def check_for_motion(self, frame):
        gray = cv2.GaussianBlur(
            cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
            (self.blur_size, self.blur_size), 0
        )
        
        thresh = cv2.dilate(
            cv2.threshold(
                cv2.absdiff(self.previous_frame, gray),
                self.threshold, 255, cv2.THRESH_BINARY
            )[1],
            None, iterations=2
        )
        
        contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        self.previous_frame = gray
        
        return any(cv2.contourArea(contour) >= self.min_area for contour in contours)
        
    def start_monitoring(self):
        if self.camera is None:
            self.start()
            
        self.running = True
        last_check = time.time()
        
        try:
            while self.running:
                ret, frame = self.camera.read()
                if not ret:
                    break
                
                current_time = time.time()
                if current_time - last_check >= self.check_interval:
                    if self.check_for_motion(frame):
                        print(f"Motion detected at {datetime.now()}")
                    last_check = current_time
                
                time.sleep(0.01)
                    
        except KeyboardInterrupt:
            print("Monitoring interrupted")
        finally:
            self.stop()
            
    def stop(self):
        self.running = False
        if self.camera:
            self.camera.release()
            self.camera = None


def wait_for_motion(camera_id=0, resolution=(320, 240), threshold=30, 
                    min_area=1000, blur_size=5, check_interval=1.0, 
                    timeout=None, warmup_time=1.0, fps=1):
    """
    Wait for motion to be detected and return True when detected.
    
    Args:
        camera_id: ID of the camera to use
        resolution: Resolution for the camera feed (width, height)
        threshold: Threshold for motion detection
        min_area: Minimum contour area to be considered as motion
        blur_size: Size of Gaussian blur kernel
        check_interval: How often to check for motion in seconds
        timeout: Maximum time to wait for motion (None for infinite)
        warmup_time: Time in seconds to let the camera stabilize before detecting motion
        fps: Frames per second to capture (lower values save power)
    
    Returns:
        True when motion is detected, False if timeout is reached
    """
    detector = MotionDetector(
        camera_id=camera_id,
        resolution=resolution,
        threshold=threshold,
        min_area=min_area,
        blur_size=blur_size,
        check_interval=check_interval,
        fps=fps
    )
    
    try:
        print(f"Motion detection initialized at {datetime.now()} with {fps} fps")
        detector.start()
        
        # Warm-up period: capture frames but don't detect motion
        print(f"Warming up camera for {warmup_time} seconds...")
        warmup_start = time.time()
        while time.time() - warmup_start < warmup_time:
            ret, frame = detector.camera.read()
            if not ret:
                print("Failed to read frame during warm-up")
                return False
                
            # Update previous_frame during warm-up to avoid false positives
            gray = cv2.GaussianBlur(
                cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
                (detector.blur_size, detector.blur_size), 0
            )
            detector.previous_frame = gray
            
            # Sleep to maintain the desired frame rate
            time.sleep(1.0 / detector.fps)
            
        print(f"Warm-up complete. Motion detection actively started at {datetime.now()}")
        
        start_time = time.time()
        last_check = start_time
        frame_time = start_time
        
        while True:
            current_time = time.time()
            
            # Limit frame capture rate to specified fps
            if current_time - frame_time >= 1.0 / detector.fps:
                ret, frame = detector.camera.read()
                if not ret:
                    print("Failed to read frame from camera")
                    return False
                
                frame_time = current_time
                
                # Check for motion at the check_interval
                if current_time - last_check >= check_interval:
                    if detector.check_for_motion(frame):
                        motion_time = datetime.now()
                        print(f"Motion detected at {motion_time}")
                        return True
                    last_check = current_time
            
            # Check for timeout
            if timeout and (current_time - start_time > timeout):
                print(f"Timeout reached after {timeout} seconds")
                return False
                
            # Small sleep to prevent CPU hogging
            time.sleep(0.01)
            
    except KeyboardInterrupt:
        print("Motion detection interrupted")
        return False
    except Exception as e:
        print(f"Error during motion detection: {e}")
        return False
    finally:
        detector.stop()

# Example usage
def simple_example():
    print("Waiting for motion...")
    # Using 1 fps for low power consumption
    if wait_for_motion(warmup_time=1.0, fps=1):
        print("Motion detected! Taking action...")
    else:
        print("Motion detection stopped or failed.")

# Uncomment to run the example
simple_example()