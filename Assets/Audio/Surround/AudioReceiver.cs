using UnityEngine;
using System.Collections;

/// <summary>
/// The audio receiver in itself provides no own functionality, but instead is used frequently by other classes.
/// The may only be one audio receiver instance, and it must also be attached to a specified game object through the Unity editor.
/// </summary>
public class AudioReceiver : MonoBehaviour {
    Vector3 velocity;

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    void Start() {}
}
