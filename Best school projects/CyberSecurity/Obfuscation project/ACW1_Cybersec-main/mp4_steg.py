import os
import struct
from typing import BinaryIO
import math
from type import type_recognition, Type
from steganography_base import SteganographyBase
import io

HEADER_TYPE = "steg"
HEADER_TYPE_SIZE = 32

class MP4Steg(SteganographyBase):
    def get_file_size(self, file: BinaryIO) -> int:
        """
        Returns the size of a file in bytes.

        Args:
            file (BinaryIO): The file object.

        Returns:
            int: The size of the file in bytes.
        """
        file.seek(0, os.SEEK_END)
        return file.tell()

    def get_num_blocks(self, file: BinaryIO, start: int, end: int) -> int:
        """
        Calculates the number of blocks in a file within the specified range.

        Args:
            file (BinaryIO): The file object to read from.
            start (int): The starting position in the file.
            end (int): The ending position in the file.

        Returns:
            int: The number of blocks in the specified range.

        Raises:
            ValueError: If an unexpected end of file is encountered.
        """
        num_blocks = 0
        file.seek(start)

        while start < end:
            buffer = file.read(4)
            if len(buffer) < 4:
                raise ValueError("Unexpected end of file")
            
            block_size = struct.unpack('>I', buffer)[0]
            start += block_size
            file.seek(start)
            
            num_blocks += 1

        return num_blocks

    def get_blocks_offsets(self, file: BinaryIO, start: int, end: int) -> list[int]:
        """
        Retrieves the offsets of blocks within a file.

        Args:
            file (BinaryIO): The file object to read from.
            start (int): The starting offset.
            end (int): The ending offset.

        Returns:
            list[int]: A list of offsets of blocks within the specified range.
        """
        offsets = []
        file_size = self.get_file_size(file)

        file.seek(start)

        while start < min(file_size, end):
            offsets.append(start)

            # Read & advance 4 bytes
            buffer = file.read(4)
            if len(buffer) < 4:
                break  # End of file or reached end offset

            # Unpack buffer to integer (big endian by default)
            block_size = struct.unpack('>I', buffer)[0]
            start += block_size

            file.seek(start)

        return offsets

    def print_block_info(self, file: BinaryIO, block_offset: int, current_block_number: int, num_blocks: int, block_level: int):
        """
        Prints information about a block in a video file.

        Args:
            file (BinaryIO): The video file object.
            block_offset (int): The offset of the block in the file.
            current_block_number (int): The current block number.
            num_blocks (int): The total number of blocks.
            block_level (int): The level of the block in the block hierarchy.

        Returns:
            None
        """
        file.seek(block_offset, os.SEEK_SET)
        block_size = struct.unpack('>I', file.read(4))[0]
        block_type = struct.unpack('>I', file.read(4))[0]

        type_data = type_recognition(block_type)
        

        indent = ' ' * (4 * block_level)

        # Print the branch
        if block_level > 0:
            branch = '└── ' if current_block_number == num_blocks else '├── '
            if current_block_number == 1:
                print('│' + ' ' * (4 * (block_level - 1)) + '│')
            print('│' + ' ' * (4 * (block_level - 1)) + branch)

        print(f"{indent}[{type_data.name}] (0x{block_offset:08x}) {block_size:7} bytes: {type_data.description}")

        if block_type in [0x6d6f6f76, 0x7472616b, 0x6d646961, 0x6d696e66, 0x7374626c, 0x75647461]:  # moov, trak, mdia, minf, stbl, udta
            block_level += 1
            from_offset = block_offset + 8
            to_offset = block_offset + block_size
            num_blocks = self.get_num_blocks(file, from_offset, to_offset)
            blocks = self.get_blocks_offsets(file, from_offset, to_offset)

            for i in range(num_blocks):
                self.print_block_info(file, blocks[i], i + 1, num_blocks, block_level)

    def write_header(self, file: BinaryIO, start_from: int, msg_size: int, lsb_count: int, chunk_size: int) -> int:
        """
        Writes the header information to the file.

        Args:
            file (BinaryIO): The file object to write to.
            start_from (int): The starting offset to write the header.
            msg_size (int): The size of the message in bits.
            lsb_count (int): The number of least significant bits to use for encoding.
            chunk_size (int): The size of the chunk to encode the message in.

        Returns:
            int: The size of the header in bytes.
        """
        # Calculate the size of the message in bytes
        msg_size_byte = math.ceil(math.log2(msg_size) / 8)
        header_size = msg_size_byte + HEADER_TYPE_SIZE 
        msg_size_byte_hex_str = f'{msg_size:0{msg_size_byte * 2}x}'
        
        # Write total size in hex as bytes
        file.seek(start_from)
        file.write(bytes.fromhex(msg_size_byte_hex_str))

        # Write header type with 1 LSB
        self.write_data(file, start_from + msg_size_byte, HEADER_TYPE_SIZE, HEADER_TYPE, 1)

        return header_size

    def ascii_to_binary(self, ascii_string: str) -> str:
        """
        Converts an ASCII string to a binary string.

        Args:
            ascii_string (str): The ASCII string to convert.

        Returns:
            str: The binary string representation of the ASCII string.
        """
        return ''.join(f'{ord(char):08b}' for char in ascii_string)

    def write_data(self, file: BinaryIO, start_from: int, msg_size: int, message: str, num_lsb: int) -> None:
        """
        Writes the message data to the file using LSB steganography.

        Args:
            file (BinaryIO): The file object to write to.
            start_from (int): The starting offset to write the message.
            msg_size (int): The size of the message in bits.
            message (str): The message to encode.
            num_lsb (int): The number of least significant bits to use for encoding.

        Returns:
            None
        """
        binary_message = self.ascii_to_binary(message)  # Convert ASCII message to binary
        file.seek(start_from)
        msg_index = 0  # Index to track position in the binary message string

        for _ in range(msg_size):
            if msg_index >= len(binary_message):
                break  # Avoid index error if message is shorter than msg_size

            # Read the current byte
            byte = int.from_bytes(file.read(1), byteorder='little')
            original_byte = byte  # Store the original byte for comparison

            # Process each LSB in the byte
            for lsb_index in range(num_lsb):
                if msg_index >= len(binary_message):
                    break  # Avoid going beyond the message length

                # Calculate the current bit and its parity
                bit = int(binary_message[msg_index])
                bit_parity = bit % 2

                # Determine the parity of the current LSB
                byte_parity = (byte >> lsb_index) & 1

                # Modify the byte if parities do not match
                if bit_parity != byte_parity:
                    byte ^= 1 << lsb_index  # Flip the lsb_index-th bit

                msg_index += 1

            # Write the byte back if it was modified
            if byte != original_byte:
                file.seek(-1, 1)  # Move back to overwrite the read byte
                file.write(byte.to_bytes(1, byteorder='little'))

    def mp4_encode_message(self, file: BinaryIO, start_from: int, msg_size: int, message: str, lsb_count: int, chunk_size: int) -> None:
        """
        Encodes a message into a file using LSB steganography.

        Args:
            file (BinaryIO): The file object to write to.
            start_from (int): The starting offset to write the message.
            msg_size (int): The size of the message in bits.
            message (str): The message to encode.
            lsb_count (int): The number of least significant bits to use for encoding.
            chunk_size (int): The size of the chunk to encode the message in.

        Returns:
            None
        """
        header_size_byte = self.write_header(file, start_from, msg_size, lsb_count, chunk_size)
        self.write_data(file, start_from + header_size_byte, msg_size, message, lsb_count)

    def read_data(self, file: BinaryIO, start_from: int, msg_size: int, num_lsb: int) -> str:
        """
        Reads the message data from the file using LSB steganography.

        Args:
            file (BinaryIO): The file object to read from.
            start_from (int): The starting offset to read the message.
            msg_size (int): The size of the message in bits.
            num_lsb (int): The number of least significant bits used for encoding.

        Returns:
            str: The decoded message.

        """
        file.seek(start_from)
        decoded_bits = []
        bits_collected = 0

        while bits_collected < msg_size:
            # Read the current byte
            byte = int.from_bytes(file.read(1), byteorder='little')

            # Process each LSB in the byte
            for lsb_index in range(num_lsb):
                if bits_collected >= msg_size:
                    break  # Stop if we have collected enough bits

                # Get the value of the current LSB
                lsb_value = (byte >> lsb_index) & 1
                decoded_bits.append(str(lsb_value))

                bits_collected += 1

        decoded_message = ''.join(decoded_bits)
        # Convert binary string to ASCII
        final_message = ''.join(chr(int(decoded_message[i:i+8], 2)) for i in range(0, len(decoded_message), 8))
        return final_message

    def find_msg_size_byte(self, file: BinaryIO, start_from: int, num_lsb: int, chunk_size: int, header_type: str = "steg") -> int:
        """
        Finds the message size byte in a file.

        Args:
            file (BinaryIO): The file object to search in.
            start_from (int): The starting position to search from.
            num_lsb (int): The number of least significant bits used for encoding.
            chunk_size (int): The size of each chunk to search.
            header_type (str, optional): The header type to search for. Defaults to "steg".

        Returns:
            int: The position of the message size byte, or -1 if not found.
        """
        file.seek(start_from)
        for i in range(chunk_size):
            if i + HEADER_TYPE_SIZE > chunk_size:
                break
            tmp = self.read_data(file, start_from + i, HEADER_TYPE_SIZE, 1)
            if tmp == header_type:
                return i
        return -1
        
    def mp4_decode_message(self, file: BinaryIO, start_from: int, lsb_count: int, chunk_size: int) -> str:
        """
        Decodes a message from a file using LSB steganography.

        Args:
            file (BinaryIO): The file object to read from.
            start_from (int): The starting offset to read the message.
            lsb_count (int): The number of least significant bits used for encoding.

        Returns:
            str: The decoded message.

        Raises:
            ValueError: If the header is not found.
        """
        msg_size_byte = self.find_msg_size_byte(file, start_from, lsb_count, chunk_size)
        if msg_size_byte == -1:
            raise ValueError("Header not found")
        
        file.seek(start_from)
        buffer = file.read(msg_size_byte)
        msg_size = int.from_bytes(buffer, 'big')
        msg = self.read_data(file, start_from + msg_size_byte + HEADER_TYPE_SIZE, msg_size, lsb_count)

        return msg

    def limit_check(self, chunk_size, msg_size, lsb_count):
        msg_size_byte = math.ceil(math.log2(msg_size) / 8)
        header_size = msg_size_byte + HEADER_TYPE_SIZE
        available_space = chunk_size - header_size
        adjusted_msg_size = math.ceil(msg_size / lsb_count)

        if adjusted_msg_size > available_space:
            raise ValueError(f"Message size {msg_size} exceeds available space {available_space}")


    def process_block(self, file: BinaryIO, mode: str, block_offset: int, msg_size: int, msg: str, lsb_count: int) -> None:
        """
        Processes a block in the file based on the specified mode.

        Args:
            file (BinaryIO): The file object to read from or write to.
            mode (str): The mode of operation ('w' for write, 'r' for read).
            block_offset (int): The offset of the block in the file.
            msg_size (int): The size of the message in bits.
            msg (str): The message to encode or decode.
            lsb_count (int): The number of least significant bits to use for encoding or decoding.

        Returns:
            None
        """
        # Read block size and type
        file.seek(block_offset)
        buffer = file.read(4)
        block_size = int.from_bytes(buffer, 'big')
        block_type = int.from_bytes(file.read(4), 'big')

        if block_type == Type.mdat.value and block_size > 8:  # 'mdat' type in hexadecimal
            # Workspace for the message is central third of the block
            chunk_size = block_size // 3
            start_from = block_offset + chunk_size
            
            if mode == 'w':
                # Check if message size exceeds block size
                self.limit_check(chunk_size, msg_size, lsb_count)
                self.mp4_encode_message(file, start_from, msg_size, msg, lsb_count, chunk_size)
            elif mode == 'r':
                print(self.mp4_decode_message(file, start_from, lsb_count, chunk_size))

    def process_file(self, file: BinaryIO, mode: str, msg: str, lsb_count: int) -> None:
        """
        Processes the entire file based on the specified mode.

        Args:
            file (BinaryIO): The file object to read from or write to.
            mode (str): The mode of operation ('w' for write, 'r' for read, 'i' for information).
            msg (str): The message to encode or decode.
            lsb_count (int): The number of least significant bits to use for encoding or decoding.

        Returns:
            None

        Raises:
            ValueError: If the LSB count is not between 1 and 8.
        """
        if lsb_count < 1 or lsb_count > 8:
            raise ValueError("LSB count must be between 1 and 8")
        from_, to = 0, self.get_file_size(file)
        num_blocks = self.get_num_blocks(file, from_, to)
        blocks = self.get_blocks_offsets(file, from_, to)
        msg_size = len(msg) * 8

        if mode in ('w', 'r'):
            for block in blocks:
                self.process_block(file, mode, block, msg_size, msg, lsb_count)
        elif mode == 'i':
            for i, block in enumerate(blocks):
                self.print_block_info(file, block, i+1, num_blocks, 0)

    def encode_message(self, cover_file: str, message: str, bits_to_use: int) -> bytes:
        """
        Encodes a message into a cover file using steganography and returns the binary content.

        Args:
            cover_file (str): The path to the cover file.
            message (str): The message to be encoded.
            bits_to_use (int): The number of least significant bits to use for encoding.

        Returns:
            bytes: The binary content of the encoded file.
        """

        # Read the original file content into memory
        with open(cover_file, "rb") as file:
            original_content = file.read()

        # Create an in-memory bytes buffer to work with
        with io.BytesIO(original_content) as file:
            from_, to = 0, self.get_file_size(file)
            blocks = self.get_blocks_offsets(file, from_, to)
            msg_size = len(message) * 8
            for block in blocks:
                file.seek(block)
                buffer = file.read(4)
                block_size = int.from_bytes(buffer, 'big')
                block_type = int.from_bytes(file.read(4), 'big')
                if block_type == Type.mdat.value and block_size > 8:  # 'mdat' type in hexadecimal
                    # Workspace for the message is central third of the block
                    chunk_size = block_size // 3
                    start_from = block + chunk_size

                    self.mp4_encode_message(file, start_from, msg_size, message, bits_to_use, chunk_size)

            # Get the modified content
            encoded_content = file.getvalue()

        return encoded_content

    def calculate_capacity(self, cover_file: str, bits_to_use: int, message: str) -> tuple:
            """
            Calculates the capacity of the given cover file for steganography.

            Args:
                cover_file (str): The path to the cover file.
                bits_to_use (int): The number of bits to use for each pixel.

            Returns:
                tuple: A tuple containing the type of the capacity (e.g. "characters", "bytes", "bit", etc.) and the capacity value.
                e.g. ("byte", 1000)
            """
            with open(cover_file, "rb") as file:
                from_, to = 0, self.get_file_size(file)
                blocks = self.get_blocks_offsets(file, from_, to)
                msg_size = len(message) * 8
                for block in blocks:
                    file.seek(block)
                    buffer = file.read(4)
                    block_size = int.from_bytes(buffer, 'big')
                    block_type = int.from_bytes(file.read(4), 'big')
                    if block_type == Type.mdat.value and block_size > 8:  # 'mdat' type in hexadecimal
                        # Workspace for the message is central third of the block
                        chunk_size = block_size // 3

                        msg_size_byte = math.ceil(math.log2(msg_size) / 8)
                        header_size = msg_size_byte + HEADER_TYPE_SIZE
                        available_space = chunk_size - header_size
            return ("bytes", available_space)

    def calculate_required_space(self, message: str, bits_to_use: int) -> tuple:
        """
        Calculates the required space to hide the given message in the steganography technique.

        Args:
            message (str): The message to be hidden.

        Returns:
            tuple: A tuple containing the type of the required space (e.g. "characters", "bytes", "bit", etc.) and the required space value.
            e.g. ("byte", 1000)
        """
        # Calculate the total number of bits in the message
        total_bits = len(message) * 8
        # Calculate the number of bytes needed, considering the number of bits used per byte
        adjusted_msg_size = math.ceil(total_bits / bits_to_use)
        return ("bytes", adjusted_msg_size)

    def decode_message(self, stego_file, bits_to_use) -> str:
        """
        Decodes a message from a stego file using steganography.

        Args:
            stego_file (str): The path to the stego file.
            bits_to_use (int): The number of least significant bits used for encoding.

        Returns:
            str: The decoded message.
        """
        with open(stego_file, "rb") as file:
            from_, to = 0, self.get_file_size(file)
            blocks = self.get_blocks_offsets(file, from_, to)
            for block in blocks:
                file.seek(block)
                buffer = file.read(4)
                block_size = int.from_bytes(buffer, 'big')
                block_type = int.from_bytes(file.read(4), 'big')
                if block_type == Type.mdat.value and block_size > 8:
                    chunk_size = block_size // 3
                    start_from = block + chunk_size
                    return self.mp4_decode_message(file, start_from, bits_to_use, chunk_size)


