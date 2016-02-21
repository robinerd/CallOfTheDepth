using UnityEngine;
using System.Collections;

public class SpeedBasedVolume : BaseBehaviour {

    public float loudestSpeed;
    public float silentSpeed;
    public string relativeTo;

    private GameObject relativeToGameObject = null;

    bool isFirstFrame = true;
    float movAvgFactor = 0.997f;
    float maxVolume = 1.0f;
    float range;
    float speed; // game units moved per second, as a moving average

    Vector3 prevPos;
    // Use this for initialization
    void Start()
    {
        if(relativeTo != null)
        {
            relativeToGameObject = GameObject.Find(relativeTo);
        }

        maxVolume = GetComponent<AudioSource>().volume;

        range = loudestSpeed - silentSpeed;
        if (range < 0.01 && range > -0.01)
        {
            Debug.LogError("Incorrect setup of SpeedBasedVolume. Silent Speed and Loudest Speed should be different.");
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if(relativeToGameObject != null)
        {
            pos -= relativeToGameObject.transform.position;
        }

        if (!isFirstFrame)
        {
            float movedDistance = Vector3.Distance(prevPos, pos);
            speed = speed * movAvgFactor + (1 - movAvgFactor) * movedDistance / Time.fixedDeltaTime;
            float volumeFactor = Mathf.InverseLerp(silentSpeed, loudestSpeed, speed);
            volumeFactor = Mathf.Clamp01(volumeFactor);
            GetComponent<AudioSource>().volume = volumeFactor * maxVolume;
        }
        isFirstFrame = false;
        prevPos = pos;
    }
}
