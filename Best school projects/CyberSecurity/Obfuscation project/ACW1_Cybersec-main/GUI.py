import sys
import os
import vlc

from PyQt5.QtWidgets import (
    QApplication,
    QSpinBox,
    QSpacerItem,
    QSplitter,
    QSizePolicy,
    QMainWindow,
    QLabel,
    QFileDialog,
    QPushButton,
    QVBoxLayout,
    QHBoxLayout,
    QWidget,
    QMessageBox,
)
from PyQt5.QtCore import Qt
from PyQt5.QtGui import QPixmap, QMovie
from PyQt5.QtMultimedia import QMediaPlayer, QMediaContent
from PyQt5.QtMultimediaWidgets import QVideoWidget
from PyQt5.QtCore import QUrl
from steganography_factory import get_steganography_algorithm
from PyQt5.QtCore import QObject, QThread, pyqtSignal
from PyQt5.QtWidgets import (
    QMainWindow,
    QApplication,
    QPushButton,
    QLabel,
    QVBoxLayout,
    QWidget,
    QApplication,
    QLabel,
    QMainWindow,
    QPushButton,
    QVBoxLayout,
    QWidget,
)
import sys
from PyQt5.QtWidgets import QTextEdit


class Worker(QObject):
    progress = pyqtSignal(int)
    finished = pyqtSignal()
    decoded_message_signal = pyqtSignal(str)
    log_message_signal = pyqtSignal(str)

    def __init__(
        self,
        steganography_algorithm,
        bits_to_use,
        cover_file_path,
        payload_text,
        save_path,
    ):
        super().__init__()
        self.steganography_algorithm = steganography_algorithm
        self.bits_to_use = bits_to_use
        self.cover_file_path = cover_file_path
        self.payload_text = payload_text
        self.save_path = save_path
        self.original_stdout = sys.stdout

    def redirect_stdout(self):
        sys.stdout = self

    def restore_stdout(self):
        sys.stdout = self.original_stdout

    def write(self, message):
        if message.strip():
            self.log_message_signal.emit(message)

    def flush(self):
        pass

    def ffv1_encode_worker(self):
        self.redirect_stdout()
        try:
            self.log_message_signal.emit(f"Payload: {self.payload_text}")
            self.log_message_signal.emit(f"checkcoverfilepath {self.cover_file_path}")
            available_space = self.steganography_algorithm.calculate_capacity(
                self.cover_file_path, self.bits_to_use, self.payload_text
            )
            required_space = self.steganography_algorithm.calculate_required_space(
                self.payload_text, self.bits_to_use
            )

            if required_space > available_space:
                self.log_message_signal.emit(f"{required_space} > {available_space}")
                self.log_message_signal.emit(
                    "The payload is too large for the selected cover object and LSB settings."
                )
                self.finished.emit()
                return

            if self.save_path.lower().endswith((".bmp", ".png")):
                image = self.steganography_algorithm.encode_message(
                    self.cover_file_path, self.payload_text, self.bits_to_use
                )
                image.save(self.save_path)
            else:
                byte_to_write = self.steganography_algorithm.encode_message(
                    self.cover_file_path, self.payload_text, self.bits_to_use
                )

                with open(self.save_path, "wb") as f:
                    f.write(byte_to_write)
                    f.close()

            self.log_message_signal.emit("Stego file saved successfully!")
        finally:
            self.restore_stdout()
            self.finished.emit()

    def ffv1_decode_worker(self):
        self.redirect_stdout()
        try:
            decoded_message = self.steganography_algorithm.decode_message(
                self.cover_file_path, self.bits_to_use
            )
            self.decoded_message_signal.emit(decoded_message)
        except Exception as e:
            self.decoded_message_signal.emit(f"Failed to decode message: {str(e)}")
        finally:
            self.restore_stdout()
            self.finished.emit()


