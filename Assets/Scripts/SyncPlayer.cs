using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncPlayer : NetworkBehaviour {

    public GameObject m_collisionPlayerPrefab;
    public GameObject m_viewPlayerPrefab;

	void Start () {
        // effectively disables collisions with other client's sync players
        foreach (GameObject otherPlayer in GameObject.FindGameObjectsWithTag("TAG_SYNC_PLAYER"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
        }

        if (isLocalPlayer) {
            // the spawn positions encode tile coordinates, so first we have
            // to convert them to map coordinates
            Vector2i tilePos = new Vector2i((int)transform.position.x, (int)transform.position.y);
            Vector2 mapPos = Globals.MapPositionFromTilePosition(tilePos);
            float yOff = -100; // this moves the syncplayer away from the collisionmap at y=0
            transform.position = new Vector3(mapPos.x, yOff, mapPos.y);
        }

        // spawn the collisionplayer for this syncplayer

        GameObject collisionPlayer = Instantiate(m_collisionPlayerPrefab);
        collisionPlayer.GetComponent<CollisionPlayer>().SetSyncPlayer(gameObject);
        
        // spawn the viewplayer for this syncplayer
        
        GameObject viewPlayer = Instantiate(m_viewPlayerPrefab);
        viewPlayer.GetComponent<ViewPlayer>().SetSyncPlayer(gameObject);
	}
	
	void Update () {
	
	}
}
