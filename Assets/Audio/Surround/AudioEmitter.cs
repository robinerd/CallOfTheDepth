using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The audio emitter class functions as sources in which sounds are played from.
/// If 3D is enabled for the emitter, then it will also do additional 3D processing based on the position of the emitter and the audio receiver.
/// </summary>
public class AudioEmitter : MonoBehaviour 
{
    /// <summary>
    /// The filename of the .wav file this emitter is using. These files can be found in the Assets/Audio/ folder.
    /// </summary>
    public string audioDataFileName;

    /// <summary>
    /// The sound volume of this emitter. A volume greater than 1 may cause clipping on the emitted sound.
    /// </summary>
    public float soundVolume = 1.0f;

    /// <summary>
    /// Indicates whether this emitter is emitting a sound or not.
    /// This variable is required since the inherited variable 'enabled' is not accessible from the audio thread.
    /// </summary>
    [HideInInspector]
    public bool isEnabled = false;

    /// <summary>
    /// If this source is going to play a 3D sound, then it will also do some 3D processing and stereo panning depending on where the player is.
    /// </summary>
    public bool is3DSound = false;

    /// <summary>
    /// This class is basically a wrapper for 3D sound properties, if this is enabled for the emitter.
    /// </summary>
    [Serializable]
    public class SoundSettings3D
    {
        /// <summary>
        /// The play radius is what will affect the sound if it's in 3D. When the player is within the radius, the sound will be activated and played.
        /// </summary>
        public float playRadius = 10.0f;
        /// <summary>
        /// The doppler level decides how much the frequency will change. For no doppler effect at all this variable should be set to 0.
        /// Default value is 1.
        /// </summary>
        public float dopplerLevel = 1.0f;

        public float soundSpeed = 340.0f;
		
		public float panningStrength = 0.6f;
		
		public float frontBoostStrength = 0.2f;
		
		public bool binauralSurround = true;
    }
    public SoundSettings3D soundSettings3D;

    /// <summary>
    /// The audio data this emitter will play.
    /// </summary>
    private AudioData audioData;

    /// <summary>
    /// Factor to properly adjust the rate of which samples are stepped through when filling the buffer.
    /// This factor is always audioData.Frequency / AudioSettings.outputSampleRate.
    /// </summary>
    private float outputRateFactor;

    /// <summary>
    /// The frequency factor is similar to the outputRateFactor, only that it is dynamically changing during runtime.
    /// It is used for doppler effect calculation, for instance.
    /// </summary>
    private float frequencyFactor = 1.0f;

    /// <summary>
    /// If the emitter has been dynamically created in the game (and not initialized by the Audio Engine in the very beginning) this is set to true.
    /// </summary>
    private bool isCreatedDynamically;
	
	private int lastProcessSampleIndex = 0;
	
    /// <summary>
    /// Controls the panning on the left channel.
    /// </summary>
    private float leftPan = 1.0f;
    /// <summary>
    /// Controls the panning on the right channel.
    /// </summary>
    private float rightPan = 1.0f;
	
	private float extrapolationTime = 0.01f;
    private float delayL1 = 0.0f;
    private float delayR1 = 0.0f;
    private float delayL2 = 0.0f;
    private float delayR2 = 0.0f;

    // Velocity is calculated as an average over several frames, and affects the doppler effect
    private const int velocityAverageFrames = 3;
    private Queue<Vector3> velocityHistory = new Queue<Vector3>();
    private Vector3 lastPosition;
    private Vector3 velocity;
    /// <summary>
    /// Gets the max amplitude this emitter can emit by default based on the audio data.
    /// </summary>
    public float MaxAmplitude
    {
        get { return audioData.MaxAmplitude; }
    }

    /// <summary>
    /// Initializes this emitter. This is either called from the audio engine if the emitter is static, or from a game object in the world if it's spawned dynamically.
    /// </summary>
    public void Initialize(bool dynamic)
    {
        if (!string.IsNullOrEmpty(audioDataFileName))
        {
            audioData = AudioBank.GetAudioData(audioDataFileName);
            outputRateFactor = (float)audioData.Frequency / AudioSettings.outputSampleRate;
            isCreatedDynamically = dynamic;

            if (is3DSound)
            {
                leftPan = 0.0f;
                rightPan = 0.0f;
            }
        }
    }

    /// <summary>
    /// Callback inherited from MonoBehaviour. Triggered when this behaviour is enabled.
    /// </summary>
    void OnEnable()
    {
        if (isCreatedDynamically)
        {
            lock (AudioEngine.DynamicEmitterLock)
            {
                AudioEngine.dynamicEmitters.Add(this);
            }
        }
        
        isEnabled = true;
    }

    /// <summary>
    /// Callback inherited from MonoBehaviour. Triggered when this behaviour is disabled.
    /// </summary>
    void OnDisable()
    {
        isEnabled = false;
    }

