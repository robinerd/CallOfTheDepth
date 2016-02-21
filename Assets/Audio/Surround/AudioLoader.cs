using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class AudioLoader : MonoBehaviour
{
    private int loadingAudioData = 0;
    private int totalAudioData = 0;
    private FileInfo[] fileInfos;

    private static Dictionary<string, AudioData> bank = new Dictionary<string, AudioData>();

    public static AudioData GetLoadedData(string name)
    {
        if (bank.ContainsKey(name))
            return bank[name];
        else
        {
            //Debug.Log("Error: " + name + " was not found in the audio bank. Attempt adding on the fly!");
            AudioData data = LoadFromFile("Assets/Audio/" + name);
            if (data != null)
                bank.Add(name, data);

            return data;
        }
    }

    void Start()
    {
        fileInfos = new DirectoryInfo("Assets/Audio/").GetFiles("*.wav");
        totalAudioData = fileInfos.Length;
        
        if (totalAudioData > 0)
            transform.GetComponent<GUIText>().text = "Loading " + (loadingAudioData + 1) + "/" + totalAudioData + ": " + fileInfos[0].Name;
        else
            transform.GetComponent<GUIText>().text = "Loading 0/0";

        pauseInterval = Time.maximumDeltaTime;
    }

    private float pauseInterval;
    private float tick = 0.0f;

    void Update()
    {
        if (tick == 0.0f)
        {
            if (loadingAudioData < totalAudioData)
            {
                AudioData data = LoadFromFile(fileInfos[loadingAudioData].FullName);

                if (data != null)
                {
                    bank.Add(fileInfos[loadingAudioData].Name, data);
                    Debug.Log(fileInfos[loadingAudioData].Name + " has been loaded.");
                }
                loadingAudioData++;
                transform.GetComponent<GUIText>().text = "Loading " + loadingAudioData + "/" + totalAudioData + ": " + fileInfos[loadingAudioData - 1].Name;
            }
            else
            {
                transform.GetComponent<GUIText>().text = "Loading Completed!";
                Application.LoadLevel("Default");
            }
        }
        tick += Time.deltaTime;
        if (tick >= pauseInterval)
            tick = 0.0f;
    }

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
            //Debug.Log(filename + " has been loaded!");
        }

        return audioData;
    }
}
