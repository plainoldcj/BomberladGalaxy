using UnityEngine;
using System.Collections;

public class ViewBomb : MonoBehaviour {

    public Vector3 m_position = Vector3.zero;
    public Vector3 m_rotation = Vector3.zero;
    public Vector3 m_scale = Vector3.one;

    public GameObject m_explosionPrefab;

    private GameObject  m_syncBomb;
    private GameObject  m_mapOrigin;
    private float       m_time = 0.0f;
    private Vector2     m_lastMapPos = Vector2.zero;

    public void SetSyncBomb(GameObject syncBomb) {
        m_syncBomb = syncBomb;
    }

	// Use this for initialization
	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");

        GetComponent<Renderer>().material.EnableKeyword("ENABLE_RIM_LIGHTING");

        // forces the gameobject to be placed at a sensible position,
        // prevents spurious spawns around local player
        Update ();
	}
	
	// Update is called once per frame
	void Update () {
        m_time += Time.deltaTime;
        
        float l = Mathf.Abs(Mathf.Sin(0.5f * Mathf.PI * m_time));
        
        float t = m_time / Globals.m_bombTimeout;
        Color bodyColor = Color.black;
        Color c1 = Color.Lerp(bodyColor, Color.red, t);
        
        float s1 = 1.0f + 0.5f * t;
        
        float s = Mathf.Lerp(1.0f, s1, l);
        m_scale = new Vector3(s, s, s);
        
        GetComponent<Renderer>().materials[0].color = Color.Lerp(bodyColor, c1, l);
        
        Quaternion rotation = 
            Quaternion.AngleAxis(90.0f * Time.time, new Vector3(1.0f, 1.0f, 0.0f)) *
            Quaternion.AngleAxis(90.0f * Time.time, new Vector3(0.0f, 0.0f, 1.0f)) *
            Quaternion.AngleAxis(-90.0f, new Vector3(1.0f, 0.0f, 0.0f));

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        
        // copy position from my syncbomb and wrap it
        Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
            m_syncBomb.transform.position.x,
            m_syncBomb.transform.position.z));
        m_lastMapPos = mapPos;
        
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
            m_position, Quaternion.Euler(m_rotation) * rotation, m_scale);
        
        Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap * localTransform;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}

    void OnDestroy() {
        GameObject explosion = Instantiate(m_explosionPrefab);
        explosion.GetComponent<Explosion>().SetMapPosition(m_lastMapPos);
        Destroy(explosion, Globals.m_explosionTimeout);
    }
}