    /// <summary>
    /// Callback inherited from MonoBehaviour. It is required that this exists in order to OnEnable and OnDisable to work.
    /// </summary>
    void Start() {
        lastPosition = transform.position;
    }
	
	float speed = 10;
	
    /// <summary>
    /// Callback inherited from MonoBehaviour. Used to calculate some internal properties needed by the sound source.
    /// </summary>
    void Update()
    {
		//DEBUG
		//transform.Translate(speed * Time.deltaTime, 0, 0);
		if(transform.localPosition.x > 5)
		{
			speed = 10;
		}
		else if(transform.localPosition.x < -5)
		{
			speed = -10;
		}
		
        velocityHistory.Enqueue((transform.position - lastPosition) / Time.deltaTime);
        lastPosition = transform.position;

        if (velocityHistory.Count > velocityAverageFrames)
            velocityHistory.Dequeue();

        Vector3 sum = Vector3.zero;
        foreach (Vector3 vel in velocityHistory)
        {
            sum += vel;
        }
        velocity = sum / velocityHistory.Count;
    }

    /// <summary>
    /// Callback invoked when the game object this emitter belongs to is destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (isCreatedDynamically)
        {
            lock (AudioEngine.DynamicEmitterLock)
            {
                AudioEngine.dynamicEmitters.Remove(this);
            }
        }
    }

    /// <summary>
    /// Processes sound which is emitting with 3D enabled.
    /// Calculates stereo panning, depth, falloff and doppler effect (if enabled).
    /// </summary>
    /// <param name="audioReceiver">The receiver (the one and only).</param>
    public void Process3D(AudioReceiver audioReceiver)
    {
		lastProcessSampleIndex = sampleIndex;
		
        float distance = Vector3.Distance(audioReceiver.transform.position, transform.position);
        if (distance <= soundSettings3D.playRadius)
        {
            // Calculate a panning for when the emitter is in front or behind the audio receiver.
            // The panning will go between 0.5 and 1.0, applied on both the channels.
            Vector3 forward = audioReceiver.transform.forward;
            Vector3 toOther = (transform.position - audioReceiver.transform.position).normalized;
            float panningFrontBack = (Vector3.Dot(forward, toOther) + 1.5f) / 2.5f;
			panningFrontBack = soundSettings3D.frontBoostStrength * panningFrontBack + (1 - soundSettings3D.frontBoostStrength);
            
            // Calculate a panning for left and right.
            Vector3 right = audioReceiver.transform.right;
            float panningLeftRight = Vector3.Dot(right, toOther);
            // Add the constant power factor to the output channels.
            // This in turn is also scaled, going between 0.2 and 1.0.
            float multiplier = Mathf.Sqrt(2) / 2;
            float angle = panningLeftRight * 0.5f;
            leftPan = multiplier * (Mathf.Cos(angle) - Mathf.Sin(angle));
            rightPan = multiplier * (Mathf.Cos(angle) + Mathf.Sin(angle));
			leftPan = soundSettings3D.panningStrength * leftPan + (1 - soundSettings3D.panningStrength);
			rightPan = soundSettings3D.panningStrength * rightPan + (1 - soundSettings3D.panningStrength);

            // Apply the front and back panning.
            leftPan *= panningFrontBack;
            rightPan *= panningFrontBack;

            // Further more, multiply the result by a falloff, currently being Min(1 - log(10 * distance / playRadius), 1), ranged between 0 and 1.
            multiplier = Mathf.Min(1 - Mathf.Log10(10 * distance / soundSettings3D.playRadius), 1.0f);
            leftPan *= multiplier;
            rightPan *= multiplier;

            // Doppler effect will also be taken into consideration (if doppler level is set to greater than zero).
            Vector3 distVector = transform.position - audioReceiver.transform.position;
            float vrr = Vector3.Dot(audioReceiver.Velocity * soundSettings3D.dopplerLevel, distVector) / distance; // The relative velocity of the receiver.
            float vsr = Vector3.Dot(velocity * soundSettings3D.dopplerLevel, distVector) / distance;               // The relative velocity of the source.
            float newFrequency = (soundSettings3D.soundSpeed + vrr) / (soundSettings3D.soundSpeed + vsr) * audioData.Frequency;
            frequencyFactor = newFrequency / audioData.Frequency;
			
			//calculate delays
            Vector3 earOffset = audioReceiver.transform.right * audioReceiver.transform.localScale.x;
            float distanceL1 = Vector3.Distance(transform.position, audioReceiver.transform.position - earOffset);
            float distanceR1 = Vector3.Distance(transform.position, audioReceiver.transform.position + earOffset);
            delayL1 = distanceL1 / soundSettings3D.soundSpeed;
            delayR1 = distanceR1 / soundSettings3D.soundSpeed;
			
			//calculate delays a while later for extrapolation (defined by extrapolationTime)
            float distanceL2 = Vector3.Distance(transform.position + velocity*0.01f, audioReceiver.transform.position - earOffset);
            float distanceR2 = Vector3.Distance(transform.position + velocity*0.01f, audioReceiver.transform.position + earOffset);
            delayL2 = distanceL2 / soundSettings3D.soundSpeed;
            delayR2 = distanceR2 / soundSettings3D.soundSpeed;
			Debug.Log(distanceL2 - distanceL1);
        }
        else
        {
            leftPan = 0.0f;
            rightPan = 0.0f;
            frequencyFactor = 1.0f;
            delayL1 = 0;
            delayR1 = 0;
            delayL2 = 0;
            delayR2 = 0;
        }
    }

    /// <summary>
    /// Iterates through the audio data samples.
    /// </summary>
    private int     sampleIndex         = 0;
    /// <summary>
    /// Prevents rounding errors and some weird behaviour that can occur when stepping through the samples.
    /// </summary>
    private float   sampleFactorStep    = 0.0f;
    
	private int lastSampleIndex = 0;
	private float lastOffsetL = 0;
	private float lastOffsetR = 0;
	
    /// <summary>
    /// This function is called by the AudioManager. It will fill the output buffer with data from this emitter, while stepping through the audio data. 
    /// </summary>
    /// <param name="data">The data buffer to be filled.</param>
    /// <param name="channels">Amount of output channels.</param>
    /// <param name="maxVolume">The max amplitude this emitter may play.</param>
    public void GetData(float[] data, int channels, float maxVolume)
    {
        float scaleFactor = (maxVolume * soundVolume) / audioData.MaxAmplitude;
		
		float offsetDeltaL = 0;
		float offsetDeltaR = 0;
		float offsetL = 0;
		float offsetR = 0;
		
		if(soundSettings3D.binauralSurround && channels >= 2){
			//delay in samples
			offsetL = - (delayL1 * audioData.Frequency);
			offsetR = - (delayR1 * audioData.Frequency);
			/*
			if(sampleIndex > lastSampleIndex)
			{
				offsetDeltaL = (offsetL - lastOffsetL) / (sampleIndex - lastSampleIndex);
				Debug.Log(offsetDeltaL);
				offsetDeltaR = (offsetR - lastOffsetR) / (sampleIndex - lastSampleIndex);
			}*/
			//lastSampleIndex = sampleIndex;
			
			//delay in samples a while later (defined by extrapolationTime)
			float offsetL2 = - (delayL2 * audioData.Frequency);
			float offsetR2 = - (delayR2 * audioData.Frequency);
			
			//delay change to add per elapsed sample (used for extrapolation)
			offsetDeltaL = (float)(offsetL2 - offsetL) / (audioData.Frequency*extrapolationTime);
			offsetDeltaR = (float)(offsetR2 - offsetR) / (audioData.Frequency*extrapolationTime);
		}
		//Debug.Log(offsetDeltaL);
        for (int i = 0; i < data.Length; i += channels)
        {
            //Output to left speaker
			int steps = sampleIndex - lastProcessSampleIndex;
			int	sampleIndexL = ((int)(sampleIndex + offsetL + offsetDeltaL * steps) + audioData.Samples) % audioData.Samples;
			int sampleIndexR = ((int)(sampleIndex + offsetR + offsetDeltaR * steps) + audioData.Samples) % audioData.Samples;
			
			data[i] 	= audioData.GetSample(sampleIndexL, 0) * leftPan * scaleFactor;
			data[i+1]	= audioData.GetSample(sampleIndexR, 0) * rightPan * scaleFactor;
			
            // Currently only supports mono and stereo audio clips. Any other sound clip with other extra channels will result in those channels being ignored.
            //if (audioData.Channels > 1)
            //    data[i + 1] = audioData.GetSample(sampleIndexR, 1) * rightPan * scaleFactor;
            //else

            // Constrain the stepping. Without it, everything goes wrong.
            sampleFactorStep += outputRateFactor * frequencyFactor;
            while (sampleFactorStep >= 1.0f)
            {
                sampleIndex++;
                sampleFactorStep -= 1.0f;
            }
            while (sampleFactorStep <= -1.0f)
            {
                sampleIndex--;
                sampleFactorStep += 1.0f;
            }

            // Constrain the sample index value to not move outside of the audio data bounds.
            while (sampleIndex >= audioData.Samples)
            {
                sampleIndex -= audioData.Samples;
            }
            while (sampleIndex < 0)
            {
                sampleIndex += audioData.Samples;
            }
        }
		
		lastOffsetL = offsetL;
		lastOffsetR = offsetR;
    }

    /// <summary>
    /// Callback inherited from MonoBehaviour. Used to draw gizmos for the emitter when running the project from inside the editor. Very handy while debugging.
    /// </summary>
    void OnDrawGizmos()
    {
        if (!string.IsNullOrEmpty(audioDataFileName) && base.enabled)
        {
            if (is3DSound)
            {
                if (soundVolume > 0)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, soundSettings3D.playRadius);
            }
            Gizmos.DrawIcon(transform.position, "soundicon.png", true);
        }
    }
}
