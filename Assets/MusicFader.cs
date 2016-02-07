using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour {

    public float nearDistance;
    public float farDistance;
    public Transform nearObject;
    public Transform farObject;

    float range;

    // Use this for initialization
    void Start()
    {
        range = farDistance - nearDistance;
        if (range < 0.01)
        {
            Debug.LogError("Incorrect setup of action music nearDistance/farDistance. Too small value (or negative).");
            gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        float distance = Vector3.Distance(nearObject.position, farObject.position);
        float volumeFactor = ((farDistance - distance) / range);
        volumeFactor = Mathf.Clamp01(volumeFactor);
        GetComponent<AudioSource>().volume = volumeFactor;
	}
}
