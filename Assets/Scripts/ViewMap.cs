using UnityEngine;
using System.Collections;

public class ViewMap : MonoBehaviour {

	public GameObject m_blockPrefab;

    private GameObject[] m_tileMap;

	public void Create(Map.Tile[] tileMap) {                            
		int numTilesPerEdge = Globals.m_numTilesPerEdge;

        // row-major layout
        m_tileMap = new GameObject[tileMap.Length];
		
		for(int i = 0; i < numTilesPerEdge; ++i) {
			for(int j = 0; j < numTilesPerEdge; ++j) {
                Map.Tile tile = tileMap[numTilesPerEdge * i + j];
				if(tile.m_isBlock) {
					GameObject block = Instantiate(m_blockPrefab);
					block.transform.parent = transform;
					
					Block scr_block = block.GetComponent<Block>();
					scr_block.Init(tile.m_blockType);
					scr_block.SetTilePosition(new Vector2i(i, j));

                    m_tileMap[numTilesPerEdge * i + j] = block;
				}
			}
		}
	}

    public void DestroyBlock(Vector2i tilePos, Map.Tile tile)
    {
        if (!tile.m_isBlock)
        {
            int tileIdx = Globals.m_numTilesPerEdge * tilePos.x + tilePos.y;
            GameObject viewBlock = m_tileMap[tileIdx];
            Destroy(viewBlock);
            m_tileMap[tileIdx] = null;
        }
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
