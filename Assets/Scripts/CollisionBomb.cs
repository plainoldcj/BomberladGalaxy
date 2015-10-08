using UnityEngine;
using System.Collections;

public class CollisionBomb : MonoBehaviour {

    private Vector2     m_mapGridOffset = Vector2.zero;
    private int         m_gridIndex = -1;
    private GameObject  m_syncBomb;

    private static GameObject m_collisionBombGroup = null;

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

    public Vector3 GetCollisionMapPosition()
    {
        Vector2 mapPos = GetMapPosition() + m_mapGridOffset;
        return new Vector3(mapPos.x, transform.position.y, mapPos.y);
    }

	// Use this for initialization
	void Start () {
        if(null == m_collisionBombGroup) {
            m_collisionBombGroup =
                GameObject.Find("CollisionBombs") ??
                new GameObject("CollisionBombs");
        }
        transform.parent = m_collisionBombGroup.transform;
	}

	// Update is called once per frame
	void Update () {
		if (m_syncBomb != null) {
			transform.position = GetCollisionMapPosition ();
		}
	}
}
