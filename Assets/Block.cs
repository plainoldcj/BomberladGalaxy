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

There are four types of blocks.
The first type is a wooden block that gets destroyed on first hit.
There are three types of stone blocks (S0, S1, S2).
When Si, i > 0, gets hit by a bomb, it is replaced by S(i-1).
When S0 gets hit, it gets destroyed.
*/

public class Block : MonoBehaviour {

	public GameObject	m_blockPiecePrefab;

	public Texture2D 	m_woodTexture;
	public Texture2D 	m_stoneTexture;
	
	public enum Type {
		Unknown,
		Wood,
		Stone0,
		Stone1,
		Stone2
	}
	
	private GameObject 	m_mapOrigin;
	private Type		m_type = Type.Unknown;
	
	private Vector2		m_tilePos = Vector2.zero; // integer tile coordinates
	
	private GameObject	m_cap;
	private GameObject	m_mantle;

	private static Mesh m_capMesh = null;
	private static Mesh m_mantleMesh = null;
	
	private static Mesh CreateMantleMesh() {
		// create four planes, one for each side, and combine them
		
		string[] sideNames = { "front", "right", "back", "left" };
		
		CombineInstance[] cins = new CombineInstance[4];
		for(int i = 0; i < 4; ++i) {
			cins[i].mesh = GeometryHelper.CreatePlaneXY(
				2 * Globals.m_blockDetail, Globals.m_blockDetail,
				1.0f,
				Matrix4x4.identity);
			cins[i].mesh.name = sideNames[i];
		}
		
		// make blocks a little bit higher so that they intersect the ground
		// and hide gaps resulting from different tessellations
		Vector3 blockScale = new Vector3(Globals.m_tileEdgeLength, Globals.m_blockHeight * 1.2f, 1.0f);
		
		// front
		cins[0].transform = Matrix4x4.TRS(
			new Vector3(0.0f, -Globals.m_tileEdgeLength, Globals.m_blockHeight),
			Quaternion.Euler(90.0f, 0.0f, 0.0f),
			blockScale);
			
		// right
		cins[1].transform = Matrix4x4.TRS(
			new Vector3(Globals.m_tileEdgeLength, -Globals.m_tileEdgeLength, Globals.m_blockHeight),
			Quaternion.Euler(0.0f, 90.0f, 90.0f),
			blockScale);
		
		// back
		cins[2].transform = Matrix4x4.TRS(
			new Vector3(Globals.m_tileEdgeLength, 0.0f, Globals.m_blockHeight),
			Quaternion.Euler(-90.0f, 0.0f, 180.0f),
			blockScale);
		
		// left
		cins[3].transform = Matrix4x4.TRS(
			new Vector3(0.0f, 0.0f, Globals.m_blockHeight),
			Quaternion.Euler(0.0f, -90.0f, -90.0f),
			blockScale);
		
		Mesh mesh = new Mesh();
		mesh.CombineMeshes(cins);
		mesh.name = "mantle";
		return mesh;
	}
	
	private static void TouchCapMesh() {
		if(null == m_capMesh) {
			Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
			Matrix4x4 distort = Matrix4x4.TRS(
				new Vector3(0.0f, 0.0f, Globals.m_blockHeight),
				Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
			m_capMesh = GeometryHelper.CreatePlaneXY(
				2 * Globals.m_blockDetail, Globals.m_blockDetail,
				Globals.m_tileEdgeLength,
				distort);
			m_capMesh.name = "cap";
		}
	}
	
	private static void TouchMantleMesh() {
		if(null == m_mantleMesh) {
			m_mantleMesh = CreateMantleMesh();
			m_mantleMesh.name = "mantle";
		}
	}
	
	// returns the position in map-space
	private Vector2 GetMapPosition() {
		return new Vector2(m_tilePos.x, -m_tilePos.y) * Globals.m_tileEdgeLength;
	}
	
	public void Init(Type type) {
		m_type = type;
		
		Texture2D texture = m_stoneTexture;
		if(Type.Wood == type) {
			texture = m_woodTexture;
		}
		
		m_mapOrigin = GameObject.Find("MapOrigin");
		
		TouchCapMesh();
		TouchMantleMesh();
		
		// create cap
		
		m_cap = Instantiate(m_blockPiecePrefab);
		m_cap.name = "cap";
		m_cap.transform.parent = this.transform;
		
		m_cap.GetComponent<MeshFilter>().mesh = m_capMesh;
		
		// create mantle
		
		m_mantle = Instantiate(m_blockPiecePrefab);
		m_mantle.name = "mantle";
		m_mantle.transform.parent = this.transform;
		
		m_mantle.GetComponent<MeshFilter>().mesh = m_mantleMesh;
		
		// set material
		
		m_cap.GetComponent<Renderer>().material.SetTexture("_DiffuseTex", texture);
		m_cap.GetComponent<Renderer>().material.SetTextureScale("_DiffuseTex", new Vector2(1.0f, 1.0f));
		
		m_mantle.GetComponent<Renderer>().material.SetTexture("_DiffuseTex", texture);
		m_mantle.GetComponent<Renderer>().material.SetTextureScale("_DiffuseTex", new Vector2(1.0f, 1.0f));
	}
	
	public void SetTilePosition(Vector2 pos) {
		m_tilePos = pos;
	}

	// Use this for initialization
	void Start () {	}
	
	void Update() { }
    
	public void SortedUpdate () {
		float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
		
		// this moves the ground map so that it initially fills the entire mapping domain
		Matrix4x4 offset = Matrix4x4.TRS(
			new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));
			
		Vector2 mapPos = GetMapPosition();
		Matrix4x4 localToMap = Matrix4x4.TRS(
			new Vector3(mapPos.x, mapPos.y, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));
		
		Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;
		
		m_cap.GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		m_cap.GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
		
		m_mantle.GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		m_mantle.GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);	
	}
}
