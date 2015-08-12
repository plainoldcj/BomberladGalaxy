using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

    public GameObject m_viewMap;
    public GameObject m_collisionMap;

    public struct Tile {
        public bool        m_isBlock;
        public Block.Type  m_blockType;
    };

    private Tile[] m_tileMap;

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
        const float probWood = 0.5f;
        
        for(int i = 0; i < numTilesPerEdge; ++i) {
            for(int j = 0; j < numTilesPerEdge; ++j) {
                float rnd0 = Random.Range(0.0f, 1.0f);
                float rnd1 = Random.Range(0.0f, 1.0f);
                
                Tile tile = new Tile();
                tile.m_isBlock = false;
                
                if(probBlock <= rnd0) {
                    tile.m_isBlock = true;
                    
                    tile.m_blockType = Block.Type.Stone2;
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
}
