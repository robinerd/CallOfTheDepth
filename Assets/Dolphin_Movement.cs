using UnityEngine;
using System.Collections;

public class Dolphin_Movement : MonoBehaviour {
    public Rigidbody2D rb2d;
    public int rotateForce;
    private float smoothedModelBend = 0;

	// Use this for initialization
	void Start () {
        rotateForce = 100;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        float modelBend = 0;
        float speedFront = Vector2.Dot(rb2d.velocity, transform.up);
        float speedRight = Vector2.Dot(rb2d.velocity, transform.right);

        float turningFactor = 0.1f;
        if (transform.position.y < 26)
        {
            turningFactor = 0.4f;
            rb2d.AddRelativeForce(Vector2.up * 7, ForceMode2D.Impulse);
        }

        float turningForce = -(speedRight * Mathf.Abs(speedRight));
        rb2d.AddRelativeForce(Vector2.right * turningForce * turningFactor, ForceMode2D.Impulse);
        if (Input.GetKey(KeyCode.A))
        {
            rb2d.AddTorque(rotateForce, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb2d.AddTorque(-rotateForce, ForceMode2D.Impulse);
        }

        if (speedRight > 0.01f) {
            modelBend = Mathf.Clamp01(speedRight / (3+Mathf.Abs(speedFront* 0.5f)) ) * 0.5f;
        }

        smoothedModelBend = 0.9f * smoothedModelBend + 0.1f * modelBend;
        this.GetComponent<Animator>().SetTime(smoothedModelBend);
    }
}
