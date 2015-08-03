using UnityEngine;
using System.Collections;

public class ViewPlayer : MonoBehaviour {

    private GameObject m_collisionPlayer;

    private GameObject m_mapOrigin;

	public void SetCollisionPlayer(GameObject collisionPlayer) {
        m_collisionPlayer = collisionPlayer;
    }

	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");
	}

	void Update () {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        // copy position from my collision player
        Vector2 mapPos = new Vector2(
            m_collisionPlayer.transform.position.x,
            m_collisionPlayer.transform.position.z);

        // wrap position to [0, mapSize]x[0, mapSize]
        while(mapPos.x > mapSize) mapPos.x -= mapSize;
        while(mapPos.x < 0.0f) mapPos.x += mapSize;
        while(mapPos.y < -mapSize) mapPos.y += mapSize;
        while(mapPos.y > 0.0f) mapPos.y -= mapSize;
        
        if(m_collisionPlayer.GetComponent<CollisionPlayer>().isLocalPlayer) {
            m_mapOrigin.transform.position = new Vector3(
                0.5f * mapSize - mapPos.x, 
                0.5f * mapSize - mapPos.y,
                0.0f);
        }

		// this moves the ground map so that it initially fills the entire mapping domain
		Matrix4x4 offset = Matrix4x4.TRS(
			new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, 1.0f));

        Vector3 localOffset = new Vector3(0.5f, 0.0f, 0.0f);
		Matrix4x4 localToMap = Matrix4x4.TRS(
			localOffset + new Vector3(mapPos.x, mapPos.y, 0.5f),
			Quaternion.identity,
			new Vector3(1.0f, 1.0f, -1.0f));

		Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;

		GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
