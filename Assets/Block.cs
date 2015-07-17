using UnityEngine;
using System.Collections;

/*
A block is a destructible obstacle and a tile of the map.
A block is a highly-tessellated cube with no bottom faces.
For rendering purposes, a block consists of two meshes:
the mantle and the cap.
The mantle consists of the side faces of a block and the top-facing
faces form the cap.
Similiarly to the ground, both the mantle and the cap use nine tiles
to draw instances of the mesh.

Because all blocks are identical, there exist a single cap mesh referenced
by all cap tiles.
*/

public class Block : MonoBehaviour {

	public GameObject m_tilePrefab;

	private static Mesh m_capMesh = null;
	
	private static void TouchCapMesh() {
		if(null == m_capMesh) {
			Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
			m_capMesh = GeometryHelper.CreatePlaneXY(10, 5, 1.0f, 0.1f);
			m_capMesh.name = "cap";
		}
	}
	
	// TODO: code duplication, Ground.cs:CreateTiles
	private void CreateCap() {
		GameObject capGroup = new GameObject();
		capGroup.name = "cap";
		capGroup.transform.parent = this.transform;
		
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
		GameObject[] m_tiles = new GameObject[numTiles];
		for(int i = 0; i < numTiles; ++i) {
			GameObject tile = GameObject.Instantiate(m_tilePrefab);
			tile.transform.parent = capGroup.transform;
			tile.name = "CapTile" + i;
			MeshFilterInstance meshFilterIns = tile.GetComponent<MeshFilterInstance>();
			meshFilterIns.Init(m_capMesh, transform.position + offsets[i]);
		}
	}

	// Use this for initialization
	void Start () {
		TouchCapMesh();
		
		CreateCap();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
