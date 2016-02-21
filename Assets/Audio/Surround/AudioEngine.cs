using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The audio engine is the core class for the audio system. It initializes static audio emitters and updates all emitters.
/// It also provides some configuration variables, such as output sample rate and master volume adjusting.
/// The engine is also provided a callback function which is invoked frequently (~40ms interval) when the audio buffer needs to be refilled.
/// A note on the audio system is that while there are member variables declared with a value already, these only decide what default values that are showed up in Unity.
/// Therefor, these variables may actually change to something else through the Unity editor. The default variables are only there as a basis, but they are subjected to change.
/// </summary>
public class AudioEngine : MonoBehaviour
{
    /// <summary>
    /// A list containing all static emitters (static as in being an emitter created at the beginning when the project is ran).
    /// </summary>
    private List<AudioEmitter> audioEmitters    = new List<AudioEmitter>();
    /// <summary>
    /// The one and only audio receiver.
    /// </summary>
    private AudioReceiver      audioReceiver;

    /// <summary>
    /// Locking variable to prevent threading access issues. This lock is required for the list below.
    /// Note that these two static variables are kind of breaking the structure I was thinking my system would have. I wasn't thinking that I would have 
    /// dynamically created emitters at first, so it was rather a quick hack/fix for it to work.
    /// The locking might also have lead to some stuttering, since it may cause blocking in the audio thread. Nevertheless, it should be barely noticable.
    /// But if I have had more time and wasn't all this tired today [2012-05-24], I would probably find another way to do it better.
    /// </summary>
    public static System.Object DynamicEmitterLock = new System.Object();
    /// <summary>
    /// A list containing all dynamically created emitters.
    /// </summary>
    public static List<AudioEmitter> dynamicEmitters = new List<AudioEmitter>();

    /// <summary>
    /// The master volume is the final scaling applied to the amplitude of the sound. The amplitude will never go above this value.
    /// </summary>
    public float masterVolume = 0.5f;
    
    /// <summary>
    /// Determines what output sample rate to be used. Unity default is 48000.
    /// </summary>
    public int outputSampleRate = 44100;

    /// <summary>
    /// Tells if the engine is ready to use.
    /// </summary>
    private bool readyToUse = false;

    /// <summary>
    /// Callback invoked due to inheriting from MonoBehaviour. Initializes the engine and statically placed emitters.
    /// </summary>
    void Start()
    {
        AudioSettings.outputSampleRate = outputSampleRate;

        audioReceiver = Object.FindObjectOfType(typeof(AudioReceiver)) as AudioReceiver;
        
        AudioEmitter[] emitters = Object.FindObjectsOfType(typeof(AudioEmitter)) as AudioEmitter[];
        for (int i = 0; i < emitters.Length; i++)
        {
            if (!string.IsNullOrEmpty(emitters[i].audioDataFileName))
            {
                audioEmitters.Add(emitters[i]);
                emitters[i].Initialize(false);
            }
            else
                Debug.Log("An emitter wth no specified audioDataFileName was ignored");
        }
        readyToUse = true;
    }

    /// <summary>
    /// Callback invoked frequently due to inheritance. Calls every emitter's 3d processing function, if 3d sound is enabled.
    /// </summary>
    void Update()
    {
        if (masterVolume < 0)
            masterVolume = 0.0f;

        foreach (AudioEmitter emitter in audioEmitters)
            if (emitter.is3DSound)
                emitter.Process3D(audioReceiver);

        foreach (AudioEmitter emitter in dynamicEmitters)
            if (emitter.is3DSound)
                emitter.Process3D(audioReceiver);
    }

    /// <summary>
    /// Scales the buffer data with the specified volume (amplitude).
    /// </summary>
    /// <param name="data">The buffer data to be scaled.</param>
    /// <param name="volume">The scaling factor.</param>
    private void Scale(float[] data, float volume)
    {
        for (int i = 0; i < data.Length; i++)
            data[i] *= volume;
    }

    /// <summary>
    /// Iterates through all audio emitters counting which ones that are enabled.
    /// </summary>
    /// <returns>The amount of emitters that are enabled.</returns>
    int CountEnabledSources()
    {
        int enabledSources = 0;
        foreach (AudioEmitter source in audioEmitters)
        {
            if (source.isEnabled)
                enabledSources++;
        }

        lock (DynamicEmitterLock)
        {
            foreach (AudioEmitter source in dynamicEmitters)
            {
                if (source.isEnabled)
                    enabledSources++;
            }
        }

        return enabledSources;
    }

    /// <summary>
    /// Finds the max amplitude of the audio data buffer.
    /// </summary>
    /// <param name="data">The data to look through.</param>
    /// <returns>The max amplitude.</returns>
    private float GetMaxAmplitude(float[] data)
    {
        float maxAmplitude = 0.0f;

        for (int i = 0; i < data.Length; i++)
        {
            float amplitude = Mathf.Abs(data[i]);
            if (amplitude > maxAmplitude)
                maxAmplitude = amplitude;
        }

        return maxAmplitude;
    }

    /// <summary>
    /// Variable currently only used for debugging.
    /// </summary>
    public float currentStreamMaxVolume;

    /// <summary>
    /// Callback invoked in the Unity audio thread. Provides a buffer and output channel count that should be filled with data.
    /// </summary>
    /// <param name="data">Data buffer to be filled.</param>
    /// <param name="channels">Output channel count (default being 2 channels).</param>
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!readyToUse)
            return;

        if (masterVolume < 0)
            masterVolume = 0.0f;

        int enabledSources = CountEnabledSources();
        if (enabledSources > 0)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;
                
            float[] sourceData = new float[data.Length];
            foreach (AudioEmitter source in audioEmitters)
            {
                if (source.isEnabled && source.soundVolume > 0)
                {
                    source.GetData(sourceData, channels, masterVolume);
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] += sourceData[i];
                    }
                }
            }

            lock (DynamicEmitterLock)
            {
                foreach (AudioEmitter source in dynamicEmitters)
                {
                    if (source.isEnabled && source.soundVolume > 0)
                    {
                        source.GetData(sourceData, channels, masterVolume);
                        for (int i = 0; i < data.Length; i++)
                        {
                            data[i] += sourceData[i];
                        }
                    }
                }
            }

            float amplitudeNow = GetMaxAmplitude(data);
            
            if (amplitudeNow > masterVolume)
                Scale(data, masterVolume / amplitudeNow);
        }
        currentStreamMaxVolume = GetMaxAmplitude(data);
    }
}
