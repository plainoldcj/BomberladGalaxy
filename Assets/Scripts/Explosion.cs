using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public Vector3 m_position = Vector3.zero;
    public Vector3 m_rotation = Vector3.zero;
    public Vector3 m_scale = Vector3.one;

    private GameObject  m_mapOrigin;
    private Vector2     m_mapPos = Vector2.zero;
    private float       m_time = 0.0f;

    public void SetMapPosition(Vector2 mapPos) {
        m_mapPos = mapPos;
    }

	// Use this for initialization
	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");

        // forces the gameobject to be placed at a sensible position,
        // prevents spurious spawns around local player
        Update ();
	}
	
	// Update is called once per frame
	void Update () {
        m_time += Time.deltaTime;

        float loopduration = 5.0f;
        float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
        float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float len = 1.0f / (r + g + b);
        r *= len;
        g *= len;
        b *= len;
        GetComponent<Renderer>().material.SetColor("_DisplacementCoeffs", new Color(r, g, b));

        float t = Mathf.Min(1.0f, (m_time / Globals.m_explosionFadeIn)) - 1.0f;
        Vector3 scale = m_scale * (t * t * t + 1.0f);

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        Vector2 mapPos = m_mapPos;
        
        // this moves the ground map so that it initially fills the entire mapping domain
        Matrix4x4 offset = Matrix4x4.TRS(
            new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, 1.0f));
        
        Vector3 localOffset = new Vector3(0.5f, 0.0f, 0.0f);
        Matrix4x4 localToMap = Matrix4x4.TRS(
            localOffset + new Vector3(mapPos.x, mapPos.y, 0.5f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, -1.0f));
        
        Matrix4x4 localTransform = Matrix4x4.TRS(
            m_position, Quaternion.Euler(m_rotation), scale);
        
        Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap * localTransform;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
