using UnityEngine;
using System.Collections;

public class TrailRendererBooster : MonoBehaviour {

    [SerializeField] private Material m_TrailMaterial;
    [SerializeField] private float m_StandardAlpha;
    [SerializeField] private float m_BoostedAlpha;
    [SerializeField] private float m_BoostDuration;

    Color col;

    // Use this for initialization
    void Start () {
        col = m_TrailMaterial.GetColor("_TintColor");
        col.a = m_StandardAlpha;
    }

    public void Boost()
    {
        col.a = m_BoostedAlpha;
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (col.a > m_StandardAlpha + 0.01)
        {
            col.a -= (m_BoostedAlpha - m_StandardAlpha) / m_BoostDuration * Time.fixedDeltaTime;
        }
        m_TrailMaterial.SetColor("_TintColor", col);
    }
}
