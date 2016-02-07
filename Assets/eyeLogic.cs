using UnityEngine;
using System.Collections;

public class eyeLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FlyerLaser(Clone)")
        {
            var monsterScript = gameObject.GetComponentInParent<BasicMonsterBehavior>();
            //gameObject.transform.parent.Translate(Vector3.forward * 1000f);
            monsterScript.Moving = false;
            monsterScript.Retreating = true;
            Destroy(gameObject);

            //TODO: make properly with HP and event system.
            GameObject.Find("ActionMusic").GetComponent<AudioSource>().volume += 0.15f;
        }
            
    }
}
