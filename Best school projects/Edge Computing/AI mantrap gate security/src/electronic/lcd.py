import time

from RPLCD.i2c import CharLCD


class LCD:
    def __init__(self, address=0x27, port=1, cols=16, rows=2, dotsize=8):
        self.lcd = CharLCD(i2c_expander='PCF8574', address=address, port=port, cols=cols, rows=rows, dotsize=dotsize)
        self.cols = cols
        self.rows = rows
        self.scrolling = False

    def write_wrapped(self, text):
        """
        Write text to the LCD with word wrapping.
        Splits the text at spaces so that words are not cut off and writes line-by-line.
        If the text is too long to fit into the available rows, a ValueError is raised.
        """
        self.lcd.clear()
        words = text.split()
        current_line = ""
        current_row = 0

        for word in words:
            # If the single word is longer than the display width, throw an error.
            if len(word) > self.cols:
                raise ValueError(f"The word '{word}' exceeds the display width of {self.cols} columns.")

            # Determine the space needed if adding the word (include a space if line isn't empty)
            needed = len(word) + (1 if current_line else 0)
            if len(current_line) + needed > self.cols:
                # If we have a next row available, write the current line and move to the next row.
                if current_row < self.rows - 1:
                    self.lcd.cursor_pos = (current_row, 0)
                    self.lcd.write_string(current_line.ljust(self.cols))
                    current_row += 1
                    current_line = word
                else:
                    # Last row reached and word doesn't fit => text too long
                    raise ValueError("Text exceeds display capacity in wrapped mode.")
            else:
                # Append the word to the current line.
                current_line += ((" " if current_line else "") + word)

        # Write the last line if there's room.
        if current_row < self.rows:
            self.lcd.cursor_pos = (current_row, 0)
            self.lcd.write_string(current_line.ljust(self.cols))
        else:
            raise ValueError("Text exceeds display capacity in wrapped mode.")

    def write_static(self, text, row=0):
        """
        Write text statically to a specified row.
        If the text length exceeds the number of columns, a ValueError is raised.
        """
        if len(text) > self.cols:
            raise ValueError(f"Text length ({len(text)}) exceeds display width ({self.cols}).")
        if row < 0 or row >= self.rows:
            raise ValueError(f"Row {row} is out of display range (0 to {self.rows - 1}).")
        self.lcd.cursor_pos = (row, 0)
        self.lcd.write_string(text.ljust(self.cols))

    def write_scrolling(self, text, row=1, delay=0.2):
        """
        Scroll a long text on the chosen row.
        The text will be padded and scrolled continuously.
        """
        padding = ' ' * self.cols
        s = padding + text + padding
        self.scrolling = True
        while self.scrolling:
            for i in range(len(s) - self.cols + 1):
                self.lcd.cursor_pos = (row, 0)
                self.lcd.write_string(s[i:i + self.cols])
                time.sleep(delay)

    def stop_and_clear(self):
        """Stops any scrolling text and clears the LCD screen."""
        self.scrolling = False
        self.lcd.clear()


# Example usage:
if __name__ == "__main__":
    lcd_display = LCD()

    # Write wrapped text. If the text is too long, it will throw a ValueError.
    try:
        lcd_display.write_wrapped("This is an example of ")
    except ValueError as e:
        print(e)

    time.sleep(5)

    # Write static text to the first row (row index 0)
    try:
        lcd_display.write_static("MIMI", row=0)
    except ValueError as e:
        print(e)

    time.sleep(5)

    try:
        # Scroll text on the second row (row index 1)
        long_text = "I love you, I love you, I love you, I love you"
        lcd_display.write_scrolling(long_text, row=1, delay=0.3)
    except KeyboardInterrupt:
        # Stop scrolling if Ctrl+C is pressed
        lcd_display.stop_and_clear()
