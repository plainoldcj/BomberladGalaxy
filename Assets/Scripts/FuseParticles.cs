using UnityEngine;
using System.Collections;

public class FuseParticles : MonoBehaviour {

    // local offset to viewbomb
    public Vector3 m_position = Vector3.zero;

    public Matrix4x4 localToWorld
    {
        get; set;
    }

    public void DoUpdate()
    {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld * Matrix4x4.TRS(m_position, Quaternion.identity, Vector3.one));
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
    }
	
	void Update () {
        DoUpdate();
	}
}
