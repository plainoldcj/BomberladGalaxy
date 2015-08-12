using UnityEngine;
using System.Collections;

public class MapOrigin : MonoBehaviour {

    private static Vector3 WrapPosition(Vector3 pos) {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        while(pos.x > mapSize) pos.x -= mapSize;
        while(pos.y > mapSize) pos.y -= mapSize;
        while(pos.x < -mapSize) pos.x += mapSize;
        while(pos.y < -mapSize) pos.y += mapSize;

        return pos;
    }

    // prefer this to setting gameobject's transform directly
    public void SetPosition(Vector3 pos) {
        transform.position = WrapPosition(pos);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	void Update() { }
    
	public void SortedUpdate () {
        transform.position = WrapPosition(transform.position);
	}
}
