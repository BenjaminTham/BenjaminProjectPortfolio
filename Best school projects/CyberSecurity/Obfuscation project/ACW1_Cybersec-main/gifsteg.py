import os
from PIL import Image, ImageSequence
import numpy as np
import io

class GIFSteg:
    def encode_message(self, cover_file: str, message: str, bits_to_use: int) -> bytes:
        """
        Encodes a message into a cover file using steganography.

        Args:
            cover_file (str): The path to the cover file.
            message (str): The message to be encoded.
            bits_to_use (int): The number of least significant bits to use for encoding.

        Returns:
            bytes: The encoded bytes for the stego file.
        """
        return self.encrypt_text_in_gif(cover_file, message, bits_to_use)

    def encrypt_text_in_gif(self, filepath, msg, lsb_bits=1):
        '''
        Embed a message in a GIF file using the LSB method.
        Read each frame of the GIF, modify the LSBs of the pixels to encode the message,
        and save the resulting GIF to the 'Result_files' directory in the same location as the script.
        '''
        img = Image.open(filepath)  # Open the GIF file
        frames = []
        bit_idx = 0
        message_encoded = False

        msg += "<-END->"  # Add end marker to the message
        msg_bits = ''.join([format(ord(char), '08b') for char in msg])  # Convert message to binary string

        for frame_idx, frame in enumerate(ImageSequence.Iterator(img)):
            frame = frame.convert("P")
            palette = frame.getpalette()
            frame_data = np.array(frame, dtype=np.uint8)
            flat_frame_data_before = frame_data.flatten()  # Print the flat array before changes

            if frame_idx == 0 and not message_encoded:
                print("Flat Frame Data Before (First 50 elements):", flat_frame_data_before[:50].tolist())  # Print the first 50 elements of the flat array of the first frame before changes
                for i in range(len(flat_frame_data_before)):
                    for lsb in range(lsb_bits):
                        if bit_idx < len(msg_bits):
                            bit = int(msg_bits[bit_idx])
                            if bit == 1:
                                flat_frame_data_before[i] |= (1 << lsb)
                            else:
                                flat_frame_data_before[i] &= ~(1 << lsb)
                            bit_idx += 1
                            if bit_idx >= len(msg_bits):
                                message_encoded = True
                                break
                        else:
                            break
                    if message_encoded:
                        break

            new_frame = Image.fromarray(flat_frame_data_before.reshape(frame_data.shape), 'P')
            new_frame.putpalette(palette)
            frames.append(new_frame)

        # Print the first 50 elements of the flat array of the first frame after changes
        flat_frame_data_after = np.array(frames[0], dtype=np.uint8).flatten()
        print("Flat Frame Data After (First 50 elements):", flat_frame_data_after[:50].tolist())

        # Use an in-memory bytes buffer
        with io.BytesIO() as output_buffer:
            frames[0].save(output_buffer, save_all=True, append_images=frames[1:], format='GIF', optimize=False, loop=0)
            encoded_bytes = output_buffer.getvalue()
        
        return encoded_bytes


    def decode_message(self, stego_file: str, bits_to_use: int) -> str:
        """
        Decodes a message from a stego file using steganography.

        Args:
            stego_file (str): The path to the stego file.
            bits_to_use (int): The number of least significant bits used for encoding. Optional.

        Returns:
            str: The decoded message.
        """
        return self.decrypt_text_in_gif(stego_file, bits_to_use)

    def decrypt_text_in_gif(self, filepath, lsb_bits=1):
        '''
        Extract a hidden message from a GIF file using the LSB method.
        Open the GIF file, read the LSBs of the pixels in the first frame, and decode the message.
        '''
        try:
            im = Image.open(filepath)  # Open the GIF file
            frame = next(ImageSequence.Iterator(im))
            frame = frame.convert('P')
            frame_data = np.array(frame, dtype=np.uint8)
            flat_frame_data = frame_data.flatten()

            # Print the first 50 elements of the flattened frame data for debugging
            print("Flat Frame Data (First 50 elements):", flat_frame_data[:50].tolist())

            msg_bits = []
            bit_idx = 0
            decrypted_message = ""

            while bit_idx < len(flat_frame_data) * lsb_bits:
                for lsb in range(lsb_bits):
                    if (bit_idx // lsb_bits) < len(flat_frame_data):
                        current_pixel = flat_frame_data[bit_idx // lsb_bits]
                        bit = (current_pixel >> lsb) & 1
                        msg_bits.append(bit)
                    bit_idx += 1

                    # Check if we have enough bits to form a character
                    if len(msg_bits) % 8 == 0 and len(msg_bits) >= 8:
                        char_bits = msg_bits[-8:]
                        char = chr(int(''.join(map(str, char_bits)), 2))
                        decrypted_message += char
                        if decrypted_message.endswith("<-END->"):
                            print("END marker detected in decrypt_text_in_gif.")
                            return decrypted_message[:-7]

            print("END marker NOT detected in decrypt_text_in_gif.")
            return decrypted_message

        except Exception as e:
            print(f"Error in decrypt_text_in_gif: {str(e)}")
            return None  # Return None in case of errors

    def get_file_size_in_bytes(self, file_path):
        """Calculate file size in bytes for any file."""
        return os.path.getsize(file_path)

    def get_cover_capacity_in_bits(self, cover_path, lsb_bits):
        """
        Calculate the total capacity in bits of the first frame of an image or video,
        or the total capacity in an audio file.

        Args:
        cover_path (str): Path to the media file.
        lsb_bits (int): Number of least significant bits used for encoding data per channel.

        Returns:
        int: Total capacity in bits.
        """
        
        # Image capacity in bits
        file_extension = cover_path.lower().split('.')[-1]
        if file_extension in ['gif', 'png', 'jpg', 'jpeg', 'bmp']:
            with Image.open(cover_path) as img:
                total_pixels = img.width * img.height
                total_bits = total_pixels * lsb_bits * 3  # each pixel has 3 color channels (RGB)
                
            return total_bits
        else:
            raise ValueError("Unsupported file type for cover object")

    def calculate_capacity(self, cover_file: str, bits_to_use: int, message: str = "") -> tuple:
        """
        Calculates the capacity of the given cover file for steganography.

        Args:
            cover_file (str): The path to the cover file.
            bits_to_use (int): The number of bits to use for each pixel.
            message (str): The message to be hidden. Optional.

        Returns:
            tuple: A tuple containing the type of the capacity (e.g. "characters", "bytes", "bit", etc.) and the capacity value.
            e.g. ("byte", 1000)
        """
        try:
            capacity_in_bits = self.get_cover_capacity_in_bits(cover_file, bits_to_use)
            capacity_in_bytes = capacity_in_bits // 8
            return ("bytes", capacity_in_bytes)
        except ValueError as e:
            return ("error", str(e))

    def calculate_required_space(self, message: str, bits_to_use: int) -> tuple:
        """
        Calculates the required space to hide the given message in the steganography technique.

        Args:
            message (str): The message to be hidden.
            bits_to_use (int): The number of bits to use for each pixel.

        Returns:
            tuple: A tuple containing the type of the required space (e.g. "characters", "bytes", "bit", etc.) and the required space value.
            e.g. ("byte", 1000)
        """
        required_space_bits = len(message) * 8 * bits_to_use  # Assuming 1 byte per character in the message
        required_space_bytes = required_space_bits // 8
        return ("bytes", required_space_bytes)
