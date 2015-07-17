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

	public Texture2D m_woodTexture;
	public Texture2D m_stoneTexture;
	
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

	private static Mesh m_capMesh = null;
	
	private static void TouchCapMesh() {
		if(null == m_capMesh) {
			Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
			m_capMesh = GeometryHelper.CreatePlaneXY(10, 5, 1.0f, 0.5f);
			m_capMesh.name = "cap";
		}
	}
	
	// returns the position in map-space
	private Vector2 GetMapPosition() {
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		float tileSize = globals.m_tileEdgeLength; // TODO: cache this value
		
		return new Vector2(m_tilePos.x, -m_tilePos.y) * tileSize;
	}
	
	public void Init(Type type) {
		m_type = type;
		
		Texture2D texture = m_stoneTexture;
		if(Type.Wood == type) {
			texture = m_woodTexture;
		}
		GetComponent<Renderer>().material.SetTexture("_DiffuseTex", texture);
		GetComponent<Renderer>().material.SetTextureScale("_DiffuseTex", new Vector2(1.0f, 1.0f));
	}
	
	public void SetTilePosition(Vector2 pos) {
		m_tilePos = pos;
	}

	// Use this for initialization
	void Start () {
		m_mapOrigin = GameObject.Find("MapOrigin");
	
		TouchCapMesh();
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = m_capMesh;
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
			
		Vector2 mapPos = GetMapPosition();
		Matrix4x4 localToMap = Matrix4x4.TRS(
			new Vector3(mapPos.x, mapPos.y, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));
		
		Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;
		
		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);	
	}
}
