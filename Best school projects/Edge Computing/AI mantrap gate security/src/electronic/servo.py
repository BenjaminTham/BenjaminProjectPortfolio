import time

import RPi.GPIO as GPIO


class Servo:
    def __init__(self, pin=14, open_angle=180, close_angle=0):
        self.servo_pin = pin
        self.open_angle = open_angle
        self.close_angle = close_angle
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(self.servo_pin, GPIO.OUT)
        self.pwm = GPIO.PWM(self.servo_pin, 50)
        self.pwm.start(0)

    def _set_angle(self, angle):
        duty_cycle = 2.0 + (angle / 18.0)
        self.pwm.ChangeDutyCycle(duty_cycle)
        time.sleep(0.5)
        self.pwm.ChangeDutyCycle(0)

    def set_door_open(self):
        self._set_angle(self.open_angle)

    def set_door_close(self):
        self._set_angle(self.close_angle)

    def cleanup(self):
        self.pwm.stop()
        GPIO.cleanup()


# Main execution block
if __name__ == "__main__":
    servo = Servo()
    try:
        while True:
            user_input = input("Door state 'open' or 'close': ")
            if user_input == "open":
                servo.set_door_open()
            elif user_input == "close":
                servo.set_door_close()
    except KeyboardInterrupt:
        pass
    finally:
        servo.cleanup()
