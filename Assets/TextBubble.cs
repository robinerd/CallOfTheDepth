using UnityEngine;
using System.Collections;

public class TextBubble : MonoBehaviour {
    public int lifetime = 2;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifetime);
    }
	
}
