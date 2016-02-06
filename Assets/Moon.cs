using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
    public GameObject FishText;
    public GameObject Hunter;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "dolphine")
        {
            var text = Instantiate(FishText, transform.position, Quaternion.identity);

            var hunterBody = coll.GetComponent<Rigidbody2D>();
            var hunterTransfrom = coll.GetComponent<Transform>();

            Time.timeScale = 0;
        }

    }
}
