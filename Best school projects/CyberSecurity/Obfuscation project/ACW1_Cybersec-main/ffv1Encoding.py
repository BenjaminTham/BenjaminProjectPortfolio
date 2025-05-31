import os
import shutil
import cv2
import numpy as np
import subprocess


class FFV1Steganography:
    def __init__(self):
        self.here = os.path.dirname(os.path.abspath(__file__))
        self.frames_directory = os.path.join(self.here, "ffv1data")
        if not os.path.exists(self.frames_directory):
            os.makedirs(self.frames_directory)

    def read_hidden_text(self, filename):
        file_path_txt = os.path.join(self.here, filename)
        with open(file_path_txt, "r") as f:
            hidden_text_content = f.read()
        return hidden_text_content

    def calculate_length_of_hidden_text(self, filename):
        hidden_text_content = self.read_hidden_text(filename)
        return len("".join(format(byte, "08b") for byte in hidden_text_content))

    def convert_video(self, input_file, ffv1_video):
        output_file = os.path.join(self.here, ffv1_video)
        if not output_file.endswith(".mkv"):
            output_file += ".mkv"

        command = [
            "ffmpeg",
            "-y",
            "-i",
            input_file,
            "-t",
            "12",
            "-c:v",
            "ffv1",
            "-level",
            "3",
            "-coder",
            "1",
            "-context",
            "1",
            "-g",
            "1",
            "-slices",
            "4",
            "-slicecrc",
            "1",
            "-c:a",
            "copy",
            output_file,
        ]

        try:
            subprocess.run(command, check=True)
            print(f"Conversion successful: {output_file}")
            return output_file
        except subprocess.CalledProcessError as e:
            print(f"Error during conversion: {e}")

    def extract_audio(self, ffv1_video, audio_path):
        if not audio_path.endswith(".aac"):
            audio_path += ".aac"

        extracted_audio = os.path.join(self.here, audio_path)
        if not ffv1_video.endswith(".mkv"):
            ffv1_video += ".mkv"

        command = [
            "ffmpeg",
            "-y",
            "-i",
            ffv1_video,
            "-q:a",
            "0",
            "-map",
            "a",
            extracted_audio,
        ]
        try:
            subprocess.run(
                command, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE
            )
            print(f"Audio successfully extracted to {extracted_audio}")
        except subprocess.CalledProcessError as e:
            print(f"An error occurred: {e}")

    def split_into_frames(self, ffv1_video):
        if not ffv1_video.endswith(".mkv"):
            ffv1_video += ".mkv"

        ffv1_video = cv2.VideoCapture(os.path.join(self.here, ffv1_video))
        currentframe = 0

        while True:
            ret, frame = ffv1_video.read()
            if ret:
                name = os.path.join(self.frames_directory, f"frame{currentframe}.png")
                print("Creating..." + name)
                cv2.imwrite(name, frame)
                currentframe += 1
            else:
                print("Complete")
                break

    def stitch_frames_to_video(self, ffv1_video, framerate=60):
        if not ffv1_video.endswith(".mkv"):
            ffv1_video += ".mkv"

        output_video_path = os.path.join(self.here, ffv1_video)
        command = [
            "ffmpeg",
            "-y",
            "-framerate",
            str(framerate),
            "-i",
            os.path.join(self.frames_directory, "frame%d.png"),
            "-c:v",
            "ffv1",
            "-level",
            "3",
            "-coder",
            "1",
            "-context",
            "1",
            "-g",
            "1",
            "-slices",
            "4",
            "-slicecrc",
            "1",
            output_video_path,
        ]

        try:
            subprocess.run(command, check=True)
            print(f"Video successfully created at {output_video_path}")
        except subprocess.CalledProcessError as e:
            print(f"An error occurred: {e}")

    def add_audio_to_video(self, encoded_video, audio_path, final_video):
        if not encoded_video.endswith(".mkv"):
            encoded_video += ".mkv"
        if not final_video.endswith(".mkv"):
            final_video += ".mkv"
        if not audio_path.endswith(".aac"):
            audio_path += ".aac"

        final_output_path = os.path.join(self.here, final_video)
        command = [
            "ffmpeg",
            "-y",
            "-i",
            os.path.join(self.here, encoded_video),
            "-i",
            os.path.join(self.here, audio_path),
            "-c:v",
            "copy",
            "-c:a",
            "aac",
            "-strict",
            "experimental",
            final_output_path,
        ]
        try:
            subprocess.run(command, check=True)
            print(f"Final video with audio created at {final_output_path}")
        except subprocess.CalledProcessError as e:
            print(f"An error occurred: {e}")

    def encode(self, image_name, secret_data, number_of_bits):
        image = cv2.imread(image_name)
        n_bytes = image.shape[0] * image.shape[1] * 3 * number_of_bits // 8
        print("[*] Maximum bytes to encode:", n_bytes)
        secret_data += "====="
        if len(secret_data) * 8 > n_bytes:
            raise ValueError("[!] Insufficient bytes, need bigger image or less data")
        print("[*] Encoding Data")

        data_index = 0
        binary_secret_data = self.to_bin(secret_data)
        data_len = len(binary_secret_data)

        binary_secret_data += "0" * (
            (number_of_bits - len(binary_secret_data) % number_of_bits) % number_of_bits
        )

        for row in image:
            for pixel in row:
                r, g, b = self.to_bin(pixel)
                if data_index < data_len:
                    new_r = (
                        r[:-number_of_bits]
                        + binary_secret_data[data_index : data_index + number_of_bits]
                    )
                    pixel[0] = int(new_r, 2)
                    data_index += number_of_bits
                if data_index < data_len:
                    new_g = (
                        g[:-number_of_bits]
                        + binary_secret_data[data_index : data_index + number_of_bits]
                    )
                    pixel[1] = int(new_g, 2)
                    data_index += number_of_bits
                if data_index < data_len:
                    new_b = (
                        b[:-number_of_bits]
                        + binary_secret_data[data_index : data_index + number_of_bits]
                    )
                    pixel[2] = int(new_b, 2)
                    data_index += number_of_bits
                if data_index >= data_len:
                    break
            if data_index >= data_len:
                break

        cv2.imwrite(image_name, image)
        return image

    def decode(self, image_name, number_of_bits):
        image = cv2.imread(image_name)
        binary_data = ""
        for row in image:
            for pixel in row:
                r, g, b = self.to_bin(pixel)
                binary_data += r[-number_of_bits:]
                binary_data += g[-number_of_bits:]
                binary_data += b[-number_of_bits:]
        all_bytes = [binary_data[i : i + 8] for i in range(0, len(binary_data), 8)]
        decoded_data = ""
        for byte in all_bytes:
            decoded_data += chr(int(byte, 2))
            if decoded_data[-5:] == "=====":
                break
        return decoded_data[:-5]

    def cleanup(self, files_to_delete):
        # Delete specified files
        for file in files_to_delete:
            file_path = os.path.join(self.here, file)
            if os.path.exists(file_path):
                os.remove(file_path)
                print(f"Deleted file: {file_path}")
            else:
                print(f"File not found: {file_path}")

        # Delete the frames directory and its contents
        if os.path.exists(self.frames_directory):
            shutil.rmtree(self.frames_directory)
            print(f"Deleted directory and its contents: {self.frames_directory}")
        else:
            print(f"Directory not found: {self.frames_directory}")

    def to_bin(self, data):
        # this function allows all types to be converted into binary 1's and 0's
        if isinstance(data, str):
            return "".join([format(ord(i), "08b") for i in data])
        elif isinstance(data, bytes) or isinstance(data, np.ndarray):
            return [format(i, "08b") for i in data]
        elif isinstance(data, int) or isinstance(data, np.uint8):
            return format(data, "08b")
        else:
            raise TypeError("Type not supported")

    def calculate_capacity(self, cover_file, bits_to_use, message):
        # Ensure the frames are extracted
        self.split_into_frames(cover_file)

        frame0_path = os.path.join(self.frames_directory, "frame0.png")
        if not os.path.exists(frame0_path):
            raise FileNotFoundError(f"{frame0_path} not found.")

        # Read the frame0.png image
        image = cv2.imread(frame0_path)
        if image is None:
            raise ValueError(f"Failed to read {frame0_path}")

        # Get image dimensions
        height, width, channels = image.shape

        # Calculate the total number of bits available for encoding
        total_pixels = height * width
        total_bits_available = total_pixels * channels * bits_to_use

        # Convert the total bits to bytes
        total_bytes_available = total_bits_available // 8

        return ("bytes", total_bytes_available)

    def calculate_required_space(self, message: str, bits_to_use: int) -> tuple:
        required_space_bits = (
            len(message) * 8 * bits_to_use
        )  # Assuming 1 byte per character in the message
        required_space_bytes = required_space_bits // 8
        return ("bytes", required_space_bytes)

    def encode_message(self, cover_file, message, number_of_bits):
        hidden_text = message
        ffv1_video = cover_file
        extracted_audio = "audio"
        encoded_video = "encoded"
        final_video = "result.mkv"

        converted_video_file = ffv1_video
        if converted_video_file and os.path.exists(converted_video_file):
            self.extract_audio(converted_video_file, extracted_audio)
        else:
            raise FileNotFoundError(
                f"Conversion failed: {converted_video_file} not found."
            )

        self.split_into_frames(ffv1_video)
        frame0_path = os.path.join(self.here, "ffv1data", "frame0.png")
        encoded_image = self.encode(frame0_path, hidden_text, number_of_bits)
        cv2.imwrite(frame0_path, encoded_image)
        self.stitch_frames_to_video(encoded_video)
        self.add_audio_to_video(encoded_video, extracted_audio, final_video)

        with open(os.path.join(self.here, final_video), "rb") as f:
            byte_data = f.read()

        files_to_delete = [
            extracted_audio + ".aac",
            encoded_video + ".mkv",
            ffv1_video + ".mkv",
        ]
        self.cleanup(files_to_delete)

        return byte_data

    def decode_message(self, stego_file, number_of_bits):
        final_video = stego_file
        self.split_into_frames(final_video)
        decode_image_path = os.path.join(self.frames_directory, "frame0.png")
        decoded_message = self.decode(decode_image_path, number_of_bits)
        files_to_delete = [
            "audio.aac",
            "encoded.mkv",
            "output.mkv",
        ]
        self.cleanup(files_to_delete)
        return decoded_message


