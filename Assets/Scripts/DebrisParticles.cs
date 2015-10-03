using UnityEngine;
using System.Collections;

public class DebrisParticles : MonoBehaviour {

    public Vector3                      m_gravity = Vector3.zero;

    private GameObject                  m_mapOrigin;

    private Vector2                     m_mapPos = Vector2.zero;
    private ParticleSystem.Particle[]   m_particles;
    private Vector3[]                   m_axis;

    public Vector2 mapPos
    {
        get { return m_mapPos; }
        set { m_mapPos = Globals.WrapMapPosition(value); }
    }

	void Start () {
        m_mapOrigin = GameObject.Find("MapOrigin");

        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        m_particles = new ParticleSystem.Particle[particleSystem.maxParticles];

        m_axis = new Vector3[particleSystem.maxParticles];
        for (int i = 0; i < particleSystem.maxParticles; ++i)
        {
            m_axis[i] = Random.onUnitSphere;
        }

        // forces the gameobject to be placed at a sensible position,
        // prevents spurious spawns around local player
        Update ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_mapOrigin == null) {
			return;
		}
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        int numParticles = particleSystem.GetParticles(m_particles);
        for (int i = 0; i < numParticles; ++i)
        {
            m_particles[i].axisOfRotation = m_axis[i];
            m_particles[i].velocity += m_gravity * Time.deltaTime;
        }
        particleSystem.SetParticles(m_particles, numParticles);

        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        
        // this moves the ground map so that it initially fills the entire mapping domain
        Matrix4x4 offset = Matrix4x4.TRS(
            new Vector3(-0.5f * mapSize, 0.5f * mapSize, 0.0f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, 1.0f));
        
        Vector3 localOffset = new Vector3(0.5f, -0.5f, 0.0f);
        Matrix4x4 localToMap = Matrix4x4.TRS(
            localOffset + new Vector3(mapPos.x, mapPos.y, 0.5f),
            Quaternion.identity,
            new Vector3(1.0f, 1.0f, -1.0f));
        
        Matrix4x4 localToWorld = m_mapOrigin.transform.localToWorldMatrix * offset * localToMap;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld);
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
	}
}
