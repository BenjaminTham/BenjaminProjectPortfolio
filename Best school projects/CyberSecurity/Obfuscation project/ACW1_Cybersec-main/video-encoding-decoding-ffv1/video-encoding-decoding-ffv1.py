import json
import os
import shutil
import magic
import ffmpeg
import cv2
import numpy as np
import subprocess
from PIL import Image
import glob


import json
import os
import shutil
import magic
import ffmpeg
import cv2
import numpy as np
import subprocess
from PIL import Image
import glob


class FFV1Steganography:
    def __init__(self):
        self.here = os.path.dirname(os.path.abspath(__file__))

        # Create a folder to save the frames
        self.frames_directory = os.path.join(self.here, "data")
        try:
            if not os.path.exists(self.frames_directory):
                os.makedirs(self.frames_directory)
        except OSError:
            print("Error: Creating directory of data")

    def read_hidden_text(self, filename):
        file_path_txt = os.path.join(self.here, filename)
        # Read the content of the file in binary mode
        with open(file_path_txt, "r") as f:
            hidden_text_content = f.read()
        return hidden_text_content

    def calculate_length_of_hidden_text(self, filename):
        hidden_text_content = self.read_hidden_text(filename)
        # Convert each byte to its binary representation and join them
        return len("".join(format(byte, "08b") for byte in hidden_text_content))

    def find_raw_video_file(self, filename):
        file_extensions = [".mp4", ".mkv", ".avi"]
        for ext in file_extensions:
            file_path = os.path.join(self.here, filename + ext)
            if os.path.isfile(file_path):
                return file_path
        return None

    def convert_video(self, input_file, ffv1_video):
        # this function is the same as running this command line
        # ffmpeg -i video.mp4 -t 12 -c:v ffv1 -level 3 -coder 1 -context 1 -g 1 -slices 4 -slicecrc 1 -c:a copy output.mkv

        # in order to run any ffmpeg subprocess, you have to have ffmpeg installed into the computer.
        # https://ffmpeg.org/download.html

        # WARNING:
        # the ffmpeg you should download is not the same as the ffmpeg library for python.
        # you need to download the exe from the link above, then add ffmpeg bin directory to system variables
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
        # Ensure the audio output file has the correct extension
        if not audio_path.endswith(".aac"):
            audio_path += ".aac"

        # Full path to the extracted audio file
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
            result = subprocess.run(
                command, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE
            )
            print(f"Audio successfully extracted to {extracted_audio}")
            print(result.stdout.decode())
            print(result.stderr.decode())
        except subprocess.CalledProcessError as e:
            print(f"An error occurred: {e}")
            print(e.stdout.decode())
            print(e.stderr.decode())

    def split_into_frames(self, ffv1_video):
        # split_into_frames function uses cv2 to cut the video into individual frames and save them in a directory.
        # in this case, the directory is data1
        # each frame HAS to be saved as png because png is lossless image codec.
        # this is to prevent lsb mutation when encoding the secret hidden text into the frames.
        if not ffv1_video.endswith(".mkv"):
            ffv1_video += ".mkv"

        ffv1_video = cv2.VideoCapture(
            os.path.join(self.here, ffv1_video)
        )  # Reinitialize raw_video with the correct file path

        currentframe = 0

        while True:
            ret, frame = ffv1_video.read()
            if ret:
                name = os.path.join(self.here, "data", f"frame{currentframe}.png")
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

    # def concatenate_videos(self, video1_path, video2_path, output_path):
    #     if not video1_path.endswith(".mkv"):
    #         video1_path += ".mkv"
    #     if not video2_path.endswith(".mkv"):
    #         video2_path += ".mkv"
    #     if not output_path.endswith(".mkv"):
    #         output_path += ".mkv"

    #     video1_path = os.path.join(self.here, video1_path)
    #     video2_path = os.path.join(self.here, video2_path)
    #     output_video_path = os.path.join(self.here, output_path)

    #     command = [
    #         "ffmpeg",
    #         "-y",
    #         "-i",
    #         video1_path,
    #         "-i",
    #         video2_path,
    #         "-filter_complex",
    #         "[0:v] [0:a] [1:v] [1:a] concat=n=2:v=1:a=1 [v] [a]",
    #         "-map",
    #         "[v]",
    #         "-map",
    #         "[a]",
    #         output_video_path,
    #     ]

    #     try:
    #         subprocess.run(command, check=True)
    #         print(f"Videos successfully concatenated into {output_video_path}")
    #     except subprocess.CalledProcessError as e:
    #         print(f"An error occurred: {e}")

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

        print("Before encoding:")
        print(image[0][0])  # Print the first pixel values before encoding

        for row in image:
            for pixel in row:
                r, g, b = self.to_bin(pixel)
                original_pixel = pixel.copy()  # Save the original pixel for debugging

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

                print(
                    f"Original RGB: {original_pixel}, Binary: {r, g, b}"
                )  # Debug original values
                print(
                    f"Modified RGB: {pixel}, Binary: {new_r, new_g, new_b}"
                )  # Debug modified values

                if data_index >= data_len:
                    break
            if data_index >= data_len:
                break

        print("After encoding:")
        print(image[0][0])  # Print the first pixel values after encoding

        cv2.imwrite(image_name, image)
        return image

    def decode(self, image_name, number_of_bits):
        print("[+] Decoding")
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


