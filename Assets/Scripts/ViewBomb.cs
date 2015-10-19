using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ViewBomb : MonoBehaviour {

    public Vector3 m_position = Vector3.zero;
    public Vector3 m_rotation = Vector3.zero;
    public Vector3 m_scale = Vector3.one;

    public GameObject m_explosionPrefab1;
    public GameObject m_explosionPrefab2;
    public GameObject m_explosionPrefab3;
    public GameObject m_explosionPrefab4;
    public GameObject m_explosionPrefab5;

    public GameObject m_explosionLightPrefab;
    public GameObject m_fuseParticlesPrefab;

    private GameObject  m_syncBomb;
    private GameObject  m_mapOrigin;
    private float       m_time = 0.0f;
    private Vector2     m_lastMapPos = Vector2.zero;
    private GameObject  m_fuseParticles;

    private GameObject GetExplosionPrefabForRange(int range)
    {
        GameObject[] prefabs =
        {
            m_explosionPrefab1,
            m_explosionPrefab2,
            m_explosionPrefab3,
            m_explosionPrefab4,
            m_explosionPrefab5
        };
        Assert.IsTrue(1 <= range && range <= 5);
        return prefabs[range - 1];
    }

    public void SetSyncBomb(GameObject syncBomb) {
        m_syncBomb = syncBomb;
    }

	// Use this for initialization
	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");

        GetComponent<Renderer>().material.EnableKeyword("ENABLE_RIM_LIGHTING");

        m_fuseParticles = Instantiate(m_fuseParticlesPrefab);
        m_fuseParticles.transform.parent = transform;

        // forces the gameobject to be placed at a sensible position,
        // prevents spurious spawns around local player
        Update ();
        m_fuseParticles.GetComponent<FuseParticles>().DoUpdate();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_syncBomb == null) {
			return;
		}
        m_time += Time.deltaTime;
        
        float l = Mathf.Abs(Mathf.Sin(0.5f * Mathf.PI * m_time));
        
        float t = m_time / Globals.m_bombTimeout;
        Color bodyColor = Color.black;
        Color c1 = Color.Lerp(bodyColor, Color.red, t);
        
        float s1 = 1.0f + 0.5f * t;
        
        float s = Mathf.Lerp(1.0f, s1, l);
        m_scale = new Vector3(s, s, s);
        
        GetComponent<Renderer>().materials[0].color = Color.Lerp(bodyColor, c1, l);
        
        Quaternion rotation = 
            Quaternion.AngleAxis(90.0f * Time.time, new Vector3(1.0f, 1.0f, 0.0f)) *
            Quaternion.AngleAxis(90.0f * Time.time, new Vector3(0.0f, 0.0f, 1.0f)) *
            Quaternion.AngleAxis(-90.0f, new Vector3(1.0f, 0.0f, 0.0f));

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        
        // copy position from my syncbomb and wrap it
        Vector2 mapPos = Globals.WrapMapPosition(new Vector2(
            m_syncBomb.transform.position.x,
            m_syncBomb.transform.position.z));
        m_lastMapPos = mapPos;
        
        // this moves the ground map so that it initially fills the entire mapping domain
        Matrix4x4 offset = Matrix4x4.TRS(
            new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, 1.0f));
        
        Vector3 localOffset = new Vector3(0.5f, 0.0f, 0.0f);
        Matrix4x4 localToMap = Matrix4x4.TRS(
            localOffset + new Vector3(mapPos.x, mapPos.y, 0.5f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, -1.0f));

        Matrix4x4 localTransform = Matrix4x4.TRS(
            m_position, Quaternion.Euler(m_rotation) * rotation, m_scale);
        
        Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap * localTransform;

        m_fuseParticles.GetComponent<FuseParticles>().localToWorld = localToWorld;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}

    /*
    returns the range of an explosion in map direction 'dir'. value
    might be less than the bomb's explosion range if a stone block
    is in the way.
    */
    private int GetExplosionRange(Vector2 dir)
    {
        return 0;
    }

    public void CreateExplosion() {
        GameObject explosionLight = Instantiate(m_explosionLightPrefab);
        explosionLight.GetComponent<ExplosionLight>().SetMapPosition(m_lastMapPos);
        Destroy(explosionLight, Globals.m_explosionTimeout);

        // create explosion in all four directions

        // note that both mapY and mapDir rotate cw
        float mapY = 0.0f;
        int mapDir = (int)Globals.MapDirection.LEFT;

        Assert.IsNotNull(m_syncBomb);
        SyncBomb scr_syncBomb = m_syncBomb.GetComponent<SyncBomb>();
        Map scr_map = GameObject.Find("Map").GetComponent<Map>();

        for(int i = 0; i < 4; ++i)
        {
            int range = scr_syncBomb.GetExplosionRange();

            SyncBomb.TouchInfo inf;
            if(scr_syncBomb.TouchesBlock((Globals.MapDirection)mapDir, out inf))
            {
                Block.Type type = scr_map.GetBlockType(inf.tilePosition);
                if(Block.Type.Wood == type)
                {
                    range = inf.distance;
                }
                else
                {
                    Assert.IsTrue(Block.Type.Stone == type);
                    range = inf.distance - 1;
                }
            }

            if (0 < range)
            {
                GameObject explosion = Instantiate(GetExplosionPrefabForRange(range));
                Explosion scr_explosion = explosion.GetComponent<Explosion>();
                scr_explosion.SetMapPosition(m_lastMapPos);
                scr_explosion.m_rotation.z = mapY;
                Destroy(explosion, Globals.m_explosionTimeout);
            }

            mapY += 90.0f;
            mapDir = (mapDir + 1) % 4;
        }
    }
}
