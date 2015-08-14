using UnityEngine;
using System.Collections;

public class ViewPlayer : MonoBehaviour {

    private GameObject m_mapOrigin;
    private GameObject m_skyboxCamera;
    private GameObject m_syncPlayer;

	public void SetSyncPlayer(GameObject syncPlayer) {
        m_syncPlayer = syncPlayer;
    }

	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");
        m_skyboxCamera = GameObject.Find("SkyboxCamera");
	}

	void Update () {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        // copy position from my syncplayer and wrap it
        Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
            m_syncPlayer.transform.position.x,
            m_syncPlayer.transform.position.z));
        
        if(m_syncPlayer.GetComponent<SyncPlayer>().isLocalPlayer) {
            m_mapOrigin.GetComponent<MapOrigin>().SetPosition(new Vector3(
                0.5f * mapSize - mapPos.x, 
                0.5f * mapSize - mapPos.y,
                0.0f));

            GameObject collisionPlayer = m_syncPlayer.GetComponent<SyncPlayer>().collisionPlayer;
            Vector3 movement = collisionPlayer.GetComponent<CollisionPlayer>().lastMovement;

            Vector3 cameraRotation = (360.0f / mapSize) * movement;

            m_skyboxCamera.transform.rotation =
                    Quaternion.AngleAxis(cameraRotation.x, m_skyboxCamera.transform.up) *
                    Quaternion.AngleAxis(cameraRotation.z, m_skyboxCamera.transform.right) *
                    m_skyboxCamera.transform.rotation;
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
