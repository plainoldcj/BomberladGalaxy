using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/*
The idea of having an additional message queue is to ...
- register message handlers as early as possible, so that no messages are lost
- buffer messages, so that they can be processed at an appropriate time

The message queue is initialized by calling OnClientConnected() in
GuiLobbyControllers.cs, when a (local) client is created.

The buffered messages are processed in the update loop in GameController.cs.
*/

public class MessageQueue {

    private class TypedMessage {
        public readonly short       m_type;
        public readonly MessageBase m_message;
    
        public TypedMessage(short type, MessageBase message) {
            m_type = type;
            m_message = message;
        }
    }

    private List<TypedMessage> m_messages = new List<TypedMessage>();

    private static MessageQueue m_theInstance = null;

    public static MessageQueue Instance {
        get {
            if(null == m_theInstance) {
                m_theInstance = new MessageQueue();
            }
            return m_theInstance;
        }
    }

    private MessageQueue() { }

    private void Push_StartGame(NetworkMessage netMsg) {
        m_messages.Add (new TypedMessage(MessageTypes.m_startGame, netMsg.ReadMessage<MSG_StartGame>()));
    }

    private void Push_DestroyBlock(NetworkMessage netMsg) {
        m_messages.Add(new TypedMessage(MessageTypes.m_destroyBlock, netMsg.ReadMessage<MSG_DestroyBlock>()));
    }

	public void OnClientConnected(NetworkConnection conn) {
        conn.RegisterHandler(MessageTypes.m_startGame, Push_StartGame);
        conn.RegisterHandler(MessageTypes.m_destroyBlock, Push_DestroyBlock);
    }
	public void OnServerConnected() {
		NetworkServer.RegisterHandler (MessageTypes.m_startGame, Push_StartGame);
		NetworkServer.RegisterHandler (MessageTypes.m_destroyBlock, Push_StartGame);
	}

    public bool PopMessage(out short msgType, out MessageBase msgBase) {
        if(0 < m_messages.Count) {
            msgType = m_messages[0].m_type;
            msgBase = m_messages[0].m_message;
            m_messages.RemoveAt(0);
            return true;
        }
        msgType = -1;
        msgBase = null;
        return false;
    }

}
