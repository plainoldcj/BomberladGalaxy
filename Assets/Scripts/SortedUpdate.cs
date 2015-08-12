using UnityEngine;
using System.Collections;

public class SortedUpdate : MonoBehaviour {

    public GameObject m_mapOrigin;
    public GameObject m_ground;

	// Use this for initialization
	void Start () {
	    if(null == m_mapOrigin) m_mapOrigin = GameObject.Find("MapOrigin");
        if(null == m_ground) m_ground = GameObject.Find("Ground");
	}
	
	/*
    In each frame, all blocks and the ground must use the same map origin to avoid
    "glibber". Therefore, the update of the map origin must occur first. Unfortunately,
    however, there is no easy way to specify an update order of gameobjects, so we do it here.
    */
	void Update () {
	    m_mapOrigin.GetComponent<MapOrigin>().SortedUpdate();
        m_ground.GetComponent<Ground>().SortedUpdate();
        
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("TAG_BLOCK");
        foreach(GameObject block in blocks) {
            block.GetComponent<Block>().SortedUpdate();
        }
	}
}
