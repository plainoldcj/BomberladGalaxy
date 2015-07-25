using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public GameObject m_blockPrefab;

    private GameObject[] m_tileMap;

	public void Create(int seed, Vector2i[] spawnPos) {                            
		int numTilesPerEdge = Globals.m_numTilesPerEdge;

        // row-major layout
        m_tileMap = new GameObject[numTilesPerEdge * numTilesPerEdge];
		
        /*
        NOTE that this method must create the exact same level every
        time it is called with a given seed. Therefore, be aware of
        calls to Random.Range() inside if-blocks etc.
        */
        Random.seed = seed;
        
		// spawn probabilities
		const float probBlock = 0.5f;
		const float probWood = 0.5f;
		
		for(int i = 0; i < numTilesPerEdge; ++i) {
			for(int j = 0; j < numTilesPerEdge; ++j) {
                float rnd0 = Random.Range(0.0f, 1.0f);
                float rnd1 = Random.Range(0.0f, 1.0f);
            
                GameObject block = null;

				if(probBlock <= rnd0) {
					block = Instantiate(m_blockPrefab);
					block.transform.parent = transform;
					
					Block.Type type = Block.Type.Stone2;
					if(probWood <= rnd1) {
						type = Block.Type.Wood;
					}
					
					Block scr_block = block.GetComponent<Block>();
					scr_block.Init(type);
					scr_block.SetTilePosition(new Vector2i(i, j));
				}

                m_tileMap[numTilesPerEdge * i + j] = block;
			}
		}

        // remove blocks in L-shaped area around players

        Debug.Log("number of spawn positions: " + spawnPos.Length);

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
            GameObject.Destroy(m_tileMap[centerIdx]);
            m_tileMap[centerIdx] = null;

            int shapeIdx = 2 * (Random.Range(0, 1000) % numShapes);
            for(int j = 0; j < 2; ++j) {
                Vector2i tileCoords = LShapes[shapeIdx + j] + spawnPos[i];
                int tileIdx = numTilesPerEdge * tileCoords.y + tileCoords.x;
                GameObject.Destroy(m_tileMap[tileIdx]);
                m_tileMap[tileIdx] = null;
            }
        }
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
