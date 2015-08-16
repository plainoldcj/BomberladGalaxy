using UnityEngine;
using System.Collections;

public class CollisionMap : MonoBehaviour {

    public GameObject m_blockPrefab;

    private GameObject[,] m_tileMap;

	public void Create(Map.Tile[] tileMap) {
        int numTilesPerEdge = Globals.m_numTilesPerEdge;

        Vector2[] offsets = Globals.m_mapGridOffsets;

        m_tileMap = new GameObject[tileMap.Length, offsets.Length];

        for(int i = 0; i < numTilesPerEdge; ++i) {
            for(int j = 0; j < numTilesPerEdge; ++j) {
                int tileIdx = numTilesPerEdge * i + j;
                if(tileMap[tileIdx].m_isBlock) {
                    Vector2i tilePos = new Vector2i(i, j);
                    Vector2 centerMapPos = Globals.MapPositionFromTilePosition(tilePos);

                    for(int k = 0; k < offsets.Length; ++k) {
                        GameObject block = Instantiate(m_blockPrefab);
                        block.GetComponent<CollisionBlock>().tilePosition = tilePos;
                        block.transform.parent = gameObject.transform;
                        Vector2 mapPos = centerMapPos + offsets[k];
                        block.transform.position = new Vector3(mapPos.x, 0.0f, mapPos.y);
                        block.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                        m_tileMap[tileIdx, k] = block;
                    }
                }
            }
        }
    }

    public void DestroyBlock(Vector2i tilePos, Map.Tile tile)
    {
        if (!tile.m_isBlock)
        {
            int tileIdx = Globals.m_numTilesPerEdge * tilePos.x + tilePos.y;
            int numOffsets = Globals.m_mapGridOffsets.Length;
            for (int i = 0; i < numOffsets; ++i)
            {
                GameObject collisionBlock = m_tileMap[tileIdx, i];
                Destroy(collisionBlock);
                m_tileMap[tileIdx, i] = null;
            }
        }
    }

}
