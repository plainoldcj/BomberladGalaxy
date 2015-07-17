using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public GameObject m_blockPrefab;

	private void Init() {
		Globals globals = GameObject.FindWithTag("GameController").GetComponent<Globals>();
		int numTilesPerEdge = globals.m_numTilesPerEdge;
		
		// spawn probabilities
		const float probBlock = 0.5f;
		const float probWood = 0.5f;
		
		for(int i = 0; i < numTilesPerEdge; ++i) {
			for(int j = 0; j < numTilesPerEdge; ++j) {
				if(probBlock <= Random.Range(0.0f, 1.0f)) {
					GameObject block = Instantiate(m_blockPrefab);
					
					Block.Type type = Block.Type.Stone2;
					if(probWood <= Random.Range(0.0f, 1.0f)) {
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
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
