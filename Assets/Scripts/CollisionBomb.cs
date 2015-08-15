using UnityEngine;
using System.Collections;

public class CollisionBomb : MonoBehaviour {

    private Vector2     m_mapGridOffset = Vector2.zero;
    private int         m_gridIndex = -1;
    private GameObject  m_syncBomb;
    private int         m_numPlayers = 0;

    public void SetGridIndex(int gridIndex)
    {
        m_gridIndex = gridIndex;
    }

    public void SetMapGridOffset(Vector2 mapGridOffset)
    {
        m_mapGridOffset = mapGridOffset;
    }

    public void SetSyncBomb(GameObject syncBomb)
    {
        m_syncBomb = syncBomb;
    }

    private Vector2 GetMapPosition()
    {
        // copy position from my syncplayer and wrap it
        Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
            m_syncBomb.transform.position.x,
            m_syncBomb.transform.position.z));
        return mapPos;
    }

    private Vector3 GetCollisionMapPosition()
    {
        Vector2 mapPos = GetMapPosition() + m_mapGridOffset;
        return new Vector3(mapPos.x, transform.position.y, mapPos.y);
    }

	// Use this for initialization
	void Start () {
        if (0 == m_gridIndex) {
            // find all players at bomb's starting position
            Vector3 rayOrigin = GetCollisionMapPosition() + GetComponent<SphereCollider>().center;

            RaycastHit[] hits = Physics.RaycastAll(rayOrigin + 50.0f * Vector3.up, Vector3.down, 100.0f);
            m_numPlayers = 0;
            foreach(RaycastHit hit in hits) {
                string tag = hit.transform.tag;
                Debug.Log("CollisionBomb raycast hit gameobject with tag " + tag);
                if("TAG_COLLISION_PLAYER" == tag) m_numPlayers++;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = GetCollisionMapPosition();
	}

    void OnTriggerExit(Collider other)
    {
        if ("TAG_COLLISION_PLAYER" == other.tag)
        {
            m_numPlayers--;
            if (0 >= m_numPlayers)
            {
                GameObject[] peers = m_syncBomb.GetComponent<SyncBomb>().collisionBombs;
                foreach (GameObject peer in peers) {
                    peer.GetComponent<Collider>().isTrigger = false;
                }
            }
        }
    }
}
