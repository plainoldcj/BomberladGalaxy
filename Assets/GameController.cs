using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(NetworkIdentity))]

public class GameController : NetworkBehaviour {

    private void CL_StartGame(int mapSeed, Vector2i[] spawnPos) {
        GameObject.Find("Map").GetComponent<Map>().Create(mapSeed, spawnPos);
    }

    static Vector2i[] CreateSpawnPositions() {
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

    static void EncodeSpawnPositions(Vector2i[] pos, MSG_StartGame msg) {
        Assert.AreEqual(4, pos.Length);

        msg.m_spawnPosX0 = (byte)pos[0].x;
        msg.m_spawnPosX1 = (byte)pos[1].x;
        msg.m_spawnPosX2 = (byte)pos[2].x;
        msg.m_spawnPosX3 = (byte)pos[3].x;

        msg.m_spawnPosY0 = (byte)pos[0].y;
        msg.m_spawnPosY1 = (byte)pos[1].y;
        msg.m_spawnPosY2 = (byte)pos[2].y;
        msg.m_spawnPosY3 = (byte)pos[3].y;
    }

    static Vector2i[] DecodeSpawnPositions(MSG_StartGame msg) {
        Vector2i[] pos = {
            new Vector2i(msg.m_spawnPosX0, msg.m_spawnPosY0),
            new Vector2i(msg.m_spawnPosX1, msg.m_spawnPosY1),
            new Vector2i(msg.m_spawnPosX2, msg.m_spawnPosY2),
            new Vector2i(msg.m_spawnPosX3, msg.m_spawnPosY3)
        };
        return pos;
    }

    public override void OnStartServer ()
    {
        base.OnStartServer ();

        Vector2i[] spawnPos = CreateSpawnPositions();

        // place players on spawn positions

        for(int i = 0; i < NetworkClient.allClients.Count; ++i) {
            NetworkConnection conn = NetworkClient.allClients[i].connection;

            GameObject player = conn.playerControllers[0].gameObject;
            Assert.Equals("Player", player.tag);

            player.GetComponent<Player>().SV_SetTilePosition(spawnPos[i]);
        }

        // send "start game" message

        MSG_StartGame msg = new MSG_StartGame();
        msg.m_mapSeed = (int)System.DateTime.Now.Ticks;
        EncodeSpawnPositions(spawnPos, msg);

        NetworkServer.SendToAll(MessageTypes.m_startGame, msg);
    }

	void Start () {
	}
	
    [ClientCallback]
	void Update () {
        short msgType = -1;
        MessageBase msgBase = null;
        if(MessageQueue.Instance.PopMessage(out msgType, out msgBase)) {
            if(MessageTypes.m_startGame == msgType) {
                MSG_StartGame msg = (MSG_StartGame)msgBase;
                CL_StartGame(msg.m_mapSeed, DecodeSpawnPositions(msg));
            }
        }
	}
}
