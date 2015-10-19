using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncPlayer : NetworkBehaviour {

    public GameObject m_collisionPlayerPrefab;
    public GameObject m_viewPlayerPrefab;

    public GameObject m_syncBombPrefab;

    [SyncVar]
    public Color playerColor = Color.black;

    private GameObject m_collisionPlayer;

    private int m_explosionRange = 1;

	public bool isDead = false;

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
		viewPlayer.GetComponentInChildren<Renderer> ().material.color = playerColor;
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
            SyncBomb scr_syncBomb = bomb.GetComponent<SyncBomb>();
            scr_syncBomb.SetMapPosition(Globals.TileCenterFromMapPosition(mapPos));
            scr_syncBomb.SetExplosionRange(m_explosionRange);
            NetworkServer.Spawn(bomb);
            RpcDropBomb();
        }
    }

    [ClientRpc]
    void RpcDropBomb() {
        viewPlayer.GetComponent<ViewPlayer>().DropBomb();
    }

    [ClientRpc]
    public void RpcDie()
    {
        collisionPlayer.GetComponent<CollisionPlayer>().Die();
        viewPlayer.GetComponent<ViewPlayer>().Die();
		isDead = true;
    }

	void Update () {
        if(isLocalPlayer) {
			if(Input.GetButtonDown("Fire1")) {
				if (isDead) {
					// TODO: switch camera? follow next player..
				} else {
					Vector2 mapPos = Globals.WrapMapPosition (new Vector2 (
						                            transform.position.x,
						                            transform.position.z));
					CmdSpawnBomb (mapPos);
				}
			}
				

            // TODO: debug only
            if(Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                m_explosionRange = (int)Mathf.Min(m_explosionRange + 1, Globals.m_maxExplosionRange);
                Debug.Log("player explosion range = " + m_explosionRange);
            }
            if(Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                m_explosionRange = (int)Mathf.Max(m_explosionRange - 1, 1);
                Debug.Log("player explosion range = " + m_explosionRange);
            }
        }
	}

	void OnDestroy() {
		Destroy (viewPlayer);
	}

}
