using UnityEngine;
using System.Collections;

public class RealSurround : BaseBehaviour
{
    [SerializeField][Inject("")]
    private AudioListener listener;
    
    public int DistanceScale = 80000;

    int m_sampleRate = 44100; //TODO fetch from audio clip.

    int targetOffsetSamplesL = 0;
    int targetOffsetSamplesR = 0;
    int prevOffsetSamplesL = 0;
    int prevOffsetSamplesR = 0;
    float[] prevRawData = null;
    float[] newRawData = null;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        //Call in start since we create these from a prefab. They're not assigned in the scene.
        base.InjectDependencies(true, true);
    }

    // Update is called once per frame
    void Update()
    {

        float targetOffsetSecondsL = Vector3.Distance(listener.transform.position - 20*listener.transform.right, this.transform.position) / (float)DistanceScale;
        float targetOffsetSecondsR = Vector3.Distance(listener.transform.position + 20*listener.transform.right, this.transform.position) / (float)DistanceScale;
        targetOffsetSamplesL = (int)(targetOffsetSecondsL * m_sampleRate);
        targetOffsetSamplesR = (int)(targetOffsetSecondsR * m_sampleRate);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (prevRawData == null)
        {
            prevRawData = new float[data.Length];
            newRawData = new float[data.Length];
            return;
        }

        for (int i = 0; i < data.Length; i++)
        {
            newRawData[i] = data[i];
        }

        int numSamples = data.Length / channels;

        // Interpolate the delay from what it was last time to what it should be now.
        // Do this interpolation per sample over the whole buffer to avoid clicks.
        float interpolationStepL = (float)(this.targetOffsetSamplesL - this.prevOffsetSamplesL) / numSamples;
        float interpolationStepR = (float)(this.targetOffsetSamplesR - this.prevOffsetSamplesR) / numSamples;

        float interpolatedOffsetL = this.prevOffsetSamplesL;
        float interpolatedOffsetR = this.prevOffsetSamplesR;

        for (int i = 0; i < numSamples; i++)
        {
            int indexL = Mathf.Clamp(i - (int)interpolatedOffsetL, -numSamples, numSamples - 1);
            int indexR = Mathf.Clamp(i - (int)interpolatedOffsetR, -numSamples, numSamples - 1);

            float sampleL;
            if (indexL < 0)
                sampleL = prevRawData[indexL * channels + data.Length];
            else
                sampleL = newRawData[indexL * channels];

            float sampleR;
            if (indexR < 0)
                sampleR = prevRawData[indexR * channels + data.Length + 1];
            else
                sampleR = newRawData[indexR * channels + 1];

            data[i * channels] = sampleL;
            if (channels > 1)
                data[i * channels + 1] = sampleR;

            interpolatedOffsetL += interpolationStepL;
            interpolatedOffsetR += interpolationStepR;
        }

        for (int i = 0; i < data.Length; i++)
        {
            prevRawData[i] = newRawData[i];
        }

        prevOffsetSamplesL = (int) (interpolatedOffsetL + 0.5f);
        prevOffsetSamplesR = (int) (interpolatedOffsetR + 0.5f);
    }
}
