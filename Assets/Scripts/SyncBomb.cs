using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncBomb : NetworkBehaviour {

    public GameObject m_viewBombPrefab;

    public void SetMapPosition(Vector2 mapPos) {
        transform.position = new Vector3(mapPos.x, Globals.m_syncYOff, mapPos.y);
    }

	void Start () {
        // disables collisions with other syncobjects
        foreach (GameObject otherPlayer in GameObject.FindGameObjectsWithTag("TAG_SYNC_PLAYER"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
        }

	    // spawn viewbomb for this syncbomb
        GameObject viewBomb = Instantiate(m_viewBombPrefab);
        viewBomb.GetComponent<ViewBomb>().SetSyncBomb(gameObject);

        Destroy(gameObject, Globals.m_bombTimeout);
        Destroy(viewBomb, Globals.m_bombTimeout);
	}
	
	void Update () {
	
	}
}