if __name__ == "__main__":
    fileName = "mp4/sample_2.mp4"
    numOfLSB = 3
    mp4Steg = MP4Steg()
    message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Odio facilisis mauris sit amet massa vitae tortor condimentum lacinia. Quam elementum pulvinar etiam non quam. Nisl purus in mollis nunc sed. A condimentum vitae sapien pellentesque habitant morbi. Fermentum posuere urna nec tincidunt praesent semper feugiat nibh. Eget egestas purus viverra accumsan in nisl nisi scelerisque eu. Porttitor lacus luctus accumsan tortor posuere ac ut consequat semper. Nisl pretium fusce id velit ut. Ornare massa eget egestas purus viverra accumsan in. In metus vulputate eu scelerisque felis imperdiet proin fermentum.Est velit egestas dui id ornare arcu odio ut sem. Est pellentesque elit ullamcorper dignissim cras tincidunt lobortis feugiat. At varius vel pharetra vel turpis nunc eget lorem dolor. Orci dapibus ultrices in iaculis nunc sed augue lacus viverra. Donec et odio pellentesque diam volutpat. Neque vitae tempus quam pellentesque nec nam aliquam sem. Vitae elementum curabitur vitae nunc sed velit dignissim sodales. Fringilla est ullamcorper eget nulla facilisi. Elementum curabitur vitae nunc sed velit. Elementum nisi quis eleifend quam adipiscing. Viverra vitae congue eu consequat. Pharetra magna ac placerat vestibulum. Nisl nunc mi ipsum faucibus vitae aliquet nec ullamcorper. Eros donec ac odio tempor orci dapibus ultrices in. Nunc lobortis mattis aliquam faucibus purus. In ante metus dictum at tempor commodo. Leo urna molestie at elementum. Nunc mattis enim ut tellus. Odio pellentesque diam volutpat commodo. Quam adipiscing vitae proin sagittis nisl rhoncus. Urna porttitor rhoncus dolor purus non. Vitae elementum curabitur vitae nunc sed velit dignissim. Sollicitudin ac orci phasellus egestas tellus rutrum tellus pellentesque eu. Tempor orci eu lobortis elementum nibh. Gravida neque convallis a cras semper auctor neque vitae. Sed sed risus pretium quam vulputate dignissim suspendisse in. Donec ac odio tempor orci dapibus ultrices in. Mattis pellentesque id nibh tortor id aliquet lectus proin. Commodo viverra maecenas accumsan lacus vel facilisis volutpat. Fusce id velit ut tortor pretium viverra suspendisse potenti. Mattis pellentesque id nibh tortor id aliquet. Accumsan tortor posuere ac ut consequat semper viverra nam. Amet dictum sit amet justo donec enim diam. Aliquet lectus proin nibh nisl condimentum id venenatis a.Sit amet mattis vulputate enim nulla aliquet porttitor. Senectus et netus et malesuada fames ac. Vitae turpis massa sed elementum tempus egestas. A cras semper auctor neque vitae. Massa eget egestas purus viverra accumsan. Amet cursus sit amet dictum sit. In est ante in nibh mauris cursus mattis molestie a. Sit amet purus gravida quis blandit turpis. Nunc vel risus commodo viverra maecenas. Non quam lacus suspendisse faucibus. Ultrices neque ornare aenean euismod elementum nisi quis eleifend quam. Facilisi etiam dignissim diam quis enim lobortis scelerisque fermentum dui. Tincidunt dui ut ornare lectus sit.Vitae et leo duis ut diam. Leo duis ut diam quam nulla porttitor massa id neque. Rhoncus dolor purus non enim praesent elementum. Bibendum est ultricies integer quis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu vitae. Facilisis leo vel fringilla est. Morbi non arcu risus quis varius. Sit amet aliquam id diam maecenas ultricies mi eget. Varius vel pharetra vel turpis nunc eget lorem dolor. Sed lectus vestibulum mattis ullamcorper velit. Facilisi morbi tempus iaculis urna id. Ipsum faucibus vitae aliquet nec ullamcorper sit amet. Rhoncus est pellentesque elit ullamcorper dignissim cras. In nibh mauris cursus mattis molestie Velit euismod in pellentesque massa placerat. Cursus metus aliquam eleifend mi in nulla posuere. Enim praesent elementum facilisis leo. Ultrices dui sapien eget mi proin sed libero enim sed. Amet est placerat in egestas. Amet consectetur adipiscing elit ut aliquam purus sit amet. A scelerisque purus semper eget duis at. Non diam phasellus vestibulum lorem sed risus ultricies. Purus semper eget duis at tellus at urna. Semper viverra nam libero justo laoreet sit amet cursus. Tellus cras adipiscing enim eu turpis. Enim tortor at auctor urna nunc. Duis convallis convallis tellus id interdum velit laoreet. Tristique nulla aliquet enim tortor at auctor urna. Ultricies tristique nulla aliquet enim tortor. Consequat nisl vel pretium lectus quam id leo in. Amet nulla facilisi morbi tempus iaculis. Vulputate odio ut enim blandit volutpat maecenas volutpat. Bibendum neque egestas congue quisque egestas diam in arcu cursus. Habitasse platea dictumst vestibulum rhoncus est pellentesque elit ullamcorper.Dictum sit amet justo donec enim diam vulputate. At elementum eu facilisis sed odio morbi. Diam maecenas sed enim ut. Lectus magna fringilla urna porttitor rhoncus. Purus gravida quis blandit turpis cursus in hac habitasse platea. Consectetur libero id faucibus nisl tincidunt eget nullam non. At volutpat diam ut venenatis tellus in. Eget magna fermentum iaculis eu non diam phasellus vestibulum lorem. Eu augue ut lectus arcu bibendum. Rhoncus mattis rhoncus urna neque viverra justo nec ultrices. Pharetra sit amet aliquam id diam maecenas ultricies. Consequat semper viverra nam libero justo laoreet sit amet cursus. Euismod lacinia at quis risus sed. Adipiscing bibendum est ultricies integer quis. Aliquet nec ullamcorper sit amet risus nullam eget felis eget. Sodales ut eu sem integer. Lectus arcu bibendum at varius vel. Purus in mollis nunc sed id semper risus in. Vel fringilla est ullamcorper eget nulla facilisi etiam dignissim. Semper viverra nam libero justo laoreet. Adipiscing elit duis tristique sollicitudin nibh sit amet commodo. Amet consectetur adipiscing elit duis tristique sollicitudin nibh. Pharetra pharetra massa massa ultricies mi quis hendrerit dolor magna. Sagittis purus sit amet volutpat consequat mauris nunc congue nisi. Nisi porta lorem mollis aliquam ut. Proin sagittis nisl rhoncus mattis rhoncus urna neque viverra justo. Faucibus turpis in eu mi bibendum neque. Cras ornare arcu dui vivamus arcu felis bibendum!"*5
    with open(fileName, "r+b") as file:
        mp4Steg.process_file(file, "i", message, numOfLSB)
        # mp4Steg.process_file(file, "w", message, numOfLSB)
        # mp4Steg.process_file(file, "r", message, numOfLSB)

    mp4Steg.encode_message(fileName, message, numOfLSB, "mp4/sample_2_steg.mp4")
    print(mp4Steg.calculate_capacity(fileName, numOfLSB))
    print(mp4Steg.calculate_required_space(message, numOfLSB))
    print(mp4Steg.decode_message("mp4/sample_2_steg.mp4", numOfLSB))