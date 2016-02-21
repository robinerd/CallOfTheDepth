using UnityEngine;
using System.Collections;

/// <summary>
/// The audio data class provides readonly properties from a loaded .wav file.
/// These are stored in the audion bank paired with a name, to allow several emitters to reference the same audio data.
/// </summary>
[System.Serializable]
public class AudioData
{
    private float[] data;
    private int     channels;
    private int     frequency;
    private int     samplesPerChannel;
    private float   maxAmplitude;
    private int     samples;

    /// <summary>
    /// Gets the sample count of this audio data.
    /// </summary>
    public int Samples
    {
        get { return samples; }
    }

    /// <summary>
    /// Gets how many channels the audio data has.
    /// </summary>
    public int Channels
    {
        get { return channels; }
    }

    /// <summary>
    /// Gets the sample rate of the audio data.
    /// </summary>
    public int Frequency
    {
        get { return frequency; }
    }

    /// <summary>
    /// Gets the proper sample data.
    /// </summary>
    /// <param name="position">A sample position index.</param>
    /// <param name="channel">What channel (mono, stereo) to take te sample data from.</param>
    /// <returns>Sample data.</returns>
    public float GetSample(int position, int channel)
    {
        return data[position * channels + channel];
    }

    /// <summary>
    /// Gets the max amplitude of the audio data.
    /// </summary>
    public float MaxAmplitude
    {
        get { return maxAmplitude; }
    }

    /// <summary>
    /// Normalizes the audio data. Currently not used, but could be useful for the future.
    /// </summary>
    private void Normalize()
    {
        float maxAmplitude = 0.0f;

        for (int i = 0; i < data.Length; i++)
        {
            float amplitude = Mathf.Abs(data[i]);
            if (amplitude > maxAmplitude)
                maxAmplitude = amplitude;
        }

        maxAmplitude = 1.0f / maxAmplitude;
        for (int i = 0; i < data.Length; i++)
            data[i] *= maxAmplitude;
    }

    /// <summary>
    /// Finds the max amplitude of the audio data and sets the corresponding max amplitude variable.
    /// Currently not used either, since finding the max amplitude is done when reading the audio data file.
    /// </summary>
    private void FindMaxAmplitude()
    {
        for (int i = 0; i < data.Length; i++)
        {
            float amplitude = Mathf.Abs(data[i]);
            if (amplitude > maxAmplitude)
                maxAmplitude = amplitude;
        }
    }

    /// <summary>
    /// Creates an instance of this class.
    /// </summary>
    /// <param name="data">Data containing interleaved samples.</param>
    /// <param name="channels">The amount of channels (mono or stereo supported).</param>
    /// <param name="frequency">The sample rate of audio data.</param>
    /// <param name="maxAmplitude">Max amplitude of the audio data</param>
    /// <param name="samples">The amount of samples.</param>
    public AudioData(float[] data, int channels, int frequency, float maxAmplitude, int samples)
    {
        this.data = data;
        this.channels = channels;
        this.frequency = frequency;
        this.maxAmplitude = maxAmplitude;
        this.samples = samples;
    }
}
