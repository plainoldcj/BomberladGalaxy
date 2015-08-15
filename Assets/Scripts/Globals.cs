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
    public static readonly float    m_explosionTimeout = 3.0f;
    public static readonly float    m_explosionFadeIn = 0.2f;

    // y-coordinate of Sync* gameobjects, which keeps their colliders away
    // from the collision map placed at y=0
    public static readonly float    m_syncYOff = -100.0f;

    public static readonly Vector2[] m_mapGridOffsets;

    static Globals() {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        m_mapGridOffsets = new Vector2[] {
            new Vector2(0.0f, 0.0f),
            new Vector2(-mapSize,  0.0f),
            new Vector2( mapSize,  0.0f),
            new Vector2( 0.0f,    -mapSize),
            new Vector2( 0.0f,     mapSize),
            new Vector2(-mapSize,  mapSize),
            new Vector2( mapSize,  mapSize),
            new Vector2(-mapSize, -mapSize),
            new Vector2( mapSize, -mapSize)
        };
    }

    public static Vector2 MapPositionFromTilePosition(Vector2i tilePos) {
        return new Vector2(tilePos.x, -tilePos.y) * m_tileEdgeLength;
    }

    public static Vector2 TileCenterFromMapPosition(Vector2 mapPos)
    {
        /*
        use this when rendering z-offset is fixed

        return new Vector2(
            (int)(mapPos.x + 0.5f * m_tileEdgeLength),
            (int)(mapPos.y - 0.5f * m_tileEdgeLength));
        */
        return new Vector2(
            (int)(mapPos.x + 0.5f * m_tileEdgeLength),
            (int)mapPos.y - 0.5f * m_tileEdgeLength);
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

    // *must* be consistent with shader code
    public static Vector3 VS_Warp(Vector3 pos) {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        float _MappingDomain = 0.5f * mapSize;

        if(pos.x > _MappingDomain) pos.x -= 2.0f * _MappingDomain;
        if(pos.y > _MappingDomain) pos.y -= 2.0f * _MappingDomain;
        if(pos.x < -_MappingDomain) pos.x += 2.0f * _MappingDomain;
        if(pos.y < -_MappingDomain) pos.y += 2.0f * _MappingDomain;
        return pos;
    }

    // *must* be consistent with shader code
    public static Vector3 VS_MapOnSphere(Vector3 pos) {
        float zOff = pos.z;

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        float _MappingDomain = 0.5f * mapSize;
        
        Vector2 pos2 = (1.0f / _MappingDomain) * new Vector2(pos.x, pos.y);
        
        float r = Mathf.Min(pos2.magnitude, 1.0f);
        
        float phi = Mathf.Atan2(pos2.y, pos2.x);
        float theta = Mathf.PI * r;

        float _SphereRadius = 5.0f;

        return (_SphereRadius + zOff) * new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi), -Mathf.Cos(theta));
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
