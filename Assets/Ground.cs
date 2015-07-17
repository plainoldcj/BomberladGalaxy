using UnityEngine;
using System.Collections;

/*
The ground of the level is a simple, highly tessellated
plane.
The ground creates and owns the ground mesh.
Also, the ground creates a grid of nine ground tiles.
Each ground tile draws the ground mesh at a different offset.

TODO: maybe there is a way to clip hidden ground tiles
*/

public class Ground : MonoBehaviour {
	
	public GameObject 	m_mapOrigin;
	
	private Mesh 			m_mesh;

	private void CreateMesh() {
		GameObject gameController = GameObject.FindWithTag("GameController");
		
		const int numEdgeVerts = 50;
		
		Globals globals = gameController.GetComponent<Globals>();
		float edgeLength = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		
		m_mesh = GeometryHelper.CreatePlaneXY(50, 25, edgeLength);
		m_mesh.name = "GroundMesh";
	}

	// Use this for initialization
	void Start () {
		m_mapOrigin = GameObject.Find("MapOrigin");
	
		CreateMesh();
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = m_mesh;
	}
	
	// Update is called once per frame
	void Update () {
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		float mapSize = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		
		// this moves the ground map so that it initially fills the entire mapping domain
		Matrix4x4 offset = Matrix4x4.TRS(
			new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));
		
		Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset;
		
		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);	
	}
}
