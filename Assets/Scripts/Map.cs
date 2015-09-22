using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class Map : MonoBehaviour {

    public GameObject m_viewMap;
    public GameObject m_collisionMap;

    public struct Tile {
        public bool         m_isBlock;
        public Block.Type   m_blockType;
        public int          m_blockHitpoints;
    };

    private Tile[] m_tileMap;

    public Block.Type GetBlockType(Vector2i tilePos)
    {
        int tileIdx = Globals.m_numTilesPerEdge * tilePos.x + tilePos.y;
        Assert.IsTrue(m_tileMap[tileIdx].m_isBlock);
        return m_tileMap[tileIdx].m_blockType;
    }

    public int GetBlockHitpoints(Vector2i tilePos)
    {
        int tileIdx = Globals.m_numTilesPerEdge * tilePos.x + tilePos.y;
        Assert.IsTrue(m_tileMap[tileIdx].m_isBlock);
        return m_tileMap[tileIdx].m_blockHitpoints;
    }

    private void CreateTileMap(int seed, Vector2i[] spawnPos) {                            
        int numTilesPerEdge = Globals.m_numTilesPerEdge;
        
        // row-major layout
        m_tileMap = new Tile[numTilesPerEdge * numTilesPerEdge];
        
        /*
        NOTE that this method must create the exact same level every
        time it is called with a given seed. Therefore, be aware of
        calls to Random.Range() inside if-blocks etc.
        */
        Random.seed = seed;
        
        // spawn probabilities
        const float probBlock = 0.8f;
        const float probWood = 0.2f;
        
        for(int i = 0; i < numTilesPerEdge; ++i) {
            for(int j = 0; j < numTilesPerEdge; ++j) {
                float rnd0 = Random.Range(0.0f, 1.0f);
                float rnd1 = Random.Range(0.0f, 1.0f);
                
                Tile tile = new Tile();
                tile.m_isBlock = false;
                
                if(probBlock <= rnd0) {
                    tile.m_isBlock = true;
                    
                    tile.m_blockType = Block.Type.Stone;
                    tile.m_blockHitpoints = Globals.m_stoneBlockHitpoints;
                    if(probWood <= rnd1) {
                        tile.m_blockType = Block.Type.Wood;
                    }
                }
                
                m_tileMap[numTilesPerEdge * i + j] = tile;
            }
        }
        
        // remove blocks in L-shaped area around players
        
        // encodes L-shapes in relative tile coordinates.
        // (0, 0) is implicit, because it's part of every L.
        // A shape starts at 2i, i = 0, 1, 2, ...
        Vector2i[] LShapes = {
            new Vector2i(-1, 0), new Vector2i(-1, -1),
            new Vector2i( 1, 0), new Vector2i( 1,  1)
        };
        int numShapes = LShapes.Length / 2;
        
        for(int i = 0; i < spawnPos.Length; ++i) {
            int centerIdx = numTilesPerEdge * spawnPos[i].y + spawnPos[i].x;
            m_tileMap[centerIdx].m_isBlock = false;
            
            int shapeIdx = 2 * (Random.Range(0, 1000) % numShapes);
            for(int j = 0; j < 2; ++j) {
                Vector2i tileCoords = LShapes[shapeIdx + j] + spawnPos[i];
                int tileIdx = numTilesPerEdge * tileCoords.y + tileCoords.x;
                m_tileMap[tileIdx].m_isBlock = false;
            }
        }
    }

    public void Create(int seed, Vector2i[] spawnPos) {
        CreateTileMap(seed, spawnPos);
        m_viewMap.GetComponent<ViewMap>().Create(m_tileMap);
        m_collisionMap.GetComponent<CollisionMap>().Create(m_tileMap);
    }

    public void DestroyBlock(Vector2i tilePos)
    {
        int tileIdx = Globals.m_numTilesPerEdge * tilePos.x + tilePos.y;

        Tile tile = m_tileMap[tileIdx];
        if (tile.m_isBlock)
        {
            bool destroyBlock = false;
            if (Block.Type.Stone == tile.m_blockType)
            {
                if (0 >= --tile.m_blockHitpoints)
                {
                    destroyBlock = true;
                }
            }
            else
            {
                Assert.IsTrue(Block.Type.Wood == tile.m_blockType);
                destroyBlock = true;
            }

            if (destroyBlock)
            {
                tile.m_isBlock = false;
            }
        }
        m_tileMap[tileIdx] = tile;

        m_collisionMap.GetComponent<CollisionMap>().DestroyBlock(tilePos, tile);
        m_viewMap.GetComponent<ViewMap>().DestroyBlock(tilePos, tile);
    }
}
