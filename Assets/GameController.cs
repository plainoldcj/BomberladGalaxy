using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(NetworkIdentity))]

public class GameController : NetworkBehaviour {

    private void CL_StartGame(int mapSeed) {
        GameObject.Find("Map").GetComponent<Map>().RpcCreate(mapSeed);
    }

    public override void OnStartServer ()
    {
        base.OnStartServer ();

        MSG_StartGame msg = new MSG_StartGame();
        msg.m_mapSeed = (int)System.DateTime.Now.Ticks;

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
                CL_StartGame(msg.m_mapSeed);
            }
        }
	}
}
