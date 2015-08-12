using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(CharacterController))]

public class CollisionPlayer : NetworkBehaviour {

    public GameObject   m_viewPlayerPrefab;

    public float m_speed = 5.0f;

    private CharacterController m_charController;
    private GameObject          m_collisionMap;

	void Start () {
        m_charController = GetComponent<CharacterController>();

        m_collisionMap = GameObject.Find("CollisionMap");

        if (isLocalPlayer) {
            // the spawn positions encode tile coordinates, so first we have
            // to convert them to map coordinates
            Vector2i tilePos = new Vector2i((int)transform.position.x, (int)transform.position.y);
            Vector2 mapPos = Globals.MapPositionFromTilePosition(tilePos);
            transform.position = new Vector3(mapPos.x, 0.0f, mapPos.y);
        }

        // spawn the view player for this collision player

        GameObject viewPlayer = Instantiate(m_viewPlayerPrefab);
        viewPlayer.GetComponent<ViewPlayer>().SetCollisionPlayer(gameObject);

        // effectively disables collisions with other client's collision players
        foreach (GameObject otherPlayer in GameObject.FindGameObjectsWithTag("TAG_COLLISION_PLAYER"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
        }
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

            // instead of wrapping the collision player's position,
            // we move the collision map, so that the collision player
            // stays in its center part

            float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

            Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
            Vector2 mapPos = new Vector2(m_collisionMap.transform.position.x, m_collisionMap.transform.position.z);

            while((playerPos - mapPos).x > mapSize) mapPos.x += mapSize;
            while((playerPos - mapPos).x < 0.0f) mapPos.x -= mapSize;
            while((playerPos - mapPos).y < -mapSize) mapPos.y -= mapSize;
            while((playerPos - mapPos).y > 0.0f) mapPos.y += mapSize;

            m_collisionMap.transform.position = new Vector3(mapPos.x, m_collisionMap.transform.position.y, mapPos.y);
        }
	}
}
