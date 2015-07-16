using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter))]

/*
Grids of meshfilter instances are used to tile
gameobjects like blocks and the ground.
*/

public class MeshFilterInstance : MonoBehaviour {

	private GameObject 	m_mapOrigin;
	private Vector3		m_position; // position in map-space

	public void Init(Mesh mesh, Vector3 position) {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		
		m_position = position;
		
		m_mapOrigin = GameObject.Find("MapOrigin");
	}
	
	void Update() {
		Matrix4x4 localToWorld =
			m_mapOrigin.transform.localToWorldMatrix *
			Matrix4x4.TRS(m_position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		float mapSize = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
