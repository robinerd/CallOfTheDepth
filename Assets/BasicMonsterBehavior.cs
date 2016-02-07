using UnityEngine;
using System.Collections;

public class BasicMonsterBehavior : MonoBehaviour
{
    GameObject monster;
    public bool Moving = false;
    public float speed = 1;
    public bool Retreating = false;
    private float retreatedMeters = 0;
    private float retreatingSpeed = 500;
    // Use this for initialization
    void Start()
    {
        //monster = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Moving)
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 80f);
            //gameObject.transform.Translate(0, 0, Speed);
        }
        if(Retreating)
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * retreatingSpeed);
            retreatedMeters += retreatingSpeed;
            if (retreatedMeters > 100*retreatingSpeed)
            {
                retreatedMeters = 0;
                Retreating = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FlyerPlayership")
            Moving = true;
    }
}
