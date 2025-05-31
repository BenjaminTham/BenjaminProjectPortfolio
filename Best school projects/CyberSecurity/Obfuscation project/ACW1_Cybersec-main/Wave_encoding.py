# Wave encoding utilities for stenography
# LSB Offset is the number of least significant bits to encode data
# For example, LSB offset of 3 means that the LSB 0 , 1, 2 bits are used to encode
# Works specifically for Sample width size 2 (16 bits)
import wave
import numpy as np
from steganography_base import SteganographyBase

# Debug flag to print debug messages
DEBUG = False
class wave_steganography(SteganographyBase):
    def encode_message(self, cover_file: str, message: str, bits_to_use: int) -> bytes:

        self.clear_encoded_data()
        self.clear_metadata()
        self.clear_wav_data()

        self.set_data_to_encode(message)
        self.read_wav(cover_file)
        self.set_LSB_OFFSET(bits_to_use)

        if self.encoding_data_limit_check():
            self.wav_encode()
            return self.write_encoded_wav()
        else:
            print("No Encoding was done.")
            return None

    def calculate_capacity(self, cover_file: str, bits_to_use: int, message: str) -> tuple:
        self.read_wav(cover_file)
        self.set_LSB_OFFSET(bits_to_use)
        available_space = self.FRAME_COUNT * self.N_CHANNELS * self.LSB_OFFSET // 8
        self.clear_wav_data()
        self.clear_metadata()
        return ("bytes", available_space)

    def calculate_required_space(self, message: str, bits_to_use: int) -> tuple:
        self.set_data_to_encode(message)
        data_len: int = len(self.DATA_TO_ENCODE)
        self.clear_data_to_encode()
        self.clear_metadata()
        return ("bytes", data_len)

    def decode_message(self, stego_file: str, bits_to_use: int) -> str:
        self.clear_encoded_data()
        self.clear_metadata()
        self.clear_wav_data()

        self.read_encoded_wav(stego_file)
        self.set_LSB_OFFSET(bits_to_use)
        return self.wav_decode()

    def __init__(self):
        self.WAV_FILENAME = ""  # Wave file name
        self.LSB_OFFSET = 1  # Least Significant Bit Offset for stenography
        self.N_CHANNELS = 2  # Mono/Stereo Channels
        self.SAMPLE_WIDTH = 2  # Sample Width: 1 byte = 8 bits (unsigned), 2 bytes = 16 bits (signed)
        self.MASK = 0 # Mask to extract LSB bits
        self.FRAME_RATE = 44100  # Frame Rate
        self.FRAME_COUNT = 0  # Frame Count
        self.COMP_TYPE = 'NONE'  # Compression Type, should always be 'NONE'
        self.WAV_DATA: np = np  # Raw wave data to encode data
        self.DATA_TO_ENCODE: np = np  # Text data to encode (ascii)
        self.DELIMITER = "0000000000000000"  # Delimiter to separate data
        self.ENCODED_DATA: np = np  # Encoded wave data with data


    def debug_print(self, msg, arg=""):
        if DEBUG:
            print(msg, arg)
    def set_LSB_OFFSET(self, offset):
        if offset < 1 or offset > 8:
            raise ValueError("Invalid LSB Offset. Must be between 1 and 8")
        else:
            self.LSB_OFFSET = offset
            self.MASK = (1 << self.LSB_OFFSET) - 1
    def set_filename(self, filename):
        if filename == "" or filename is None:
            print("Invalid filename")
            return
        else:
            if filename.endswith('.wav'):
                self.WAV_FILENAME = filename
            else:
                filename.append('.wav')

    # Read wav file to configure metadata for encoding
    def read_wav(self, filename):
        try:
            wave_file = wave.open(filename, 'r') # Open wave file
        except (FileNotFoundError, wave.Error) as e:
            print("Error reading wave file: ", e)
            return
        self.WAV_FILENAME = filename
        self.FRAME_COUNT = wave_file.getnframes()
        self.N_CHANNELS = wave_file.getnchannels()
        self.SAMPLE_WIDTH = wave_file.getsampwidth()
        self.FRAME_RATE = wave_file.getframerate()
        self.COMP_TYPE = wave_file.getcomptype()
        self.WAV_DATA = np.frombuffer(wave_file.readframes(self.FRAME_COUNT), dtype=np.int16)
        self.debug_print("Wav Data: ", self.WAV_DATA)

        # Calculate the mask to extract LSB bits
        self.MASK = (1 << self.LSB_OFFSET) - 1
        bin_MASK = bin(self.MASK)
        self.debug_print("LSB Offset: ", self.LSB_OFFSET)
        self.debug_print("Mask: ", bin_MASK)
        wave_file.close()

    def read_encoded_wav(self, filename):
        try:
            wave_file = wave.open(filename, 'r') # Open wave file
        except (FileNotFoundError, wave.Error) as e:
            print("Error reading wave file: ", e)
            return
        self.WAV_FILENAME = filename
        self.FRAME_COUNT = wave_file.getnframes()
        self.N_CHANNELS = wave_file.getnchannels()
        self.SAMPLE_WIDTH = wave_file.getsampwidth()
        self.FRAME_RATE = wave_file.getframerate()
        self.COMP_TYPE = wave_file.getcomptype()
        self.ENCODED_DATA = np.frombuffer(wave_file.readframes(self.FRAME_COUNT), dtype=np.int16)
        self.debug_print("Encoded Data: ", self.ENCODED_DATA)

        # Calculate the mask to extract LSB bits
        self.MASK = (1 << self.LSB_OFFSET) - 1
        bin_MASK = bin(self.MASK)
        self.debug_print("LSB Offset: ", self.LSB_OFFSET)
        self.debug_print("Mask: ", bin_MASK)
        wave_file.close()

    # Set data to encode in binary format
    def set_data_to_encode(self, data):
        # check that data is not empty and in binary format
        if data is None or data == "":
            print("Data to encode is empty")
            return
        else:
            data_to_encode = "".join(format(ord(char), '08b') for char in data)
            self.DATA_TO_ENCODE = data_to_encode + self.DELIMITER
            self.debug_print("Data to encode: ", self.DATA_TO_ENCODE)

    def clear_wav_data(self):
        self.WAV_DATA = None

    def clear_encoded_data(self):
        self.ENCODED_DATA = None

    def clear_data_to_encode(self):
        self.DATA_TO_ENCODE = None
    def clear_metadata(self):
        self.WAV_FILENAME = ""
        self.LSB_OFFSET = 1
        self.N_CHANNELS = 2
        self.SAMPLE_WIDTH = 1
        self.MASK = 0
        self.FRAME_RATE = 44100
        self.FRAME_COUNT = 0
        self.COMP_TYPE = 'NONE'
        self.WAV_DATA = None
        self.DATA_TO_ENCODE = None
        self.ENCODED_DATA = None

    def encoding_data_limit_check(self):
        if self.DATA_TO_ENCODE is None or self.DATA_TO_ENCODE == "":
            print("Data to encode is empty")
            return False
        if self.WAV_DATA is None:
            print("Wave data is empty")
            return False
        if self.LSB_OFFSET < 1 or self.LSB_OFFSET > 8:
            print("Invalid LSB Offset. Must be between 1 and 8")
            return False
        if self.SAMPLE_WIDTH != 2:
            print("Invalid Sample Width. Must be 2 bytes (16 bits)")
            return False

        # Calculate the available LSB data length
        max_bytes_to_hide = self.FRAME_COUNT * self.N_CHANNELS * self.LSB_OFFSET // 8
        self.debug_print("Max Bits available for Hiding: ", max_bytes_to_hide)
        self.debug_print("Bit of Data to Encode: ", len(self.DATA_TO_ENCODE))

        if len(self.DATA_TO_ENCODE) > max_bytes_to_hide:
            self.debug_print("Data to encode is too large to fit in the wave data")
            return False

        self.debug_print("Data to encode can be encoded in the wave data")
        return True

    # Encode data into raw wave data
    def wav_encode(self):
        if not self.encoding_data_limit_check():
            return
        # Convert data to binary format
        binary_data = self.DATA_TO_ENCODE
        self.debug_print("Binary Data: ", binary_data)
        # Convert binary data to text
        binary_text = "".join(chr(int(binary_data[i:i + 8], 2)) for i in range(0, len(binary_data), 8))
        self.debug_print("Text of Binary data: ", binary_text)
        self.debug_print("Binary data length: ", len(binary_data))
        self.debug_print("Text data length: ", len(binary_text))
        data_length = len(binary_data)

        # self.debug_print("WAV Data Length: ", len(self.WAV_DATA))
        self.debug_print("WAV Data: ", self.WAV_DATA)


        #initialize ENCODED_DATA to be written into
        self.ENCODED_DATA = self.WAV_DATA.copy()

        # Encode data into the wave data
        data_index = 0

        for i in range (len(self.WAV_DATA)):
            if data_index >= len(binary_data):
                break
            sample = self.WAV_DATA[i]
            sample = sample & ~self.MASK
            remaining_bits = min(self.LSB_OFFSET, data_length - data_index)
            sample = sample | (int(binary_data[data_index: data_index + remaining_bits], 2) & self.MASK)
            self.ENCODED_DATA[i] = sample
            data_index += remaining_bits

        self.debug_print("Encoded Data: ", self.ENCODED_DATA)


    def wav_decode(self):
        if self.ENCODED_DATA is None or self.ENCODED_DATA.size == 0:
            print("Encoded data is empty")
            return
        BIT_WIDTH = 8  # Number of bits in a byte
        BITS_PER_SAMPLE = BIT_WIDTH * self.SAMPLE_WIDTH  # Number of bits per sample
        SIZE_OF_DELIMTER = int(len(self.DELIMITER) / 8)  # Size of the delimiter in bytes

        bin_decoded_text = ""  # Decoded text from the binary data

        # # Check if the lengths of the encoded and original wave data match
        # if self.WAV_DATA.size != self.ENCODED_DATA.size:
        #     print("Encoded data length does not match the original wave data length")
        #     return
        # else:
        #     print("Encoded data length matches the original wave data length")

        # Decode the data from the wave data
        for i in range(len(self.ENCODED_DATA)):
            # Step 2: Extract the LSB bits from the wave data
            sample = self.ENCODED_DATA[i]
            decoded_bits = format(sample & self.MASK, '016b')

            if (i == 0): self.debug_print("Index: ", i)
            if (i == 0): self.debug_print("Sample: ", decoded_bits)

            # Step 3: Write the extracted data to a buffer
            bin_decoded_text += decoded_bits[BITS_PER_SAMPLE - self.LSB_OFFSET: BITS_PER_SAMPLE]

            # Step 5: Check for delimiter to stop extraction
            if bin_decoded_text.endswith(self.DELIMITER):
                self.debug_print("Delimiter found at index: ", i)
                break

        #Convert the binary data to text
        if len(bin_decoded_text) % 8 != 0:
            bin_decoded_text = bin_decoded_text[:-(len(bin_decoded_text) % 8)]

        decoded_text = "".join(chr(int(bin_decoded_text[i:i + 8], 2)) for i in range(0, len(bin_decoded_text), 8))


        #Trim delimiter from the decoded text
        decoded_text = decoded_text[:-SIZE_OF_DELIMTER]
        self.debug_print("Decoded Text: ", decoded_text)

        #check if the decoded text is valid ascii
        if not all((128 > ord(char) > 0) for char in decoded_text):
            return "\nERROR: Invalid decoded text found. Possibly incorrect LSB Offset"

        return decoded_text


    # Checks if the encoded data matches the original wave data
    # Returns True if the data matches, False otherwise
    def check_encode_original_match(self):
        # Compares the original WAV and Encoded WAV data
        if np.array_equal(self.WAV_DATA, self.ENCODED_DATA):
            self.debug_print("Encoded data matches the original wave data")
            return True
        else:
            self.debug_print("Encoded data does not match the original wave data")
            return False

    # Check if the metadata of the wave file matches the metadata of the encoded wave file.
    # This is used to verify that the files for encoding and decoding are the same
    # Returns True if the metadata matches, False otherwise
    def check_metadata_match(self, filename):
        try:
            wave_file = wave.open(filename, 'rb') # Open wave file
        except (FileNotFoundError, wave.Error) as e:
            print("Error reading wave file: ", e)
            return False
        msg = ""
        if self.FRAME_COUNT != wave_file.getnframes(): msg += "Frame Count does not match\n"
        if self.N_CHANNELS != wave_file.getnchannels(): msg += "Channels does not match\n"
        if self.SAMPLE_WIDTH != wave_file.getsampwidth(): msg += "Sample Width does not match\n"
        if self.FRAME_RATE != wave_file.getframerate(): msg += "Frame Rate does not match\n"
        if self.COMP_TYPE != wave_file.getcomptype(): msg += "Compression Type does not match\n"
        wave_file.close()

        if msg == "":
            self.debug_print("Metadata matches the encoded wave file")
            return True
        else:
            self.debug_print("Metadata does not match the encoded wave file")
            self.debug_print(msg)
            return False

    # Write wave data to file
    def write_encoded_wav(self):
        if self.ENCODED_DATA is None or self.ENCODED_DATA.size == 0:
            print("Encoded data is empty")
            return
        if self.WAV_FILENAME is None or self.WAV_FILENAME == "":
            print("Wave file name is empty")
            return
        try:
            #encoded_wav_filename = self.WAV_FILENAME.replace('.wav', '_encoded.wav')

            wave_file: wave = wave.open("data/wav/temp/Temp.wav", 'wb')
            wave_file.setnchannels(self.N_CHANNELS)
            wave_file.setsampwidth(self.SAMPLE_WIDTH)
            wave_file.setframerate(self.FRAME_RATE)
            wave_file.setnframes(self.FRAME_COUNT)
            wave_file.setcomptype(self.COMP_TYPE, 'NONE')
            wave_file.writeframes(self.ENCODED_DATA)
            wave_file.close()

            with open("data/wav/temp/Temp.wav", 'rb') as fd:
                contents = fd.read()

            return contents
        except (FileNotFoundError, wave.Error) as e:
            print("Error writing wave file: ", e)
    # Write raw wave data to file

    def print_metadata(self):
        print("Wave File Metadata")
        print("Wave File Name: \t\t", self.WAV_FILENAME)
        print("Frame Count: \t\t", self.FRAME_COUNT)
        print("Channels: \t\t", self.N_CHANNELS)
        print("Sample Width: \t\t", self.SAMPLE_WIDTH)
        print("Frame Rate: \t\t", self.FRAME_RATE)
        print("Compression Type: \t\t", self.COMP_TYPE)
        print("LSB Offset: \t\t", self.LSB_OFFSET)
        # self.debug_print("Wav Data: ", self.WAV_DATA)
        # self.debug_print("Data to Encode: ", self.DATA_TO_ENCODE)
        # self.debug_print("Encoded Data: ", self.ENCODED_DATA)

