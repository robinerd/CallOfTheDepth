using UnityEngine;
using System.Collections;

public class TentacleSoundSpawner : BaseBehaviour {

    [SerializeField][Inject("#TentacleSoundNode")]
    private Transform[] m_soundNodes;

    [SerializeField]
    private Transform m_tentacleSwooshPrefab;

	// Use this for initialization
	void Awake () {
	    foreach (Transform soundNode in m_soundNodes)
        {
            Transform sound = GameObject.Instantiate(m_tentacleSwooshPrefab) as Transform;
            sound.parent = soundNode;
            sound.localPosition = Vector3.zero;
        }
	}
}
