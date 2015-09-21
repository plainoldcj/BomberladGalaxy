using UnityEngine;
using System.Collections;

public class ExplosionLight : MonoBehaviour {

    private GameObject  m_mapOrigin;
    private Vector2     m_mapPos = Vector2.zero;
    private float       m_time = 0.0f;

    public void SetMapPosition(Vector2 mapPos)
    {
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

        // animate light intensity
        float t = m_time / Globals.m_explosionTimeout;
        GetComponent<Light>().intensity = 8.0f * (1.0f - t);

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        Vector2 mapPos = m_mapPos;
        
        // this moves the ground map so that it initially fills the entire mapping domain
        Matrix4x4 offset = Matrix4x4.TRS(
            new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, 1.0f));

        // slightly move light above ground
        float height = 3.0f;
        Vector3 localOffset = new Vector3(0.5f, 0.0f, height);
        Matrix4x4 localToMap = Matrix4x4.TRS(
            localOffset + new Vector3(mapPos.x, mapPos.y, 0.5f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, -1.0f));
        
        Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;

        // map light on sphere, consistent with shader mapping

        Vector3 p = Globals.VS_MapOnSphere(Globals.VS_Warp(localToWorld.MultiplyPoint(Vector3.zero)));

        transform.position = p;
	}
}
