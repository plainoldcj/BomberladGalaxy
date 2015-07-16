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

	private static Mesh CreateMesh() {
		const int numEdgeVerts = 50;
		const int numVerts = numEdgeVerts * numEdgeVerts;
		
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		float edgeLength = globals.m_tileEdgeLength * globals.m_numTilesPerEdge;
		
		float ds = edgeLength / (numEdgeVerts - 1);
		float dt = 1.0f / (numEdgeVerts - 1);
		
		// create vertices
		
		// row-major grid layout
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] texCoords = new Vector2[numVerts];
		
		Vector3 pos = Vector3.zero;
		Vector2 tcoord = Vector2.zero;
		for(int i = 0; i < numEdgeVerts; ++i) {
			for(int j = 0; j < numEdgeVerts; ++j) {
				int vidx = numEdgeVerts * i + j;
				vertices[vidx] = pos;
				texCoords[vidx] = tcoord;
				pos.x += ds;
				tcoord.x += dt;
			}
			pos.x = 0.0f;
			pos.y -= ds;
			tcoord.x = 0.0f;
			tcoord.y += dt;
		}
		
		// create indices
		
		const int numTris = 2 * (numEdgeVerts - 1) * (numEdgeVerts - 1);
		const int numIndices = 3 * numTris;
		
		int[] indices = new int[numIndices];
		
		int cur = 0;
		for(int i = 0; i < numEdgeVerts - 1; ++i) {
			for(int j = 0; j < numEdgeVerts - 1; ++j) {
				indices[cur + 0] = numEdgeVerts * i + j;
				indices[cur + 1] = numEdgeVerts * i + (j + 1);
				indices[cur + 2] = numEdgeVerts * (i + 1) + j;
				
				indices[cur + 3] = numEdgeVerts * (i + 1) + j;
				indices[cur + 4] = numEdgeVerts * i + (j + 1);
				indices[cur + 5] = numEdgeVerts * (i + 1) + (j + 1);
				
				cur += 6;
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.name = "GroundMesh";
		mesh.vertices = vertices;
		mesh.uv = texCoords;
		mesh.triangles = indices;
		
		// logging
		Debug.Log("ground mesh: " + vertices.Length + " vertices, " + indices.Length + " indices");
		
		return mesh;
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
			GroundTile groundTile = tile.GetComponent<GroundTile>();
			groundTile.Init(m_mesh, transform.position + offsets[i]);
		}
	}

	// Use this for initialization
	void Start () {
		m_mesh = CreateMesh();
		CreateTiles();
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
