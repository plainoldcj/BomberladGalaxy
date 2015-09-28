using UnityEngine;
using System.Collections;

public class CollisionBomb : MonoBehaviour {

    public float m_spawnRangeFactor = 1.3f; // set this in prefab

    private Vector2     m_mapGridOffset = Vector2.zero;
    private int         m_gridIndex = -1;
    private GameObject  m_syncBomb;
    private GameObject  m_localCollisionPlayer = null;
    private float       m_colliderDistance = 0.0f;

    public void SetGridIndex(int gridIndex)
    {
        m_gridIndex = gridIndex;
    }

    public void SetMapGridOffset(Vector2 mapGridOffset)
    {
        m_mapGridOffset = mapGridOffset;
    }

    public GameObject syncBomb
    {
        get { return m_syncBomb; }
        set { m_syncBomb = value; }
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

    // 'myPos', 'otherPos' in collision space
    private bool InSpawnRange(Vector3 myPos, Vector3 otherPos)
    {
        Vector2 center = new Vector2(myPos.x, myPos.z);
        float radius = m_colliderDistance * m_spawnRangeFactor;
        return (new Vector2(otherPos.x, otherPos.z) - center).sqrMagnitude <= (radius * radius);
    }

	// Use this for initialization
	void Start () {
        m_localCollisionPlayer = Globals.FindLocalPlayer().GetComponent<SyncPlayer>().collisionPlayer;

        m_colliderDistance = GetComponent<SphereCollider>().radius +
            m_localCollisionPlayer.GetComponent<CharacterController>().radius;
	}

	// Update is called once per frame
	void Update () {
        transform.position = GetCollisionMapPosition();

        Collider collider = GetComponent<Collider>();
        if(collider.isTrigger && !InSpawnRange(transform.position, m_localCollisionPlayer.transform.position))
        {
            collider.isTrigger = false;
        }
	}
}
