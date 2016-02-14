using UnityEngine;
using System.Collections;

public class InGameOnly : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnInGame(bool inGame) //TODO: trigger from state change instead.
    {
        /*foreach (Renderer r in GetComponents<Renderer>())
        {
            r.enabled = inGame;
        }

        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = inGame;
        }*/

        gameObject.SetActive(inGame);
    }
}
