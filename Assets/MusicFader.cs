using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour {

    public float loudestDistance;
    public float silentDistance;
    public Transform player;
    public Transform other;

    float range;

    // Use this for initialization
    void Start()
    {
        range = silentDistance - loudestDistance;
        if (range < 0.01)
        {
            Debug.LogError("Incorrect setup of action music nearDistance/farDistance. Silent Distance must be more than Loudest Distance.");
            gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        float distance = Vector3.Distance(player.position, other.position);
        float volumeFactor = Mathf.InverseLerp(silentDistance, loudestDistance, distance);
        volumeFactor = Mathf.Clamp01(volumeFactor);
        GetComponent<AudioSource>().volume = volumeFactor;
	}
}
