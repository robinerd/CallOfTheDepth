using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {
    public GameObject FishText;
    public GameObject Hunter;

    public int fishFart = 200;
    // Use this for initialization
    void Start () {
        fishFart = 500;
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
            hunterBody.AddForce(hunterTransfrom.up * fishFart, ForceMode2D.Impulse);
            Destroy(gameObject);
        }

    }

}
