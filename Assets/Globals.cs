using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MessageTypes {
    public static readonly short m_startGame = MsgType.Highest + 1;
}

public class MSG_StartGame : MessageBase {
    public int  m_mapSeed;

    // tile coordinates of spawns

    public byte m_spawnPosX0;
    public byte m_spawnPosX1;
    public byte m_spawnPosX2;
    public byte m_spawnPosX3;

    public byte m_spawnPosY0;
    public byte m_spawnPosY1;
    public byte m_spawnPosY2;
    public byte m_spawnPosY3;
}

public class Globals {
    public static readonly int      m_maxPlayers = 4;

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
