using UnityEngine;
using System.Collections;

public class SpeedBasedVolume : BaseBehaviour {

    public float loudestSpeed;
    public float silentSpeed;

    //==============================================================================
    //Injected dependencies
    [SerializeField][Inject("CthulhuWIP")]
    private Transform relativeTo = null;
    //==============================================================================

    bool isFirstFrame = true;
    float movAvgFactor = 0.997f;
    float range;
    float speed; // game units moved per second, as a moving average

    Vector3 prevPos;
    // Use this for initialization
    void Start()
    {
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
        if(relativeTo != null)
        {
            pos -= relativeTo.position;
        }

        if (!isFirstFrame)
        {
            float movedDistance = Vector3.Distance(prevPos, pos);
            speed = speed * movAvgFactor + (1 - movAvgFactor) * movedDistance / Time.fixedDeltaTime;
            float volumeFactor = Mathf.InverseLerp(silentSpeed, loudestSpeed, speed);
            volumeFactor = Mathf.Clamp01(volumeFactor);
            GetComponent<AudioSource>().volume = volumeFactor;
        }
        isFirstFrame = false;
        prevPos = pos;
    }
}
