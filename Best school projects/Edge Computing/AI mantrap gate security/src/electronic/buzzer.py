from time import sleep

import RPi.GPIO as GPIO


class Buzzer:
    def __init__(self, pin=27):
        """Initialize the buzzer with a specific GPIO pin."""
        # Disable warnings (optional)
        GPIO.setwarnings(False)
        # Select GPIO mode
        GPIO.setmode(GPIO.BCM)
        # Set buzzer pin as output
        self.buzzer = pin
        GPIO.setup(self.buzzer, GPIO.OUT)

    def beep(self, duration=0.5):
        """Make the buzzer beep once for the specified duration."""
        GPIO.output(self.buzzer, GPIO.HIGH)
        print("Beep")
        sleep(duration)
        GPIO.output(self.buzzer, GPIO.LOW)
        print("No Beep")
        sleep(duration)

    def beep_sequence(self, count=3, duration=0.5):
        """Make the buzzer beep multiple times."""
        for _ in range(count):
            self.beep(duration)

    def start_continuous(self):
        """Turn the buzzer on continuously."""
        GPIO.output(self.buzzer, GPIO.HIGH)

    def stop(self):
        """Turn the buzzer off."""
        GPIO.output(self.buzzer, GPIO.LOW)
        print("Beep stopped")

    def cleanup(self):
        """Clean up GPIO resources."""
        GPIO.cleanup(self.buzzer)


if __name__ == "__main__":
    try:
        buzzer = Buzzer()
        print("Buzzer example: single beep")
        buzzer.beep()
        print("Buzzer example: 3 beeps")
        buzzer.beep_sequence(3)
        print("Buzzer example: continuous beep for 2 seconds")
        buzzer.start_continuous()
        sleep(2)
        buzzer.stop()
    except KeyboardInterrupt:
        print("Program stopped")
    finally:
        buzzer.cleanup()
