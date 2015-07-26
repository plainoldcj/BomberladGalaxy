using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	public float m_speed = 2.0f;

	//[SyncVar]
	public Vector2 m_mapPosition = Vector2.zero;

	private Vector2i    m_tilePos = Vector2i.zero;

	private GameObject m_mapOrigin;
	private float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

	public void SV_SetTilePosition(Vector2i tilePos) {
		m_tilePos = tilePos;
		m_mapPosition = Globals.MapPositionFromTilePosition(m_tilePos);
	}

	// Use this for initialization
	void Start () {
		m_mapOrigin = GameObject.Find("MapOrigin");
	}

	[Command]
	void CmdUpdatePosition(Vector2 pos) { // executed on the server only!
		m_mapPosition = pos;
		RpcSetPosition (pos);
	}

	[ClientRpc]
	void RpcSetPosition(Vector2 pos) { // other clients
		if (!isLocalPlayer)
			m_mapPosition = pos;
	}

	// Update is called once per frame
	void Update () {

		if (isLocalPlayer) {
			if (Input.GetKey (KeyCode.RightArrow)) {
				float dt = m_speed * Time.deltaTime;
				m_mapPosition.x += dt;
				m_mapOrigin.transform.position = m_mapOrigin.transform.position - new Vector3 (dt, 0.0f, 0.0f);
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				float dt = m_speed * Time.deltaTime;
				m_mapPosition.x -= dt;
				m_mapOrigin.transform.position = m_mapOrigin.transform.position + new Vector3 (dt, 0.0f, 0.0f);
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				float dt = m_speed * Time.deltaTime;
				m_mapPosition.y += dt;
				m_mapOrigin.transform.position = m_mapOrigin.transform.position - new Vector3 (0.0f, dt, 0.0f);
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				float dt = m_speed * Time.deltaTime;
				m_mapPosition.y -= dt;
				m_mapOrigin.transform.position = m_mapOrigin.transform.position + new Vector3 (0.0f, dt, 0.0f);
			}


			Vector2 pos = m_mapPosition;
			while (pos.x > mapSize)
				pos.x -= mapSize;
			while (pos.x < 0)
				pos.x += mapSize;
			while (pos.y > mapSize)
				pos.y -= mapSize;
			while (pos.y < 0)
				pos.y += mapSize;
			m_mapPosition = pos;


			CmdUpdatePosition (m_mapPosition); // tell my position to the server
		}

		// this moves the ground map so that it initially fills the entire mapping domain
		Matrix4x4 offset = Matrix4x4.TRS(
			new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));

		Vector2 mapPos = m_mapPosition;
		Matrix4x4 localToMap = Matrix4x4.TRS(
			new Vector3(mapPos.x, mapPos.y, 0.5f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, -1.0f));

		Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;

		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
