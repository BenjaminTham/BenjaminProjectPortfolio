from steganography_base import SteganographyBase

class TextSteg(SteganographyBase):
    ZWSP = '\u200b'  # Zero Width Space
    ZWNJ = '\u200c'  # Zero Width Non-Joiner
    END_MARKER = '\u200d\u200d\u200d\u200d\u200d'  # Zero Width Joiner repeated five times as an end marker

    def encode_message(self, cover_file: str, message: str, bits_to_use: int = 1) -> bytes:
        with open(cover_file, 'r', encoding='utf-8') as file:
            cover_text = file.read()

        message_bits = self._string_to_bits(message)
        required_capacity = len(message_bits) + len(self._string_to_bits(self.END_MARKER))

        if self.calculate_capacity(cover_file, bits_to_use, message)[1] < required_capacity:
            raise ValueError("Cover file does not have enough capacity to hold the message.")

        stego_text = self._encode_data(cover_text, message_bits + self._string_to_bits(self.END_MARKER))

        return stego_text.encode('utf-8')

    def calculate_capacity(self, cover_file: str, bits_to_use: int, message: str = '') -> tuple:
        with open(cover_file, 'r', encoding='utf-8') as file:
            cover_text = file.read()
        capacity = len(cover_text) * bits_to_use

        return ('bits', capacity)

    def calculate_required_space(self, message: str, bits_to_use: int) -> tuple:
        message_bits = len(self._string_to_bits(message))
        return ('bits', message_bits)

    def decode_message(self, stego_file: str, bits_to_use: int = 1) -> str:
        with open(stego_file, 'r', encoding='utf-8') as file:
            stego_text = file.read()

        message_bits = self._decode_data(stego_text)

        return self._bits_to_string(message_bits)

    def _string_to_bits(self, message: str) -> str:
        return ''.join(format(ord(char), '08b') for char in message)

    def _bits_to_string(self, bits: str) -> str:
        chars = [bits[i:i+8] for i in range(0, len(bits), 8)]
        return ''.join(chr(int(char, 2)) for char in chars if len(char) == 8)

    def _encode_data(self, cover_text: str, message_bits: str) -> str:
        stego_text = list(cover_text)
        bit_index = 0

        for i in range(len(stego_text)):
            if bit_index >= len(message_bits):
                break

            if message_bits[bit_index] == '0':
                stego_text.insert(i, self.ZWSP)
            elif message_bits[bit_index] == '1':
                stego_text.insert(i, self.ZWNJ)

            bit_index += 1

        return ''.join(stego_text)

    def _decode_data(self, stego_text: str) -> str:
        message_bits = ''

        for char in stego_text:
            if char == self.ZWSP:
                message_bits += '0'
            elif char == self.ZWNJ:
                message_bits += '1'
            # Detect the end marker
            if message_bits.endswith(self._string_to_bits(self.END_MARKER)):
                message_bits = message_bits[:-len(self._string_to_bits(self.END_MARKER))]
                break

        return message_bits
