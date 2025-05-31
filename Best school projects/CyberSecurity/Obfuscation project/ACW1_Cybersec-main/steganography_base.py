from abc import ABC, abstractmethod

class SteganographyBase(ABC):
    @abstractmethod
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
            pass
        
    
    @abstractmethod
    def calculate_capacity(self, cover_file: str, bits_to_use: int, message: str) -> tuple:
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
            pass

    @abstractmethod
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
        pass

    @abstractmethod
    def decode_message(self, stego_file: str, bits_to_use: int) -> str:
        """
        Decodes a message from a stego file using steganography.

        Args:
            stego_file (str): The path to the stego file.
            bits_to_use (int): The number of least significant bits used for encoding. Optional.

        Returns:
            str: The decoded message.
        """
        pass
