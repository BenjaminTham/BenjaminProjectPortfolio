from png_encoding import PngSteganography
from Wave_encoding import wave_steganography
from mp4_steg import MP4Steg
from gifsteg import GIFSteg
from ffv1Encoding import FFV1Steganography
from textsteg import TextSteg



def get_steganography_algorithm(file_path):
    if file_path.lower().endswith((".bmp", ".png")):
        return PngSteganography()
    elif file_path.lower().endswith(".gif"):
        # Return GIF steganography algorithm
        return GIFSteg()
    elif file_path.lower().endswith(".wav"):
        # Return WAV steganography algorithm
        # from wav_steganography import WavSteganography
        # return WavSteganography()
        return wave_steganography()
    elif file_path.lower().endswith(".mp4"):
        # Return MP4 steganography algorithm
        return MP4Steg()
    elif file_path.lower().endswith(".mkv"):
        return FFV1Steganography()
    elif file_path.lower().endswith(".txt"):
        return TextSteg()
    else:
        raise ValueError("Unsupported file type")


