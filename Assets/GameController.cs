using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	void Start () {
	    /*
        when this method is called we know that the main scene has been loaded,
        so we use it to initialize the game
        */
        
        int mapSeed = (int)System.DateTime.Now.Ticks;
        GameObject.Find("Map").GetComponent<Map>().RpcCreate(mapSeed);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