# if __name__ == "__main__":
#     stego = FFV1Steganography()

#     # Original video (mp4,mkv,avi)
#     original_video = "video"
#     # Converted ffv1 video
#     ffv1_video = "output"
#     # Extracted audio
#     extracted_audio = "audio"
#     # Encoded video without sound
#     encoded_video = "encoded"
#     # Final result video, encoded, with sound
#     final_video = "result"

#     number_of_bits = 6

#     # region --hidden text processing --
#     hidden_text_path = os.path.join(stego.here, "hiddentext.txt")
#     with open(hidden_text_path, "r", encoding="utf-8") as f:
#         hidden_text = f.read()
#     # endregion

#     # region -- raw video locating --
#     raw_video_file = stego.find_raw_video_file(original_video)
#     if raw_video_file:
#         print(f"Found video file: {raw_video_file}")
#     else:
#         print("video.mp4 not found.")
#     # endregion

#     # region -- video processing INPUT--
#     converted_video_file = stego.convert_video(raw_video_file, ffv1_video)

#     if converted_video_file and os.path.exists(converted_video_file):
#         stego.extract_audio(converted_video_file, extracted_audio)
#     else:
#         print(f"Conversion failed: {converted_video_file} not found.")

#     stego.split_into_frames(ffv1_video)
#     # endregion

