using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(CharacterController))]

public class CollisionPlayer : NetworkBehaviour {

    public GameObject m_viewPlayerPrefab;

    public float m_speed = 5.0f;

    private CharacterController m_charController;

	void Start () {
        m_charController = GetComponent<CharacterController>();

	    // the spawn positions encode tile coordinates, so first we have
        // to convert them to map coordinates
        Vector2i tilePos = new Vector2i((int)transform.position.x, (int)transform.position.y);
        Vector2 mapPos = Globals.MapPositionFromTilePosition(tilePos);
        transform.position = new Vector3(mapPos.x, 0.0f, mapPos.y);

        // spawn the view player for this collision player

        GameObject viewPlayer = Instantiate(m_viewPlayerPrefab);
        viewPlayer.GetComponent<ViewPlayer>().SetCollisionPlayer(gameObject);
	}
	
	void Update () {
	    if(isLocalPlayer) {
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

            // wrap position
            float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            while(pos.x > mapSize) pos.x -= mapSize;
            while(pos.x < 0) pos.x += mapSize;
            while(pos.y < mapSize) pos.y += mapSize;
            while(pos.y > 0) pos.y -= mapSize;
            transform.position = new Vector3(pos.x, transform.position.y, pos.y);
        }
	}
}
