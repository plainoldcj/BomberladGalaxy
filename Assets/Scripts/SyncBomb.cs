using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

public class SyncBomb : NetworkBehaviour {

    // set this in prefab, must not change at runtime
    public float m_spawnRangeFactor = 1.3f;

    public GameObject m_viewBombPrefab;
    public GameObject m_collisionBombPrefab;

    private bool m_isServer = false;

    private GameObject[]    m_collisionBombs;
    private GameObject      m_localCollisionPlayer = null;
    private float           m_colliderDistance = 0.0f;

    [SyncVar]
    private int m_explosionRange = 1;

    public GameObject[] collisionBombs
    {
        get { return m_collisionBombs; }
    }

    public void SetExplosionRange(int range)
    {
        Assert.IsTrue(1 <= range && range <= Globals.m_maxExplosionRange);
        m_explosionRange = range;
    }

    public int GetExplosionRange()
    {
        return m_explosionRange;
    }

    public void SetMapPosition(Vector2 mapPos) {
        transform.position = new Vector3(mapPos.x, Globals.m_syncYOff, mapPos.y);
    }

	void Start () {
        // somehow 'false == isServer' in OnDestroy.
        m_isServer = isServer;

        // disables collisions with other syncobjects
        foreach (GameObject otherPlayer in GameObject.FindGameObjectsWithTag("TAG_SYNC_PLAYER"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
        }

	    // spawn viewbomb for this syncbomb
        GameObject viewBomb = Instantiate(m_viewBombPrefab);
        viewBomb.GetComponent<ViewBomb>().SetSyncBomb(gameObject);
        Destroy(viewBomb, Globals.m_bombTimeout);

        // spawn collisionbombs for this syncbomb
        Vector2[] offsets = Globals.m_mapGridOffsets;
        m_collisionBombs = new GameObject[offsets.Length];
        for (int i = 0; i < offsets.Length; ++i)
        {
            GameObject collisionBomb = Instantiate(m_collisionBombPrefab);
            CollisionBomb scr_collisionBomb = collisionBomb.GetComponent<CollisionBomb>();
            scr_collisionBomb.syncBomb = gameObject;
            scr_collisionBomb.SetGridIndex(i);
            scr_collisionBomb.SetMapGridOffset(offsets[i]);
            Destroy(collisionBomb, Globals.m_bombTimeout);
            m_collisionBombs[i] = collisionBomb;
        }

        m_localCollisionPlayer = Globals.FindLocalPlayer().GetComponent<SyncPlayer>().collisionPlayer;

        m_colliderDistance = m_collisionBombPrefab.GetComponent<SphereCollider>().radius +
            m_localCollisionPlayer.GetComponent<CharacterController>().radius;

        Destroy(gameObject, Globals.m_bombTimeout);
	}

    // 'myPos', 'otherPos' in collision space
    private bool InSpawnRange(Vector3 myPos, Vector3 otherPos)
    {
        Vector2 center = new Vector2(myPos.x, myPos.z);
        float radius = m_colliderDistance * m_spawnRangeFactor;
        return (new Vector2(otherPos.x, otherPos.z) - center).sqrMagnitude <= (radius * radius);
    }
	
	void Update () {
        // test each collision bomb for intersection with collision player	
        bool touchesPlayer = false;
        for(int i = 0; !touchesPlayer && i < m_collisionBombs.Length; ++i)
        {
            // calling 'GetCollisionMapPosition', in contrast to reading transform.position, works
            // even if Update() hasnt been called yet.
            Vector3 bombPos = m_collisionBombs[i].GetComponent<CollisionBomb>().GetCollisionMapPosition();
            if(InSpawnRange(bombPos, m_localCollisionPlayer.transform.position))
            {
                touchesPlayer = true;
            }
        }

        // enable collision with bombs, if there is no intersection
        if(!touchesPlayer)
        {
            foreach(GameObject it in m_collisionBombs)
            {
                it.GetComponent<SphereCollider>().isTrigger = false;
            }
        }
	}

    public class TouchInfo
    {
        public Vector2i tilePosition;
        public int      distance; // in adjacent tiles
    };

    /*
    finds nearest collision block in explosion range
    */
    public bool TouchesBlock(Globals.MapDirection mapDir, out TouchInfo inf)
    {
        inf = null;
        for(int i = 1; i <= m_explosionRange; ++i)
        {
            Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
                transform.position.x,
                transform.position.z));
            Vector3 rayOrigin = new Vector3(mapPos.x, 0.0f, mapPos.y) + m_collisionBombPrefab.GetComponent<SphereCollider>().center;
            Vector3 rayDir = Globals.ToCollisionMapVector(mapDir);

            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDir, i * Globals.m_tileEdgeLength);
            foreach (RaycastHit hit in hits)
            {
                if ("TAG_COLLISION_BLOCK" == hit.transform.tag)
                {
                    GameObject collisionBlock = hit.transform.gameObject;
                    inf = new TouchInfo();
                    inf.tilePosition = collisionBlock.GetComponent<CollisionBlock>().tilePosition;
                    inf.distance = i;
                    return true;
                }
            }
        }
        return false;
    }

    private void SV_KillPlayers(Globals.MapDirection mapDir, int range)
    {
        Vector2 mapPos0 = Globals.WrapMapPosition(new Vector2(
            transform.position.x,
            transform.position.z));
        Vector3 rayDir = Globals.ToCollisionMapVector(mapDir);

        Vector3 center = m_collisionBombPrefab.GetComponent<SphereCollider>().center;
        center.y = 0.0f;

        for (int i = 0; i < Globals.m_mapGridOffsets.Length; ++i)
        {
            Vector2 mapPos = mapPos0 + Globals.m_mapGridOffsets[i];
            Vector3 rayOrigin = new Vector3(mapPos.x, 0.0f, mapPos.y) + center;

            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDir, range * Globals.m_tileEdgeLength);
            foreach (RaycastHit hit in hits)
            {
                if ("TAG_COLLISION_PLAYER" == hit.transform.tag)
                {
                    Debug.Log("explosion in direction " + mapDir + " hit collision player");
                    GameObject collisionPlayer = hit.transform.gameObject;
                    GameObject syncPlayer = collisionPlayer.GetComponent<CollisionPlayer>().GetSyncPlayer();
					if (syncPlayer)
	                    syncPlayer.GetComponent<SyncPlayer>().RpcDie();
                }
            }

        }
    }

    void OnDestroy()
    {
		if (m_isServer)
        {
            for (int i = 0; i < 4; ++i)
            {
                Globals.MapDirection mapDir = (Globals.MapDirection)i;

                int range = m_explosionRange;

                TouchInfo inf;
                if (TouchesBlock(mapDir, out inf))
                {
                    Vector2i tilePos = inf.tilePosition;
                    MSG_DestroyBlock msg = new MSG_DestroyBlock();
                    msg.m_tilePosX = tilePos.x;
                    msg.m_tilePosY = tilePos.y;
                    NetworkServer.SendToAll(MessageTypes.m_destroyBlock, msg);
                }

                SV_KillPlayers(mapDir, range);
            }
        }
    }
}