if __name__ == "__main__":
    stego = FFV1Steganography()

    # Original video (mp4,mkv,avi)
    original_video = "video"
    # Converted ffv1 video
    ffv1_video = "output"
    # Extracted audio
    extracted_audio = "audio"
    # Encoded video without sound
    encoded_video = "encoded"
    # Final result video, encoded, with sound
    final_video = "result"

    number_of_bits = 6

    # region --hidden text processing --
    hidden_text_path = os.path.join(stego.here, "hiddentext.txt")
    with open(hidden_text_path, "r", encoding="utf-8") as f:
        hidden_text = f.read()
    # endregion

    # region -- raw video locating --
    raw_video_file = stego.find_raw_video_file(original_video)
    if raw_video_file:
        print(f"Found video file: {raw_video_file}")
    else:
        print("video.mp4 not found.")
    # endregion

    # region -- video processing INPUT--
    converted_video_file = stego.convert_video(raw_video_file, ffv1_video)

    if converted_video_file and os.path.exists(converted_video_file):
        stego.extract_audio(converted_video_file, extracted_audio)
    else:
        print(f"Conversion failed: {converted_video_file} not found.")

    stego.split_into_frames(ffv1_video)
    # endregion

    # region -- encode --
    # Encode the message
    frame0_path = os.path.join(stego.here, "data", "frame0.png")
    encoded_image = stego.encode(frame0_path, hidden_text, number_of_bits)
    # Save the encoded image
    cv2.imwrite(frame0_path, encoded_image)
    # endregion

    # region -- video processing RESULT --
    stego.stitch_frames_to_video(encoded_video)
    stego.add_audio_to_video(encoded_video, extracted_audio, final_video)
    # endregion

    # region -- cleanup --
    files_to_delete = [
        extracted_audio + ".aac",
        encoded_video + ".mkv",
        ffv1_video + ".mkv",
    ]

    stego.cleanup(files_to_delete)
    # endregion

    # # region --decode--
    # stego.split_into_frames(final_video)
    # decode_image_path = os.path.join(stego.here, "data", "frame0.png")
    # decoded_message = stego.decode(decode_image_path, number_of_bits)
    # print(decoded_message)
    # # endregion

    # # region -- cleanup --
    # files_to_delete = [
    #     extracted_audio + ".aac",
    #     encoded_video + ".mkv",
    #     ffv1_video + ".mkv",
    # ]

    # stego.cleanup(files_to_delete)
    # # endregion