#     # region -- encode --
#     # Encode the message
#     frame0_path = os.path.join(stego.here, "ffv1data", "frame0.png")
#     encoded_image = stego.encode(frame0_path, hidden_text, number_of_bits)
#     # Save the encoded image
#     cv2.imwrite(frame0_path, encoded_image)
#     # endregion

#     # region -- video processing RESULT --
#     stego.stitch_frames_to_video(encoded_video)
#     stego.add_audio_to_video(encoded_video, extracted_audio, final_video)
#     # endregion

#     # region -- cleanup --
#     files_to_delete = [
#         extracted_audio + ".aac",
#         encoded_video + ".mkv",
#         ffv1_video + ".mkv",
#     ]

#     stego.cleanup(files_to_delete)
#     # endregion

#     # region --decode--
#     stego.split_into_frames(final_video)
#     decode_image_path = os.path.join(stego.here, "ffv1data", "frame0.png")
#     decoded_message = stego.decode(decode_image_path, number_of_bits)
#     print(decoded_message)
#     # endregion

#     # region -- cleanup --
#     files_to_delete = [
#         extracted_audio + ".aac",
#         encoded_video + ".mkv",
#         ffv1_video + ".mkv",
#     ]

#     stego.cleanup(files_to_delete)
#     # endregion
