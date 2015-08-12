using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(CharacterController))]

public class CollisionPlayer : MonoBehaviour {

    public float m_speed = 5.0f;

    private CharacterController m_charController;
    private GameObject          m_syncPlayer;

    public void SetSyncPlayer(GameObject syncPlayer) {
        m_syncPlayer = syncPlayer;
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

	    if(m_syncPlayer.GetComponent<SyncPlayer>().isLocalPlayer) {
            Vector3 oldPosition = transform.position;

            if(Input.GetKey(KeyCode.RightArrow)) {
                m_charController.SimpleMove(m_speed * Vector3.right);
            }
            if(Input.GetKey(KeyCode.LeftArrow)) {
                m_charController.SimpleMove(m_speed * -Vector3.right);
            }
            if(Input.GetKey(KeyCode.UpArrow)) {
                m_charController.SimpleMove(m_speed * Vector3.forward);
            }
            if(Input.GetKey(KeyCode.DownArrow)) {
                m_charController.SimpleMove(m_speed * -Vector3.forward);
            }

            Vector3 movement = transform.position - oldPosition;
            m_syncPlayer.transform.position += movement;
        }
	}
}
