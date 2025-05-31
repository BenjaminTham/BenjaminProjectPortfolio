import collections
import statistics
import time

from gpiozero import DistanceSensor


class Ultrasonic:
    def __init__(self, echo_pin=17, trigger_pin=4, max_distance=4, window_size=10, calibration_samples=30):
        # Initialize the ultrasonic sensor
        self.sensor = DistanceSensor(echo=echo_pin, trigger=trigger_pin)
        self.sensor.max_distance = max_distance

        # Setup moving average filter
        self.window_size = window_size
        self.readings = collections.deque(maxlen=self.window_size)

        # Calibration settings
        self.calibration_samples = calibration_samples
        self.baseline = None
        self.noise_std = None

        # Dynamic threshold factor (adaptive detection)
        self.threshold_factor = 2  # Only consider deviations > 2x the noise level as significant

        # Calibrate sensor to determine baseline and noise
        self.calibrate()

    def moving_average(self, new_value):
        """Append new_value to the window and return the current moving average."""
        self.readings.append(new_value)
        return sum(self.readings) / len(self.readings)

    def calibrate(self):
        """Collect a set of samples to establish the sensor baseline and noise level."""
        print("Calibrating sensor baseline...")
        baseline_samples = []
        for _ in range(self.calibration_samples):
            reading = self.sensor.distance
            baseline_samples.append(reading)
            time.sleep(0.1)
        self.baseline = sum(baseline_samples) / len(baseline_samples)
        self.noise_std: float = statistics.stdev(baseline_samples) if len(baseline_samples) > 1 else 0.05
        print(f"Baseline distance: {self.baseline:.2f} m, Noise Std: {self.noise_std:.3f}")

    def wait_for_passage(self):
        """
        Monitor the sensor until an object passes through the gate.
        Returns True once the passage is detected.
        """
        print("Waiting for object to pass through the gate...")
        object_in_gate = False
        while True:
            current_reading = self.sensor.distance
            filtered_reading = self.moving_average(current_reading)
            deviation = self.baseline - filtered_reading

            # Detect object entering if deviation exceeds adaptive threshold.
            if not object_in_gate and deviation > self.threshold_factor * self.noise_std:
                print("Object entering detected.")
                object_in_gate = True

            # Detect object passage when deviation drops to near baseline.
            if object_in_gate and deviation < (self.threshold_factor * self.noise_std * 0.5):
                print("Object passed through the gate!")
                return True

            time.sleep(0.1)


if __name__ == "__main__":
    gate = Ultrasonic()
    if gate.wait_for_passage():
        print("Passage event detected and confirmed!")
