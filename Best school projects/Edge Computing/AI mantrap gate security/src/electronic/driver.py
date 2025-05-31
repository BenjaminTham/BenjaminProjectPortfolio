import logging

from electronic.buzzer import Buzzer
from electronic.lcd import LCD
from electronic.servo import Servo
from electronic.ultrasonic import Ultrasonic

from utils.logger_mixin import LoggerMixin


class Driver(LoggerMixin):
    def __init__(self, config: dict, logging_level=logging.INFO):
        buzzer_config: dict | None = config.get("buzzer", None)
        lcd_config: dict = config.get("lcd")
        servo_config: dict = config.get("servo")
        ultrasonic_config: dict = config.get("ultrasonic")

        if buzzer_config:
            self.buzzer = Buzzer(buzzer_config.get("pin"))
        else:
            self.buzzer = None
        self.lcd = LCD(
            lcd_config.get("address"),
            lcd_config.get("port"),
            lcd_config.get("cols"),
            lcd_config.get("rows"),
            lcd_config.get("dotsize")
        )
        self.servo = Servo(servo_config.get("pin"))
        self.ultrasonic = Ultrasonic(
            ultrasonic_config.get("echo"),
            ultrasonic_config.get("trigger"),
            ultrasonic_config.get("max_distance"),
            ultrasonic_config.get("window_size"),
            ultrasonic_config.get("calibration_step")
        )
        self.logger = self.setup_logger(__name__, logging_level)
        self.text = ""

    def open_gate(self):
        self.servo.set_door_open()
        self.lcd.write_wrapped("Gate opened.")
        self.logger.info("Gate opened.")

    def close_gate(self):
        self.servo.set_door_close()
        self.lcd.write_wrapped("Gate closed.")
        self.logger.info("Gate closed.")

    def personnel_passed(self):
        self.ultrasonic.wait_for_passage()
        self.logger.info("Personnel passed.")
        return True

    def display_text(self, text: str):
        if self.text != text:
            self.text = text
            self.lcd.write_wrapped(self.text)
            self.logger.debug(f"Text displayed: {self.text}")

    def alert(self):
        self.buzzer.start_continuous()

    def clear(self):
        if self.buzzer is not None:
            self.buzzer.stop()
        self.lcd.stop_and_clear()


if __name__ == "__main__":
    driver_config = {
        "buzzer": {
            "pin": 13
        },
        "lcd": {
            "address": 0x27,
            "port": 1,
            "cols": 16,
            "rows": 2,
            "dotsize": 8,
        },
        "servo": {
            "pin": 14,
            "open_angle": 180,
            "close_angle": 0,
        },
        "ultrasonic": {
            "echo": 12,
            "trigger": 18,
            "max_distance": 4,
            "window_size": 10,
            "calibration_step": 30,
        }
    }

    driver = Driver(driver_config)
