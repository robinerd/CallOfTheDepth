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

            //gameObject.transform.parent.Translate(Vector3.forward * 1000f);
            var flyerController = GameObject.Find("FlyerPlayership").GetComponent<VRStandardAssets.Flyer.FlyerMovementController>();
            flyerController.jumpTargetBool = true;
            flyerController.jumpTarget = gameObject;
            Destroy(gameObject, 0.5f);
        }
            
    }
    void Destroy()
    {
        var monsterScript = gameObject.GetComponentInParent<BasicMonsterBehavior>();
        monsterScript.Moving = false;
        monsterScript.Retreating = true;
        var flyerController = GameObject.Find("FlyerPlayership").GetComponent<VRStandardAssets.Flyer.FlyerMovementController>();
        flyerController.jumpTargetBool = false;
    }
}
