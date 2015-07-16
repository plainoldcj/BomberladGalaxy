using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter))]

/*
A ground tile is a child object of a ground and renders
one of its nine tiles.
*/

public class GroundTile : MonoBehaviour {

	private GameObject 	m_mapOrigin;
	private Vector3		m_offset;

	public void Init(Mesh mesh, Vector3 offset) {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		
		m_offset = offset;
		
		m_mapOrigin = GameObject.Find("MapOrigin");
	}
	
	void Update() {
		Matrix4x4 localToWorld =
			m_mapOrigin.transform.localToWorldMatrix *
			Matrix4x4.TRS(m_offset, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
	}
}
