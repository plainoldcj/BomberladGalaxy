using UnityEngine;
using System.Collections;

public class CollisionMap : MonoBehaviour {

    public GameObject m_blockPrefab;

    private GameObject[] m_tileMap;

	public void Create(Map.Tile[] tileMap) {
        int numTilesPerEdge = Globals.m_numTilesPerEdge;

        m_tileMap = new GameObject[tileMap.Length];

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        Vector2[] offsets = {
            new Vector2(0.0f, 0.0f),
            new Vector2(-mapSize, 0.0f),
            new Vector2( mapSize, 0.0f),
            new Vector2(0.0f, -mapSize),
            new Vector2(0.0f,  mapSize)
        };

        for(int i = 0; i < numTilesPerEdge; ++i) {
            for(int j = 0; j < numTilesPerEdge; ++j) {
                int tileIdx = numTilesPerEdge * i + j;
                if(tileMap[tileIdx].m_isBlock) {
                    Vector2i tilePos = new Vector2i(i, j);
                    Vector2 centerMapPos = Globals.MapPositionFromTilePosition(tilePos);

                    for(int k = 0; k < 5; ++k) {
                        GameObject block = Instantiate(m_blockPrefab);
                        block.transform.parent = gameObject.transform;
                        Vector2 mapPos = centerMapPos + offsets[k];
                        block.transform.position = new Vector3(mapPos.x, 0.0f, mapPos.y);
                        block.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                        m_tileMap[tileIdx] = block;
                    }
                }
            }
        }
    }

}
