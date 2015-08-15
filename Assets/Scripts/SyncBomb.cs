using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncBomb : NetworkBehaviour {

    public GameObject m_viewBombPrefab;
    public GameObject m_collisionBombPrefab;

    private GameObject[] m_collisionBombs;

    public GameObject[] collisionBombs
    {
        get { return m_collisionBombs; }
    }

    public void SetMapPosition(Vector2 mapPos) {
        transform.position = new Vector3(mapPos.x, Globals.m_syncYOff, mapPos.y);
    }

	void Start () {
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
            scr_collisionBomb.SetSyncBomb(gameObject);
            scr_collisionBomb.SetGridIndex(i);
            scr_collisionBomb.SetMapGridOffset(offsets[i]);
            Destroy(collisionBomb, Globals.m_bombTimeout);
            m_collisionBombs[i] = collisionBomb;
        }

        Destroy(gameObject, Globals.m_bombTimeout);
	}
	
	void Update () {
	
	}
}
