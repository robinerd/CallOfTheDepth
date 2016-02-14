using UnityEngine;
using System.Collections;

public class BasicMonsterBehavior : MonoBehaviour
{
    GameObject monster;
    public bool PlayerClose = false;
    public bool Retreating = false;
    private float retreatedMeters = 0;
    private float retreatingSpeed = 200;
    private float retreatingDistance = 800;
    public Animator cthulhuAnimator;

    int entranceHash = Animator.StringToHash("Entrance");
    int attackHash = Animator.StringToHash("Attack");
    int idleHash = Animator.StringToHash("Idle");
    int escapeHash = Animator.StringToHash("IdleEscape");

    // Use this for initialization
    void Start()
    {
        cthulhuAnimator = transform.Find("CthulhuWIP").GetComponent<Animator>();
        retreatedMeters = 0;
        //monster = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float speed;
        if(PlayerClose)
        {
            speed = 70f;
        }
        else
        {
            speed = 47f;
        }
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (Retreating)
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * retreatingSpeed);
            retreatedMeters += retreatingSpeed * Time.deltaTime;
            
            if (retreatedMeters > retreatingDistance)
            {
                retreatedMeters = 0;
                Retreating = false;
                cthulhuAnimator.SetTrigger(idleHash);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FlyerPlayership" && Retreating == false)
        {
            PlayerClose = true;
            cthulhuAnimator.SetTrigger(attackHash);
        }
            


    }
}
