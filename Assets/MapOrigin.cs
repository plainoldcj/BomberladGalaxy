using UnityEngine;
using System.Collections;

public class MapOrigin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void Update() { }
    
	public void SortedUpdate () {
		float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
		
		Vector3 pos = transform.position;
		while(pos.x > mapSize) pos.x -= mapSize;
		while(pos.y > mapSize) pos.y -= mapSize;
		while(pos.x < -mapSize) pos.x += mapSize;
		while(pos.y < -mapSize) pos.y += mapSize;
		transform.position = pos;
	}
}