class DraggableLabel(QLabel):
    def __init__(self, parent, initial_text="", drop_function=None, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.setText(initial_text)
        self.setAcceptDrops(True)
        self.parent = parent
        self.drop_function = drop_function

    def dragEnterEvent(self, event):
        if event.mimeData().hasUrls():
            event.accept()
        else:
            event.ignore()

    def dropEvent(self, event):
        if event.mimeData().hasUrls():
            file_path = event.mimeData().urls()[0].toLocalFile()
            if self.drop_function:
                self.drop_function(file_path)


class SteganographyApp(QMainWindow):
    def __init__(self):
        super().__init__()
        self.initUI()
        self.thread = None  # Add this to keep track of the thread
        self.worker = None  # Add this to keep track of the worker
        # VLC instances for cover and stego videos
        self.cover_vlc_instance = vlc.Instance()
        self.cover_vlc_player = self.cover_vlc_instance.media_player_new()

        self.stego_vlc_instance = vlc.Instance()
        self.stego_vlc_player = self.stego_vlc_instance.media_player_new()

    def initUI(self):
        self.setWindowTitle("Steganography Application")
        self.setGeometry(100, 100, 800, 600)

        self.cover_file_path = None
        self.stego_file_path = None
        self.payload_text = None

        self.cover_media_player = QMediaPlayer()
        self.stego_media_player = QMediaPlayer()

        # Main layout
        main_layout = QVBoxLayout()

        # Media display layout
        media_splitter = QSplitter(self)  # Using QSplitter for flexible division
        main_layout.addWidget(media_splitter)

        # Container widgets for cover and stego layouts
        cover_widget = QWidget()
        stego_widget = QWidget()
        media_splitter.addWidget(cover_widget)
        media_splitter.addWidget(stego_widget)

        # Image preview for cover file
        self.image_label = QLabel(self)
        self.image_label.setAlignment(Qt.AlignCenter)
        self.image_label.setMinimumSize(
            550, 310
        )  # Set a specific size that fits your UI
        self.image_label.hide()

        # Text preview for cover file
        self.cover_text_edit = QTextEdit(self)
        self.cover_text_edit.setReadOnly(True)
        self.cover_text_edit.setMinimumSize(400, 310)
        self.cover_text_edit.hide()

        # Cover file layout
        cover_layout = QVBoxLayout(cover_widget)
        self.top_cover_label = QLabel(
            "Steganography: Encode payload into a cover object"
        )
        self.top_cover_label.setAlignment(Qt.AlignCenter)
        self.top_cover_label.setFixedSize(550, 45)
        cover_layout.addWidget(self.top_cover_label)

        # Select Cover Drag and Drop
        self.cover_media_label = DraggableLabel(
            self, "Drag and drop a cover file here", self.handle_cover_file_drop
        )
        self.cover_media_label.setFixedSize(550, 45)
        self.cover_media_label.setAlignment(Qt.AlignCenter)
        self.cover_media_label.setStyleSheet("border: 1px solid black;")

        cover_layout.addWidget(self.cover_media_label)
        cover_layout.addWidget(self.image_label)
        cover_layout.addWidget(self.cover_text_edit)

        # Add media player controls
        self.cover_play_pause_button = QPushButton("Play", self)
        self.cover_play_pause_button.clicked.connect(
            lambda: self.toggle_play_pause("cover")
        )
        self.cover_play_pause_button.setStyleSheet(
            "QPushButton { background-color: blue; color: white; }"
        )
        self.cover_play_pause_button.hide()
        cover_layout.addWidget(self.cover_play_pause_button)

        # Media player for video and audio files
        self.cover_media_player = QMediaPlayer(self)
        self.cover_video_widget = QVideoWidget(self)
        self.cover_media_player.setVideoOutput(self.cover_video_widget)
        self.cover_video_widget.setMinimumSize(500, 300)
        cover_layout.addWidget(self.cover_video_widget)
        self.cover_video_widget.hide()

        # Select Cover Button
        self.select_cover_button = QPushButton("Select Cover File")
        self.select_cover_button.clicked.connect(lambda: self.select_file("cover"))
        cover_layout.addWidget(self.select_cover_button)

        # Select Payload File Drag and Drop
        self.text_label = DraggableLabel(
            self, "Drag and drop a text file here", self.handle_payload_drop
        )
        self.text_label.setFixedSize(550, 45)
        self.text_label.setAlignment(Qt.AlignCenter)
        self.text_label.setStyleSheet("border: 1px solid black;")
        cover_layout.addWidget(self.text_label)

        # Select Payload File Button
        self.select_payload_button = QPushButton("Select Payload File", self)
        self.select_payload_button.clicked.connect(self.select_payload_file)
        cover_layout.addWidget(self.select_payload_button)

        # Spin box for LSB selection (encode)
        self.lsb_spinbox = QSpinBox(self)
        self.lsb_spinbox.setRange(1, 8)
        self.lsb_spinbox.setValue(1)
        self.lsb_spinbox.setPrefix("Number of LSBs to use (encode): ")
        cover_layout.addWidget(self.lsb_spinbox)

        # Save button
        self.save_button = QPushButton("Save Stego File", self)
        self.save_button.clicked.connect(self.save_stego_file)
        cover_layout.addWidget(self.save_button)

        cover_layout.addItem(
            QSpacerItem(20, 40, QSizePolicy.Minimum, QSizePolicy.Expanding)
        )

        # Image preview for stego file
        self.image_label_stego = QLabel(self)
        self.image_label_stego.setAlignment(Qt.AlignCenter)
        self.image_label_stego.setMinimumSize(
            550, 310
        )  # Same size as cover image label
        self.image_label_stego.hide()

        # Text preview for stego file
        self.stego_text_edit = QTextEdit(self)
        self.stego_text_edit.setReadOnly(True)
        self.stego_text_edit.setMinimumSize(400, 310)
        self.stego_text_edit.hide()

        # Stego file layout
        stego_layout = QVBoxLayout(stego_widget)
        self.top_stego_label = QLabel(
            "Steganalysis: Extract hidden payload from steganographic object"
        )
        self.top_stego_label.setAlignment(Qt.AlignCenter)
        self.top_stego_label.setFixedSize(550, 45)
        stego_layout.addWidget(self.top_stego_label)

        # Select Stego File Drag and Drop
        self.stego_media_label = DraggableLabel(
            self, "Drag and drop a stego file here", self.handle_stego_file_drop
        )
        self.stego_media_label.setFixedSize(550, 45)
        self.stego_media_label.setAlignment(Qt.AlignCenter)
        self.stego_media_label.setStyleSheet("border: 1px solid black;")

        stego_layout.addWidget(self.stego_media_label)
        stego_layout.addWidget(self.image_label_stego)
        stego_layout.addWidget(self.stego_text_edit)

        # Add media player controls
        self.stego_play_pause_button = QPushButton("Play", self)
        self.stego_play_pause_button.clicked.connect(
            lambda: self.toggle_play_pause("stego")
        )
        self.stego_play_pause_button.setStyleSheet(
            "QPushButton { background-color: blue; color: white; }"
        )
        self.stego_play_pause_button.hide()
        stego_layout.addWidget(self.stego_play_pause_button)

        # Media player for video and audio files
        self.stego_media_player = QMediaPlayer(self)
        self.stego_video_widget = QVideoWidget(self)
        self.stego_media_player.setVideoOutput(self.stego_video_widget)
        self.stego_video_widget.setMinimumSize(500, 300)
        stego_layout.addWidget(self.stego_video_widget)
        self.stego_video_widget.hide()

        # Select Stego File Button
        self.select_stego_button = QPushButton("Select Stego File")
        self.select_stego_button.clicked.connect(lambda: self.select_file("stego"))
        stego_layout.addWidget(self.select_stego_button)

        # Spin box for LSB selection (decode)
        self.lsb_spinbox2 = QSpinBox(self)
        self.lsb_spinbox2.setRange(1, 8)
        self.lsb_spinbox2.setValue(1)
        self.lsb_spinbox2.setPrefix("Number of LSBs to use (decode): ")
        stego_layout.addWidget(self.lsb_spinbox2)

        # Button to decode message
        self.decode_button = QPushButton("Decode Message", self)
        self.decode_button.clicked.connect(self.decode_message)
        stego_layout.addWidget(self.decode_button)

        stego_layout.addItem(
            QSpacerItem(20, 40, QSizePolicy.Minimum, QSizePolicy.Expanding)
        )

        # Add QTextEdit for logging
        self.log_text_edit = QTextEdit(self)
        self.log_text_edit.setReadOnly(True)
        main_layout.addWidget(self.log_text_edit)

        # Add QTextEdit for displaying decoded message
        self.decoded_message_text_edit = QTextEdit(self)
        self.decoded_message_text_edit.setReadOnly(True)
        self.decoded_message_text_edit.hide()
        main_layout.addWidget(self.decoded_message_text_edit)

        # Set central widget
        central_widget = QWidget(self)
        central_widget.setLayout(main_layout)
        self.setCentralWidget(central_widget)

        self.show()

    def append_log_message(self, message):
        self.log_text_edit.append(message)

    # Payload file selection
    def select_payload_file(self):
        options = QFileDialog.Options()
        file_path, _ = QFileDialog.getOpenFileName(
            self, "Select Payload File", "", "Text Files (*.txt)", options=options
        )
        if file_path:
            self.handle_payload_drop(file_path)

    def handle_payload_drop(self, file_path):
        with open(file_path, "r") as f:
            self.payload_text = f.read()
        self.text_label.clear()
        self.text_label.setText(f"Selected text file: {os.path.basename(file_path)}")

    def display_media(
        self,
        file_path,
        media_label,
        image_label,
        video_widget,
        media_player,
        play_pause_button,
        text_edit,
        vlc_player,
    ):
        # Hide all media components initially
        text_edit.hide()
        image_label.hide()
        video_widget.hide()
        play_pause_button.hide()
        play_pause_button.setText("Play")

        # Set the media according to the file type
        if file_path.lower().endswith((".bmp", ".png")):
            pixmap = QPixmap(file_path)
            image_label.setPixmap(
                pixmap.scaled(
                    image_label.size(), Qt.KeepAspectRatio, Qt.SmoothTransformation
                )
            )
            image_label.show()
        elif file_path.lower().endswith(".gif"):
            movie = QMovie(file_path)
            movie.setScaledSize(image_label.size())
            image_label.setMovie(movie)
            movie.start()
            image_label.show()
        elif file_path.lower().endswith((".mp4", ".mkv")):
            media = vlc_player.get_media()
            media = self.cover_vlc_instance.media_new(file_path)
            vlc_player.set_media(media)
            vlc_player.set_hwnd(video_widget.winId())
            video_widget.show()
            play_pause_button.show()
        elif file_path.lower().endswith(".wav"):
            media_player.setMedia(QMediaContent(QUrl.fromLocalFile(file_path)))
            play_pause_button.show()
        elif file_path.lower().endswith(".txt"):
            with open(file_path, "r") as f:
                text_content = f.read()
            text_edit.setPlainText(text_content)
            text_edit.setAlignment(Qt.AlignLeft | Qt.AlignTop)
            text_edit.show()

        # Set media label text
        media_label.setText(f"Selected file: {os.path.basename(file_path)}")

    def select_file(self, file_type):
        file_path, _ = QFileDialog.getOpenFileName(
            self,
            "Select File",
            "",
            "PNG Files (*.png);;BMP Files (*.bmp);;WAV Files (*.wav);;MP4 Files (*.mp4);;FFV1 Files (*.mkv);;GIF Files (*.gif);;TEXT Files(*.txt)",
        )
        if file_path:
            if file_type == "cover":
                self.cover_file_path = file_path
                self.display_media(
                    file_path,
                    self.cover_media_label,
                    self.image_label,
                    self.cover_video_widget,
                    self.cover_media_player,
                    self.cover_play_pause_button,
                    self.cover_text_edit,
                    self.cover_vlc_player,
                )

                self.update_spinbox_state(file_path, is_cover=False)

            elif file_type == "stego":
                self.stego_file_path = file_path
                self.display_media(
                    file_path,
                    self.stego_media_label,
                    self.image_label_stego,
                    self.stego_video_widget,
                    self.stego_media_player,
                    self.stego_play_pause_button,
                    self.stego_text_edit,
                    self.stego_vlc_player,
                )

                self.update_spinbox_state(file_path, is_cover=False)

    def toggle_play_pause(self, media_type):
        if media_type == "cover":
            if self.cover_file_path.lower().endswith((".mp4", ".mkv")):
                if self.cover_vlc_player.is_playing():
                    self.cover_vlc_player.pause()
                    self.cover_play_pause_button.setText("Play")
                else:
                    self.cover_vlc_player.play()
                    self.cover_play_pause_button.setText("Pause")
            else:
                if self.cover_media_player.state() == QMediaPlayer.PlayingState:
                    self.cover_media_player.pause()
                    self.cover_play_pause_button.setText("Play")
                else:
                    self.cover_media_player.play()
                    self.cover_play_pause_button.setText("Pause")
        elif media_type == "stego":
            if self.stego_file_path.lower().endswith((".mp4", ".mkv")):
                if self.stego_vlc_player.is_playing():
                    self.stego_vlc_player.pause()
                    self.stego_play_pause_button.setText("Play")
                else:
                    self.stego_vlc_player.play()
                    self.stego_play_pause_button.setText("Pause")
            else:
                if self.stego_media_player.state() == QMediaPlayer.PlayingState:
                    self.stego_media_player.pause()
                    self.stego_play_pause_button.setText("Play")
                else:
                    self.stego_media_player.play()
                    self.stego_play_pause_button.setText("Pause")

    def set_lsb_spinbox_enabled(self, cover_enabled, stego_enabled):
        self.lsb_spinbox.setEnabled(cover_enabled)
        self.lsb_spinbox2.setEnabled(stego_enabled)

    def handle_cover_file_drop(self, file_path):
        self.cover_file_path = file_path
        self.display_media(
            file_path,
            self.cover_media_label,
            self.image_label,
            self.cover_video_widget,
            self.cover_media_player,
            self.cover_play_pause_button,
            self.cover_text_edit,
            self.cover_vlc_player,
        )

        # Enable or disable the cover LSB spinbox based on the file type
        self.update_spinbox_state(file_path, is_cover=True)

    def handle_stego_file_drop(self, file_path):
        self.stego_file_path = file_path
        self.display_media(
            file_path,
            self.stego_media_label,
            self.image_label_stego,
            self.stego_video_widget,
            self.stego_media_player,
            self.stego_play_pause_button,
            self.stego_text_edit,
            self.stego_vlc_player,
        )

        # Enable or disable the stego LSB spinbox based on the file type
        self.update_spinbox_state(file_path, is_cover=False)

    def update_spinbox_state(self, file_path, is_cover):
        # Enable or disable the appropriate LSB spinbox based on the file type
        if file_path.lower().endswith(".txt"):
            if is_cover:
                self.lsb_spinbox.setEnabled(False)
            else:
                self.lsb_spinbox2.setEnabled(False)
        else:
            if is_cover:
                self.lsb_spinbox.setEnabled(True)
            else:
                self.lsb_spinbox2.setEnabled(True)

    def handle_payload_drop(self, file_path):
        with open(file_path, "r") as f:
            self.payload_text = f.read()
        self.text_label.clear()
        self.text_label.setText(f"Selected text file: {os.path.basename(file_path)}")

    def save_stego_file(self):
        if self.cover_file_path and self.payload_text:
            bits_to_use = self.lsb_spinbox.value()
            steganography_algorithm = get_steganography_algorithm(self.cover_file_path)

            self.append_log_message(f"Payload: {self.payload_text}")
            self.append_log_message(f"checkcoverfilepath {self.cover_file_path}")
            available_space = steganography_algorithm.calculate_capacity(
                self.cover_file_path, bits_to_use, self.payload_text
            )
            print(f"Capacity: {available_space} bytes")
            required_space = steganography_algorithm.calculate_required_space(
                self.payload_text, bits_to_use
            )
            print(f"Required Space: {required_space} bytes")
            if required_space > available_space:
                self.append_log_message(f"{required_space} > {available_space}")
                QMessageBox.critical(
                    self,
                    "Error",
                    "The payload is too large for the selected cover object and LSB settings.",
                )
                return

            save_path, _ = QFileDialog.getSaveFileName(
                self,
                "Save Stego File",
                "",
                "PNG Files (*.png);;BMP Files (*.bmp);;WAV Files (*.wav);;MP4 Files (*.mp4);;FFV1 Files (*.mkv);;GIF Files (*.gif);;TEXT Files (*.txt)",
            )

            if not save_path:
                return  # User canceled the save dialog

            if self.cover_file_path.lower().endswith((".mkv")):
                self.thread = QThread()
                self.worker = Worker(
                    steganography_algorithm,
                    bits_to_use,
                    self.cover_file_path,
                    self.payload_text,
                    save_path,
                )
                self.worker.moveToThread(self.thread)
                self.worker.log_message_signal.connect(self.append_log_message)
                self.thread.started.connect(self.worker.ffv1_encode_worker)
                self.worker.finished.connect(self.thread.quit)
                self.worker.finished.connect(self.worker.deleteLater)
                self.thread.finished.connect(self.thread.deleteLater)
                self.thread.start()
            else:
                if save_path.lower().endswith((".bmp", ".png")):
                    image = steganography_algorithm.encode_message(
                        self.cover_file_path, self.payload_text, bits_to_use
                    )
                    image.save(save_path)
                else:
                    byte_to_write = steganography_algorithm.encode_message(
                        self.cover_file_path, self.payload_text, bits_to_use
                    )

                    with open(save_path, "wb") as f:
                        f.write(byte_to_write)
                        f.close()

                self.statusBar().showMessage("Stego file saved successfully!", 5000)
        else:
            self.statusBar().showMessage(
                "Please load both a cover file and a text file", 5000
            )

    def decode_message(self):
        if self.stego_file_path:
            bits_to_use = self.lsb_spinbox2.value()
            steganography_algorithm = get_steganography_algorithm(self.stego_file_path)
            print(f"fileextensionmkv {self.stego_file_path}")
            if self.stego_file_path.lower().endswith((".mkv")):
                self.thread = QThread()
                self.worker = Worker(
                    steganography_algorithm,
                    bits_to_use,
                    self.stego_file_path,
                    None,  # No payload_text for decoding
                    None,  # No save_path for decoding
                )
                self.worker.moveToThread(self.thread)
                self.worker.decoded_message_signal.connect(self.show_decoded_message)
                self.worker.log_message_signal.connect(self.append_log_message)
                self.thread.started.connect(self.worker.ffv1_decode_worker)
                self.worker.finished.connect(self.thread.quit)
                self.worker.finished.connect(self.worker.deleteLater)
                self.thread.finished.connect(self.thread.deleteLater)
                self.thread.start()
            else:
                try:
                    decoded_message = steganography_algorithm.decode_message(
                        self.stego_file_path, bits_to_use
                    )
                    print("decoded message is", decoded_message)
                    # Remove hidden characters
                    decoded_message = "".join(
                        char for char in decoded_message if char.isprintable()
                    )
                    # Debug print to verify the type and content of the decoded message
                    print(
                        f"Type of decoded message: {type(decoded_message)}, Content: {decoded_message}"
                    )
                    if (
                        len(decoded_message) > 1000
                    ):  # Arbitrary length threshold for large messages
                        self.decoded_message_text_edit.setPlainText(decoded_message)
                        self.decoded_message_text_edit.show()
                    else:
                        QMessageBox.information(
                            self, "Decoded Message", decoded_message
                        )
                except Exception as e:
                    QMessageBox.critical(
                        self, "Error", f"Failed to decode message: {str(e)}"
                    )
        else:
            self.statusBar().showMessage("Please select a stego file to decode", 5000)

    def show_decoded_message(self, decoded_message):
        QMessageBox.information(self, "Decoded Message", decoded_message)

    # def reportProgress(self, n):
    #     print(f"Progress: {n * 10}%")


if __name__ == "__main__":
    app = QApplication(sys.argv)
    ex = SteganographyApp()
    sys.exit(app.exec_())
