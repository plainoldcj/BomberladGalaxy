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

    public static readonly float    m_bombTimeout = 3.0f; // in secs, must be odd

    public static Vector2 MapPositionFromTilePosition(Vector2i tilePos) {
        return new Vector2(tilePos.x, -tilePos.y) * m_tileEdgeLength;
    }

    // wrap position to [0, mapSize]x[0, -mapSize]
    public static Vector2 WrapMapPosition(Vector2 mapPos) {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;

        while(mapPos.x > mapSize) mapPos.x -= mapSize;
        while(mapPos.x < 0.0f) mapPos.x += mapSize;
        while(mapPos.y < -mapSize) mapPos.y += mapSize;
        while(mapPos.y > 0.0f) mapPos.y -= mapSize;

        return mapPos;
    }

    public static Vector2i[] GetSpawnPositions() {
        if(20 != Globals.m_numTilesPerEdge || 4 != Globals.m_maxPlayers) {
            // at the moment, we just return positions
            // that look good for a certain map size
            Debug.Log("revisit spawn positions");
        }
        
        // place players somewhere in the middle of each quadrant
        Vector2i[] pos = {
            new Vector2i(5, 5),
            new Vector2i(14, 14),
            new Vector2i(5, 14),
            new Vector2i(14, 5)
        };
        
        return pos;
    }
}
