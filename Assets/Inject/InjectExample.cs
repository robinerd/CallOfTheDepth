using UnityEngine;
using System.Collections;

public class InjectExample : BaseBehaviour {

    [SerializeField]
    [Inject("")]  // Get all components, any object
    ArmScript[] armScripts = null;

    [SerializeField]
    [Inject("")]
    Camera cam = null; // Get single component, any object

    [SerializeField]
    [Inject("LeftArm")] // Get object named LeftArm
    ArmScript leftArmScript = null;

    [SerializeField]
    [Inject("TheMan/LeftArm")] // Get object LeftArm, direct child of object TheMan
    GameObject leftArm = null;

    [SerializeField]
    [Inject("#Finger")] //Get all Renderers on objects with the tag "Finger"
    Renderer[] fingerRenderers = null;

    [SerializeField]
    [Inject("#MainBodyPart/BodyPartManager")] //Get all Renderers on objects with the tag "Finger"
    Health[] healthManagers = null;

    [SerializeField]
    [Inject("TheMan/NonExistingChild")] // Get object LeftArm, direct child of object TheMan
    GameObject child = null;
   
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
