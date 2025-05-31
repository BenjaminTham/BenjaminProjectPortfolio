import logging
import time
from datetime import datetime

import cv2

from utils.logger_mixin import LoggerMixin


class MotionDetector(LoggerMixin):
    """
    MotionDetector class is responsible for detecting motion from a camera feed.

    This class utilizes OpenCV to process video frames and identify motion based on
    configured parameters such as resolution, threshold, and minimum area for
    motion detection. It also provides methods to monitor motion in real-time and
    to wait for motion within a specified timeout.

    :ivar _camera_id: ID of the camera to use for video capture.
    :type _camera_id: int
    :ivar _resolution: Resolution of video frames as a tuple (width, height).
    :type _resolution: tuple[int, int]
    :ivar _threshold: Threshold value for identifying significant pixel differences.
    :type _threshold: int
    :ivar _min_area: Minimum area, in pixels, for contours to be considered motion.
    :type _min_area: int
    :ivar _blur_size: Kernel size used for Gaussian blur.
    :type _blur_size: int
    :ivar _check_interval: Time interval (in seconds) between motion checks.
    :type _check_interval: float
    :ivar _fps: Target frames per second for video processing.
    :type _fps: float
    :ivar logger: Logger instance for capturing logs specific to motion detection.
    :type logger: logging.Logger
    :ivar _camera: OpenCV VideoCapture object for capturing frames from the camera.
    :type _camera: cv2.VideoCapture or None
    :ivar _previous_frame: Stores the previous frame for motion comparison.
    :type _previous_frame: np.ndarray or None
    :ivar _running: Indicates if the monitoring process is currently active.
    :type _running: bool
    """

    def __init__(self, motion_detector_config, logging_level=logging.INFO):
        """
        Initializes the MotionDetector class with configuration and logging level.
        The class is designed to detect motion using input configurations and
        manages parameters such as resolution, threshold, and frame rate. This
        class provides a logger setup for tracking its processes.

        :param motion_detector_config:
            Dictionary containing configuration details for the motion detection
            system such as camera_id, resolution, threshold value, minimum
            detection area, blur size, frame interval, and target frames per second (fps).
        :param logging_level:
            Logging level to be used for setting up the logger. Defaults to
            `logging.INFO`.
        """
        self._camera_id = motion_detector_config["camera_id"]
        self._resolution = motion_detector_config["resolution"]
        self._threshold = motion_detector_config["threshold"]
        self._min_area = motion_detector_config["min_area"]
        self._blur_size = motion_detector_config["blur_size"]
        self._check_interval = motion_detector_config["check_interval"]
        self._fps = motion_detector_config["fps"]  # Target frames per second
        self.logger = self.setup_logger(__name__, logging_level)
        self._camera = None
        self._previous_frame = None
        self._running = False

    def _start(self):
        """
        Initializes and starts the camera for video capture with the specified settings.
        This function configures the camera resolution, captures the initial frame,
        and prepares it for further processing. If the camera cannot be opened or the
        initial frame cannot be captured, it raises an exception.

        :raises Exception: If the camera could not be opened using the provided
                           camera ID. Also, raises an exception if the initial frame
                           capture fails.
        :returns: The initialized camera object.
        """
        self._camera = cv2.VideoCapture(self._camera_id)
        if not self._camera.isOpened():
            raise Exception(f"Could not open camera {self._camera_id}")

        self._camera.set(cv2.CAP_PROP_FRAME_WIDTH, self._resolution[0])
        self._camera.set(cv2.CAP_PROP_FRAME_HEIGHT, self._resolution[1])

        ret, frame = self._camera.read()
        if not ret:
            raise Exception("Failed to capture initial frame")

        self._previous_frame = cv2.GaussianBlur(
            cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
            (self._blur_size, self._blur_size),
            0,
        )
        return self._camera

    def _check_for_motion(self, frame):
        """
        Analyzes a given video frame to detect motion based on visual changes.

        This method processes the input video frame by converting it to grayscale,
        applying Gaussian blurring, and computing the absolute difference against the
        previous frame. Thresholding and dilation steps are then performed to isolate
        regions of potential motion. Finally, contours are detected, and the areas of
        those contours are evaluated to determine if they meet the minimum area criteria
        to qualify as motion.

        If motion is detected, it returns True. Motion detection involves looking for
        changes in the regions between consecutive video frames.

        This method is a private utility function intended for internal use within
        a motion detection system.

        :param frame: The current video frame to be analyzed.
            This should be an image matrix in the format used by OpenCV.
        :return: A Boolean indicating whether motion was detected in the frame.
        """
        gray = cv2.GaussianBlur(
            cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
            (self._blur_size, self._blur_size),
            0,
        )

        thresh = cv2.dilate(
            cv2.threshold(
                cv2.absdiff(self._previous_frame, gray),
                self._threshold,
                255,
                cv2.THRESH_BINARY,
            )[1],
            None,
            iterations=2,
        )

        contours, _ = cv2.findContours(
            thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
        )
        self._previous_frame = gray

        return any(cv2.contourArea(contour) >= self._min_area for contour in contours)

    def start_monitoring(self):
        """
        Starts the process of monitoring a video feed for motion. The function captures
        frames from a connected camera and periodically checks for motion using an
        internal detection mechanism. If motion is detected, an informational log is recorded.
        The process handles interruptions gracefully and ensures resources are cleaned up properly.

        :raises KeyboardInterrupt: If the monitoring process is interrupted via a keyboard input.
        """
        if self._camera is None:
            self._start()

        self._running = True
        last_check = time.time()

        try:
            while self._running:
                ret, frame = self._camera.read()
                if not ret:
                    break

                current_time = time.time()
                if current_time - last_check >= self._check_interval:
                    if self._check_for_motion(frame):
                        self.logger.info(f"Motion detected at {datetime.now()}")
                    last_check = current_time

                time.sleep(0.01)

        except KeyboardInterrupt:
            self.logger.debug("Monitoring interrupted")
        finally:
            self._stop()

    def _stop(self):
        """
        Sets the running flag to False to stop execution and releases the camera
        resource if it is currently active, ensuring proper cleanup.

        :return: None
        """
        self._running = False
        if self._camera:
            self._camera.release()
            self._camera = None

    def wait_for_motion(self, timeout=None, warmup_time=1.0):
        """
        Waits for motion detection while allowing for a warm-up period and optional timeout.

        This function initializes motion detection, performs a warm-up period for the camera,
        and continuously checks for motion until motion is detected, the timeout is reached, or an
        interruption occurs. The function reads camera frames at a specified frame rate and applies
        image processing techniques to detect motion. It uses a sliding window approach to ensure
        motion detection is both efficient and reactive, while respecting supplied time constraints.

        :param timeout: The maximum duration (in seconds) to wait for motion to be detected. If set
            to None, waits indefinitely until motion is detected or interrupted.
        :type timeout: float or None
        :param warmup_time: The duration (in seconds) for warming up the camera before actively
            performing motion detection. This allows the camera to stabilize and avoid false positives.
        :type warmup_time: float
        :return: True if motion is detected within the timeout period, otherwise False.
        :rtype: bool
        """
        try:
            self.logger.debug(
                f"Motion detection initialized at {datetime.now()} with {self._fps} fps"
            )
            if self._camera is None:
                self._start()

            # Warm-up period: capture frames but don't detect motion
            self.logger.debug(f"Warming up camera for {warmup_time} seconds...")
            warmup_start = time.time()
            while time.time() - warmup_start < warmup_time:
                ret, frame = self._camera.read()
                if not ret:
                    self.logger.error("Failed to read frame during warm-up")
                    return False

                # Update previous_frame during warm-up to avoid false positives
                gray = cv2.GaussianBlur(
                    cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY),
                    (self._blur_size, self._blur_size),
                    0,
                )
                self._previous_frame = gray

                # Sleep to maintain the desired frame rate
                time.sleep(1.0 / self._fps)

            self.logger.debug(
                f"Warm-up complete. Motion detection actively started at {datetime.now()}"
            )

            start_time = time.time()
            last_check = start_time
            frame_time = start_time

            while True:
                current_time = time.time()

                # Limit frame capture rate to specified fps
                if current_time - frame_time >= 1.0 / self._fps:
                    ret, frame = self._camera.read()
                    if not ret:
                        self.logger.error("Failed to read frame from camera")
                        return False

                    frame_time = current_time

                    # Check for motion at the check_interval
                    if current_time - last_check >= self._check_interval:
                        if self._check_for_motion(frame):
                            motion_time = datetime.now()
                            self.logger.info(f"Motion detected at {motion_time}")
                            return True
                        last_check = current_time

                # Check for timeout
                if timeout and (current_time - start_time > timeout):
                    self.logger.debug(f"Timeout reached after {timeout} seconds")
                    return False

                # Small sleep to prevent CPU hogging
                time.sleep(0.01)

        except KeyboardInterrupt:
            self.logger.debug("Motion detection interrupted")
            self._stop()  # Ensure camera is released
            raise  # Re-raise to propagate the interrupt
        except Exception as e:
            self.logger.error(f"Error during motion detection: {e}")
            return False
        finally:
            self._stop()


if __name__ == "__main__":
    print("Waiting for motion...")
    motion_detector_config = {
        "camera_id": 0,
        "resolution": (320, 240),
        "threshold": 25,
        "min_area": 500,
        "blur_size": 5,
        "check_interval": 1.0,
        "fps": 1,  # Using 1 fps for low power consumption
    }
    detector = MotionDetector(motion_detector_config)
    if detector.wait_for_motion(warmup_time=1.0):
        print("Motion detected! Taking action...")
    else:
        print("Motion detection stopped or failed.")
