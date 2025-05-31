from PIL import Image
from typing import Tuple, Optional
from steganography_base import SteganographyBase

class PngSteganography(SteganographyBase):
    def __init__(self):
        pass

    def calculate_capacity(self, cover_file_path: str, bits_to_use: int, message: str) -> Tuple[str, int]:
        img = Image.open(cover_file_path)
        width, height = img.size
        capacity = width * height * 3 * bits_to_use  # 3 channels (RGB)
        return ("bits", capacity)

    def calculate_required_space(self, message: str, bits_to_use: int) -> Tuple[str, int]:
        payload_size = len(message.encode('utf-8'))
        required_space = payload_size * 8  # bits_to_use
        return ("bits", required_space)

    def encode_message(self, cover_file_path: str, message: str, bits_to_use: int) -> Image.Image:
        img = Image.open(cover_file_path)

        # Convert image to RGB if it's not already in a compatible mode
        if img.mode != 'RGB':
            img = img.convert('RGB')

        encoded_img = img.copy()
        width, height = img.size

        # Convert the payload to binary
        binary_payload = ''.join(format(ord(char), '08b') for char in message)
        binary_payload += '00000000'  # Adding a termination null character for safety

        # Add the length of the payload at the beginning
        payload_length = len(binary_payload)
        length_bin = format(payload_length, '032b')  # 32-bit integer for the length
        binary_payload = length_bin + binary_payload

        payload_index = 0
        payload_length = len(binary_payload)

        for row in range(height):
            for col in range(width):
                if payload_index < payload_length:
                    pixel = list(encoded_img.getpixel((col, row)))

                    for i in range(len(pixel)):
                        if payload_index < payload_length:
                            # Replace the LSB bits_to_use with the message bits
                            pixel_bin = format(pixel[i], '08b')
                            new_pixel_bin = pixel_bin[:-bits_to_use] + binary_payload[payload_index:payload_index + bits_to_use]
                            pixel[i] = int(new_pixel_bin, 2)
                            payload_index += bits_to_use

                    encoded_img.putpixel((col, row), tuple(pixel))
                else:
                    break
            if payload_index >= payload_length:
                break

        return encoded_img

    def decode_message(self, stego_file_path: str, bits_to_use: int) -> Optional[str]:
        try:
            img = Image.open(stego_file_path)

            # Convert image to RGB if it's not already in a compatible mode
            if img.mode != 'RGB':
                img = img.convert('RGB')

            width, height = img.size

            binary_payload = []
            for row in range(height):
                for col in range(width):
                    pixel = list(img.getpixel((col, row)))

                    for i in range(len(pixel)):
                        pixel_bin = format(pixel[i], '08b')
                        binary_payload.append(pixel_bin[-bits_to_use:])

            # Join the list to create the full binary string
            binary_payload = ''.join(binary_payload)

            # Extract the message length (first 32 bits)
            length_bin = binary_payload[:32]
            message_length = int(length_bin, 2)

            # Extract the message binary
            binary_payload = binary_payload[32:32 + message_length]

            # Print for debugging
            print(f"Binary message: {binary_payload[:64]}...")  # Print first 64 bits for brevity

            # Extract the binary string in chunks of 8 bits
            binary_payload = [binary_payload[i:i + 8] for i in range(0, len(binary_payload), 8)]

            # Convert binary chunks to characters
            decoded_message = ''.join([chr(int(char, 2)) for char in binary_payload])

            # Ensure we remove any trailing null characters
            decoded_message = decoded_message.rstrip('\x00')

            return decoded_message

        except Exception as e:
            print(f"Error during decoding: {e}")
            return None
