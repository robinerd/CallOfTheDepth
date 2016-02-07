using UnityEngine;
using System.Collections;

public class BasicMonsterBehavior : MonoBehaviour
{
    GameObject monster;
    bool moving = false;
    public float speed = 1;
    // Use this for initialization
    void Start()
    {
        //monster = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 100f);
            //gameObject.transform.Translate(0, 0, Speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FlyerPlayership")
            moving = true;
    }
}
