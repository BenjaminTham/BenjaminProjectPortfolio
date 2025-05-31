export const convertToWav = async (blob: Blob) => {
  const audioContext = new (window.AudioContext)();
  const arrayBuffer = await blob.arrayBuffer();

  // Decode audio data correctly
  const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
  const sampleRate = audioBuffer.sampleRate;
  const numChannels = audioBuffer.numberOfChannels;
  const length = audioBuffer.length * numChannels * 2 + 44; // WAV size calculation

  const buffer = new ArrayBuffer(length);
  const view = new DataView(buffer);

  // WAV Header
  writeString(view, 0, "RIFF");
  view.setUint32(4, length - 8, true);
  writeString(view, 8, "WAVE");

  writeString(view, 12, "fmt ");
  view.setUint32(16, 16, true); // Subchunk1Size
  view.setUint16(20, 1, true);  // AudioFormat (PCM = 1)
  view.setUint16(22, numChannels, true);
  view.setUint32(24, sampleRate, true);
  view.setUint32(28, sampleRate * numChannels * 2, true); // ByteRate
  view.setUint16(32, numChannels * 2, true); // BlockAlign
  view.setUint16(34, 16, true); // BitsPerSample

  writeString(view, 36, "data");
  view.setUint32(40, length - 44, true);

  // Write PCM samples
  let offset = 44;
  for (let i = 0; i < audioBuffer.numberOfChannels; i++) {
    const channelData = audioBuffer.getChannelData(i);
    for (let j = 0; j < channelData.length; j++) {
      const sample = Math.max(-1, Math.min(1, channelData[j])); // Clip to -1..1
      view.setInt16(offset, sample * 0x7FFF, true);
      offset += 2;
    }
  }

  return new Blob([buffer], { type: "audio/wav" });
};

const writeString = (view: DataView<ArrayBuffer>, offset: number, string: string) => {
  for (let i = 0; i < string.length; i++) {
    view.setUint8(offset + i, string.charCodeAt(i));
  }
};