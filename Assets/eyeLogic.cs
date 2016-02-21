using UnityEngine;
using System.Collections;

public class eyeLogic : MonoBehaviour {

    [SerializeField]
    private Transform m_hurtSoundEffect;

    int escapeHash = Animator.StringToHash("IdleEscape");

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

            Invoke("Die",   0.5f);
        }
            
    }
    void Die()
    {
        Debug.Log("Eye destroyed!!");
        BasicMonsterBehavior monsterScript = gameObject.GetComponentInParent<BasicMonsterBehavior>();
        monsterScript.cthulhuAnimator.SetTrigger(escapeHash);
        monsterScript.PlayerClose = false;
        monsterScript.Retreating = true;

        var flyerController = GameObject.FindObjectOfType<VRStandardAssets.Flyer.FlyerMovementController>();
        flyerController.jumpTargetBool = false;

        if (m_hurtSoundEffect)
            GameObject.Instantiate(m_hurtSoundEffect, monsterScript.transform.position, Quaternion.identity);
        else
            Debug.LogError("Error: missing sound effect prefab for m_hurtSoundEffect");

        Destroy(gameObject, 0.05f);
    }
}
