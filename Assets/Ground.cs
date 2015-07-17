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
	public GameObject	m_tilePrefab;
	
	private Mesh 			m_mesh;
	private GameObject[]	m_tiles;

	private void CreateMesh() {
		GameObject gameController = GameObject.FindWithTag("GameController");
		
		const int numEdgeVerts = 50;
		
		Globals globals = gameController.GetComponent<Globals>();
		float edgeLength = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		
		m_mesh = GeometryHelper.CreatePlaneXY(50, 25, edgeLength);
		m_mesh.name = "GroundMesh";
	}
	
	private void CreateTiles() {
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		float mapSize = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		
		Vector3[] offsets = {
			new Vector3(0.0f, 0.0f, 0.0f),
			new Vector3(mapSize, 0.0f, 0.0f),
			new Vector3(mapSize, mapSize, 0.0f),
			new Vector3(0.0f, mapSize, 0.0f),
			new Vector3(-mapSize, mapSize, 0.0f),
			new Vector3(-mapSize, 0.0f, 0.0f),
			new Vector3(-mapSize, -mapSize, 0.0f),
			new Vector3(0.0f, -mapSize, 0.0f),
			new Vector3(mapSize, -mapSize, 0.0f)
		};
		int numTiles = 9;
		m_tiles = new GameObject[numTiles];
		for(int i = 0; i < numTiles; ++i) {
			GameObject tile = GameObject.Instantiate(m_tilePrefab);
			tile.transform.parent = this.transform;
			tile.name = "GroundTile" + i;
			MeshFilterInstance meshFilterIns = tile.GetComponent<MeshFilterInstance>();
			meshFilterIns.Init(m_mesh, transform.position + offsets[i]);
		}
	}

	// Use this for initialization
	void Start () {
		CreateMesh();
		CreateTiles();
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
