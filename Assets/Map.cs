using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public GameObject m_blockPrefab;

	public void RpcCreate(int seed) {                            
		int numTilesPerEdge = Globals.m_numTilesPerEdge;
		
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
            
				if(probBlock <= rnd0) {
					GameObject block = Instantiate(m_blockPrefab);
					block.transform.parent = transform;
					
					Block.Type type = Block.Type.Stone2;
					if(probWood <= rnd1) {
						type = Block.Type.Wood;
					}
					
					Block scr_block = block.GetComponent<Block>();
					scr_block.Init(type);
					scr_block.SetTilePosition(new Vector2(i, j));
				}
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
