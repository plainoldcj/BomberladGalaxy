using UnityEngine;
using System.Collections;

public class ViewPlayer : MonoBehaviour {

    public Vector3 m_position = Vector3.zero;
    public Vector3 m_rotation = Vector3.zero;
    public Vector3 m_scale = Vector3.one;

    private GameObject m_mapOrigin;
    private GameObject m_skyboxCamera;
    private GameObject m_syncPlayer;

    public float        m_viewInterpolationFactor = 0.1f;
    private Vector2     m_lastViewPos = Vector2.zero;
    private Quaternion  m_viewRotation = Quaternion.identity;
    private Quaternion  m_targetViewRotation = Quaternion.identity;

    // child gameobject that has mesh attached to it
    private GameObject  m_mesh;
    private Transform   m_rootBone;

	public void SetSyncPlayer(GameObject syncPlayer) {
        m_syncPlayer = syncPlayer;
    }

	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");
        m_skyboxCamera = GameObject.Find("SkyboxCamera");

        foreach(Transform child in transform) {
            if("Mesh" == child.name) m_mesh = child.gameObject;
            if("LK" == child.name) m_rootBone = child;
        }

        m_mesh.GetComponent<Renderer>().material.EnableKeyword("ENABLE_RIM_LIGHTING");
	}

	void Update () {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        // NOTE: view direction computation with unwrapped coordinates
        Vector2 viewPos = new Vector2(
            m_syncPlayer.transform.position.x,
            m_syncPlayer.transform.position.z);
        Vector2 deltaMapPos = viewPos - m_lastViewPos;
        float speed = deltaMapPos.sqrMagnitude;
        // use same small eps in animator transition threshold
        if(0.001f < speed) {
            float viewAngle = Mathf.Rad2Deg * Mathf.Atan2(deltaMapPos.y, deltaMapPos.x);
            m_targetViewRotation = Quaternion.AngleAxis(viewAngle, Vector3.forward);
            m_lastViewPos = viewPos;
        }
        m_viewRotation = Quaternion.Slerp(m_viewRotation, m_targetViewRotation, m_viewInterpolationFactor);
        GetComponent<Animator>().SetFloat("speed", speed);

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

        Matrix4x4 localTransform = Matrix4x4.TRS(m_position, Quaternion.Euler(m_rotation), m_scale);

        Matrix4x4 viewTransform = Matrix4x4.TRS(Vector3.zero, m_viewRotation, Vector3.one);

		Matrix4x4 localToWorld =
                m_rootBone.worldToLocalMatrix *
                m_mapOrigin.transform.localToWorldMatrix *
                offset *
                localToMap *
                viewTransform *
                localTransform;

		m_mesh.GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
		m_mesh.GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
