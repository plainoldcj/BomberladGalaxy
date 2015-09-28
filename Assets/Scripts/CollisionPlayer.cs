using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(CharacterController))]

public class CollisionPlayer : MonoBehaviour {

    public float m_speed = 5.0f;

    private bool                m_isAlive = true;
    private CharacterController m_charController;
    private GameObject          m_syncPlayer;
    private Vector3             m_lastMovement = Vector3.zero;

    public void SetSyncPlayer(GameObject syncPlayer) {
        m_syncPlayer = syncPlayer;
    }

    public GameObject GetSyncPlayer()
    {
        return m_syncPlayer;
    }

    public Vector3 lastMovement {
        get { return m_lastMovement; }
    }

    public void Die()
    {
        m_isAlive = false;
    }

	void Start () {
        m_charController = GetComponent<CharacterController>();

        // effectively disables collisions with other client's collision players
        foreach (GameObject otherPlayer in GameObject.FindGameObjectsWithTag("TAG_COLLISION_PLAYER"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
        }
	}
	
	void Update () {
        // copy position from my syncplayer and wrap it
        Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
            m_syncPlayer.transform.position.x,
            m_syncPlayer.transform.position.z));
        transform.position = new Vector3(mapPos.x, transform.position.y, mapPos.y);

        // applies gravitation, brings remote player objects on same y-coordinate
        // as remote players
        m_charController.SimpleMove(Vector3.zero);

	    if(m_isAlive && m_syncPlayer.GetComponent<SyncPlayer>().isLocalPlayer) {
            Vector3 oldPosition = transform.position;

            Vector3 direction =
                Vector3.right * Input.GetAxis("Horizontal") +
                Vector3.forward * Input.GetAxis("Vertical");
            float dirLen = direction.magnitude;

            if (0.0f < dirLen) {
                m_charController.SimpleMove(m_speed * (direction / dirLen));
            }

            Vector3 movement = transform.position - oldPosition;
            m_syncPlayer.transform.position += movement;
            m_lastMovement = movement;
        }
	}
}
