using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// The audio bank is a container for audio data. Whenever an audio data is added to a custom audio source, the data will first be checked to see if it already is here.
/// If it is, then the data can be returned back to the source. If not, then the audio bank will attempt loading the data from the harddrive.
/// The bank does also contain a direct access to audio clip data, which is more efficient to use, rather than copying the audio data multiply times,
/// whenever the same data is used in several emitters.
/// </summary>
public class AudioBank
{
    /// <summary>
    /// Provides a dictionary (hashtable) mapping a name with an audio data.
    /// </summary>
    private static Dictionary<string, AudioData> bank = new Dictionary<string, AudioData>();

    /// <summary>
    /// Returns the audio data mapped to the input name.
    /// If the data is not found, an attempt to read it from the harddrive will be done.
    /// </summary>
    /// <param name="name">The name associated to the data.</param>
    /// <returns>Audio data, or null.</returns>
    public static AudioData GetAudioData(string name)
    {
        if (bank.ContainsKey(name))
            return bank[name];
        else
        {
            AudioData data = LoadFromFile("Assets/Audio/" + name);
            if (data != null)
                bank.Add(name, data);

            return data;
        }
    }

    /// <summary>
    /// Attempts loading an audio file with a specified filename.
    /// Will throw an exception if the file could not be read for some reason.
    /// This function does only read 16-bit .wav files with no metadata. If the file is not valid then it could lead to corrupt data,
    /// or unhandled exceptions.
    /// </summary>
    /// <param name="filename">The path to the file to be read.</param>
    /// <returns>AudioData, or null.</returns>
    public static AudioData LoadFromFile(string filename)
    {
        AudioData audioData = null;

        using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
        {
            Debug.Log("Reading wav: " + filename);

            reader.BaseStream.Seek(22, SeekOrigin.Begin);
            ushort channels = reader.ReadUInt16();
            //Debug.Log("Channels: " + channels);
            uint sampleRate = reader.ReadUInt32();
            //Debug.Log("Sample rate: " + sampleRate);
            reader.BaseStream.Seek(34, SeekOrigin.Begin);
            ushort bitsPerSample = reader.ReadUInt16();
            //Debug.Log("Bits per sample: " + bitsPerSample);
            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            uint numberOfBytes = reader.ReadUInt32();
            //Debug.Log("Number of bytes: " + numberOfBytes);
            uint numberOfSamples = numberOfBytes * 8 / bitsPerSample;
            //Debug.Log("Number of samples: " + numberOfSamples);

            float maxAmplitude = 0.0f;
            float[] data = new float[numberOfSamples * channels];
            //short[] shortData = new short[numberOfSamples * channels];
            byte[] buffer = new byte[numberOfBytes];

            if (bitsPerSample / 8 == 2)
            {

                reader.BaseStream.Seek(44, SeekOrigin.Begin);
                buffer = reader.ReadBytes((int)numberOfBytes);

                int bufferStep = 0;
                for (int i = 0; i < numberOfSamples && bufferStep < buffer.Length; i++)
                {
                    float sample = (float)BitConverter.ToInt16(buffer, bufferStep) / Int16.MaxValue;

                    float abs = Mathf.Abs(sample);
                    if (abs > maxAmplitude)
                        maxAmplitude = abs;

                    data[i] = sample;
                    bufferStep += 2;
                }
            }
            else
                Debug.LogWarning(filename + "is not a 16-bit wav.");

            audioData = new AudioData(data, (int)channels, (int)sampleRate, maxAmplitude, (int)numberOfSamples / (int)channels);
        }

        return audioData;
    }
}
