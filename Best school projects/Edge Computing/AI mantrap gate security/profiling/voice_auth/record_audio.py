import pyaudio
import wave


def record_audio(filename, duration=3, rate=44100, chunk=1024, channels=1):
    audio = pyaudio.PyAudio()
    stream = audio.open(format=pyaudio.paInt16, channels=channels,
                        rate=rate, input=True,
                        frames_per_buffer=chunk)

    print("Recording...")
    frames = []

    for _ in range(0, int(rate / chunk * duration)):
        data = stream.read(chunk)
        frames.append(data)

    print("Recording finished.")

    stream.stop_stream()
    stream.close()
    audio.terminate()

    with wave.open(filename, "wb") as wf:
        wf.setnchannels(channels)
        wf.setsampwidth(audio.get_sample_size(pyaudio.paInt16))
        wf.setframerate(rate)
        wf.writeframes(b''.join(frames))