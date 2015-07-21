using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	/*
	A map is a square array of tiles.
	*/

	// map constants
	public static readonly int 		m_numTilesPerEdge = 20;
	
	// map geometry
	public static readonly float 	m_tileEdgeLength = 1.0f;
	
	public static readonly int		m_blockDetail = 10;		// number of vertical edge vertices
	public static readonly float 	m_blockHeight = 1f;

}
