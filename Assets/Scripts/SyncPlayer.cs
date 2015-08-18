﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncPlayer : NetworkBehaviour {

    public GameObject m_collisionPlayerPrefab;
    public GameObject m_viewPlayerPrefab;

    public GameObject m_syncBombPrefab;

    private GameObject m_collisionPlayer;

    public GameObject collisionPlayer {
        get { return m_collisionPlayer; }
    }

    public GameObject viewPlayer { get; set; }

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
            transform.position = new Vector3(mapPos.x, Globals.m_syncYOff, mapPos.y);
        }

        // spawn the collisionplayer for this syncplayer

        m_collisionPlayer = Instantiate(m_collisionPlayerPrefab);
        m_collisionPlayer.GetComponent<CollisionPlayer>().SetSyncPlayer(gameObject);
        
        // spawn the viewplayer for this syncplayer
        
        viewPlayer = Instantiate(m_viewPlayerPrefab);
        viewPlayer.GetComponent<ViewPlayer>().SetSyncPlayer(gameObject);
	}
	
    [Command]
    void CmdSpawnBomb(Vector2 mapPos) {
        // first check if there is a bomb on this tile already
        Vector3 colliderCenter = m_syncBombPrefab.GetComponent<SyncBomb>().m_collisionBombPrefab.GetComponent<SphereCollider>().center;
        Vector3 rayOrigin = new Vector3(mapPos.x, 0.0f, mapPos.y) + colliderCenter;
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin + 50.0f * Vector3.up, Vector3.down, 100.0f);
        bool hitBomb = false;
        foreach (RaycastHit hit in hits)
        {
            if ("TAG_COLLISION_BOMB" == hit.transform.tag) hitBomb = true;
        }
        if (!hitBomb) {
            GameObject bomb = Instantiate(m_syncBombPrefab);
            bomb.GetComponent<SyncBomb>().SetMapPosition(Globals.TileCenterFromMapPosition(mapPos));
            NetworkServer.Spawn(bomb);
            RpcDropBomb();
        }
    }

    [ClientRpc]
    void RpcDropBomb() {
        viewPlayer.GetComponent<ViewPlayer>().DropBomb();
    }

	void Update () {
        if(isLocalPlayer) {
            if(Input.GetKeyDown(KeyCode.LeftControl)) {
                Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
                    transform.position.x,
                    transform.position.z));

                CmdSpawnBomb(mapPos);
            }
        }
	}
}