if __name__ == "__main__":
    text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Odio facilisis mauris sit amet massa vitae tortor condimentum lacinia. Quam elementum pulvinar etiam non quam. Nisl purus in mollis nunc sed. A condimentum vitae sapien pellentesque habitant morbi. Fermentum posuere urna nec tincidunt praesent semper feugiat nibh. Eget egestas purus viverra accumsan in nisl nisi scelerisque eu. Porttitor lacus luctus accumsan tortor posuere ac ut consequat semper. Nisl pretium fusce id velit ut. Ornare massa eget egestas purus viverra accumsan in. In metus vulputate eu scelerisque felis imperdiet proin fermentum.Est velit egestas dui id ornare arcu odio ut sem. Est pellentesque elit ullamcorper dignissim cras tincidunt lobortis feugiat. At varius vel pharetra vel turpis nunc eget lorem dolor. Orci dapibus ultrices in iaculis nunc sed augue lacus viverra. Donec et odio pellentesque diam volutpat. Neque vitae tempus quam pellentesque nec nam aliquam sem. Vitae elementum curabitur vitae nunc sed velit dignissim sodales. Fringilla est ullamcorper eget nulla facilisi. Elementum curabitur vitae nunc sed velit. Elementum nisi quis eleifend quam adipiscing. Viverra vitae congue eu consequat. Pharetra magna ac placerat vestibulum. Nisl nunc mi ipsum faucibus vitae aliquet nec ullamcorper. Eros donec ac odio tempor orci dapibus ultrices in. Nunc lobortis mattis aliquam faucibus purus. In ante metus dictum at tempor commodo. Leo urna molestie at elementum. Nunc mattis enim ut tellus. Odio pellentesque diam volutpat commodo. Quam adipiscing vitae proin sagittis nisl rhoncus. Urna porttitor rhoncus dolor purus non. Vitae elementum curabitur vitae nunc sed velit dignissim. Sollicitudin ac orci phasellus egestas tellus rutrum tellus pellentesque eu. Tempor orci eu lobortis elementum nibh. Gravida neque convallis a cras semper auctor neque vitae. Sed sed risus pretium quam vulputate dignissim suspendisse in. Donec ac odio tempor orci dapibus ultrices in. Mattis pellentesque id nibh tortor id aliquet lectus proin. Commodo viverra maecenas accumsan lacus vel facilisis volutpat. Fusce id velit ut tortor pretium viverra suspendisse potenti. Mattis pellentesque id nibh tortor id aliquet. Accumsan tortor posuere ac ut consequat semper viverra nam. Amet dictum sit amet justo donec enim diam. Aliquet lectus proin nibh nisl condimentum id venenatis a.Sit amet mattis vulputate enim nulla aliquet porttitor. Senectus et netus et malesuada fames ac. Vitae turpis massa sed elementum tempus egestas. A cras semper auctor neque vitae. Massa eget egestas purus viverra accumsan. Amet cursus sit amet dictum sit. In est ante in nibh mauris cursus mattis molestie a. Sit amet purus gravida quis blandit turpis. Nunc vel risus commodo viverra maecenas. Non quam lacus suspendisse faucibus. Ultrices neque ornare aenean euismod elementum nisi quis eleifend quam. Facilisi etiam dignissim diam quis enim lobortis scelerisque fermentum dui. Tincidunt dui ut ornare lectus sit.Vitae et leo duis ut diam. Leo duis ut diam quam nulla porttitor massa id neque. Rhoncus dolor purus non enim praesent elementum. Bibendum est ultricies integer quis. Pretium vulputate sapien nec sagittis aliquam malesuada bibendum arcu vitae. Facilisis leo vel fringilla est. Morbi non arcu risus quis varius. Sit amet aliquam id diam maecenas ultricies mi eget. Varius vel pharetra vel turpis nunc eget lorem dolor. Sed lectus vestibulum mattis ullamcorper velit. Facilisi morbi tempus iaculis urna id. Ipsum faucibus vitae aliquet nec ullamcorper sit amet. Rhoncus est pellentesque elit ullamcorper dignissim cras. In nibh mauris cursus mattis molestie Velit euismod in pellentesque massa placerat. Cursus metus aliquam eleifend mi in nulla posuere. Enim praesent elementum facilisis leo. Ultrices dui sapien eget mi proin sed libero enim sed. Amet est placerat in egestas. Amet consectetur adipiscing elit ut aliquam purus sit amet. A scelerisque purus semper eget duis at. Non diam phasellus vestibulum lorem sed risus ultricies. Purus semper eget duis at tellus at urna. Semper viverra nam libero justo laoreet sit amet cursus. Tellus cras adipiscing enim eu turpis. Enim tortor at auctor urna nunc. Duis convallis convallis tellus id interdum velit laoreet. Tristique nulla aliquet enim tortor at auctor urna. Ultricies tristique nulla aliquet enim tortor. Consequat nisl vel pretium lectus quam id leo in. Amet nulla facilisi morbi tempus iaculis. Vulputate odio ut enim blandit volutpat maecenas volutpat. Bibendum neque egestas congue quisque egestas diam in arcu cursus. Habitasse platea dictumst vestibulum rhoncus est pellentesque elit ullamcorper.Dictum sit amet justo donec enim diam vulputate. At elementum eu facilisis sed odio morbi. Diam maecenas sed enim ut. Lectus magna fringilla urna porttitor rhoncus. Purus gravida quis blandit turpis cursus in hac habitasse platea. Consectetur libero id faucibus nisl tincidunt eget nullam non. At volutpat diam ut venenatis tellus in. Eget magna fermentum iaculis eu non diam phasellus vestibulum lorem. Eu augue ut lectus arcu bibendum. Rhoncus mattis rhoncus urna neque viverra justo nec ultrices. Pharetra sit amet aliquam id diam maecenas ultricies. Consequat semper viverra nam libero justo laoreet sit amet cursus. Euismod lacinia at quis risus sed. Adipiscing bibendum est ultricies integer quis. Aliquet nec ullamcorper sit amet risus nullam eget felis eget. Sodales ut eu sem integer. Lectus arcu bibendum at varius vel. Purus in mollis nunc sed id semper risus in. Vel fringilla est ullamcorper eget nulla facilisi etiam dignissim. Semper viverra nam libero justo laoreet. Adipiscing elit duis tristique sollicitudin nibh sit amet commodo. Amet consectetur adipiscing elit duis tristique sollicitudin nibh. Pharetra pharetra massa massa ultricies mi quis hendrerit dolor magna. Sagittis purus sit amet volutpat consequat mauris nunc congue nisi. Nisi porta lorem mollis aliquam ut. Proin sagittis nisl rhoncus mattis rhoncus urna neque viverra justo. Faucibus turpis in eu mi bibendum neque. Cras ornare arcu dui vivamus arcu felis bibendum."

    wav = wave_steganography()

    # Example usage of the wave_encode class
    print("The Capacity is (in bytes):",
          wav.calculate_capacity('data/CantinaBand60.wav', 4, text))
    print("The Required Space (in bytes) is:",
          wav.calculate_required_space(text, 4))


    print("Encode Message:")

    with open("data/CantinaBand60_encoded.wav", 'wb') as f:
        f.write(wav.encode_message('data/CantinaBand60.wav', text, 2))
        f.close()

    print(wav.print_metadata())
    print("Encoding data limit check:", wav.encoding_data_limit_check())

    print("check encode and original match:", wav.check_encode_original_match())
    print("check metadata match:", wav.check_metadata_match('data/CantinaBand60_encoded.wav'))

    for LSB in range(1, 9):
        print("\n\nLSB:", LSB)
        wav.print_metadata()
        print("Decoded Message with LSB Offset of", LSB, ":",
              wav.decode_message('data/CantinaBand60_encoded.wav', LSB))


 #
 # # Example usage of the wave_encode class
 #    wav = wave_steganography()
 #    wav2 = wave_steganography()
 #    # Set the LSB Offset for encoding
 #
 #    for LSB in range(1, 9):
 #        print("\nLSB: ", LSB)
 #        wav.set_LSB_OFFSET(LSB)
 #        wav2.set_LSB_OFFSET(LSB)
 #        print("The LSB Offset is:", wav.LSB_OFFSET)
 #        # Read the wave file to encode data
 #        wav.read_wav('data/CantinaBand60.wav')
 #        # Set the data to encode (secret message)
 #        wav.set_data_to_encode(text)
 #        # Check if there is enough space to encode the data, then encode the data
 #        if wav.encoding_data_limit_check():
 #            wav.wav_encode()
 #            with open("data/CantinaBand60_encoded.wav", 'wb') as f:
 #                f.write(wav.write_encoded_wav())
 #                f.close()
 #        # clear the secret message
 #        wav.clear_data_to_encode()
 #
 #        # check if the metadata of the encoded and decoded files match,
 #        # then write the encoded data to a new file
 #        if not wav.check_encode_original_match(): wav.write_encoded_wav()
 #
 #        # IF LSB Offset is incorrect, the decoded message will be incorrect
 #
 #
 #        # Decode the encoded data from the new file
 #
 #
 #
 #        wav2.read_encoded_wav('data/CantinaBand60_encoded.wav')
 #        wav.print_metadata()
 #        wav2.print_metadata()
 #        print("\nMetadata match: ", wav2.check_metadata_match('data/CantinaBand60.wav'))
 #        print("The secret message is:", wav2.wav_decode())
 #
