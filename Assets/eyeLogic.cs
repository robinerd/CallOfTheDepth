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
    void OnDestroy()
    {
        Debug.Log("Eye destroyed!!");
        BasicMonsterBehavior monsterScript = gameObject.GetComponentInParent<BasicMonsterBehavior>();
        monsterScript.PlayerClose = false;
        monsterScript.Retreating = true;
        var flyerController = GameObject.FindObjectOfType<VRStandardAssets.Flyer.FlyerMovementController>();
        flyerController.jumpTargetBool = false;
    }
}
