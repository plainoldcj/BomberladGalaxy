using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

public class SyncBomb : NetworkBehaviour {

    public GameObject m_viewBombPrefab;
    public GameObject m_collisionBombPrefab;

    private bool m_isServer = false;

    private GameObject[] m_collisionBombs;

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

        Destroy(gameObject, Globals.m_bombTimeout);
	}
	
	void Update () {
	
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

    void OnDestroy()
    {
        if (m_isServer)
        {
            for (int i = 0; i < 4; ++i)
            {
                Globals.MapDirection mapDir = (Globals.MapDirection)i;

                TouchInfo inf;
                if (TouchesBlock(mapDir, out inf))
                {
                    Vector2i tilePos = inf.tilePosition;
                    MSG_DestroyBlock msg = new MSG_DestroyBlock();
                    msg.m_tilePosX = tilePos.x;
                    msg.m_tilePosY = tilePos.y;
                    NetworkServer.SendToAll(MessageTypes.m_destroyBlock, msg);
                }
            }
        }
    }
}
