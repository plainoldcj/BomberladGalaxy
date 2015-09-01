using UnityEngine;
using System.Collections;

public class FuseParticles : MonoBehaviour {

    // local offset to viewbomb
    public Vector3 m_position = Vector3.zero;

    private static Mesh m_particleMesh = null;

    // mesh particles are billboards that get expanded in vertex shader
    private static Mesh CreateParticleMesh()
    {
        Vector3[] vertices = {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };

        Vector2[] texCoords = {
            new Vector2(-1.0f, -1.0f),
            new Vector2(-1.0f,  1.0f),
            new Vector2( 1.0f,  1.0f),
            new Vector2( 1.0f, -1.0f)
        };

        int[] tris = { 0, 1, 2, 0, 2, 3 };

        Mesh mesh = new Mesh();
        mesh.name = "particle";
        mesh.vertices = vertices;
        mesh.uv = texCoords;
        mesh.triangles = tris;

        return mesh;
    }

    private static void TouchParticleMesh()
    {
        if (null == m_particleMesh)
        {
            m_particleMesh = CreateParticleMesh();
        }
    }

    public Matrix4x4 localToWorld
    {
        get; set;
    }

    public void DoUpdate()
    {
        float mapSize = Globals.m_tileEdgeLength * Globals.m_numTilesPerEdge;
        
        GetComponent<Renderer>().material.SetMatrix("_LocalToWorld", localToWorld * Matrix4x4.TRS(m_position, Quaternion.identity, Vector3.one));
        GetComponent<Renderer>().material.SetFloat("_MappingDomain", 0.5f * mapSize);
    }

    void Start()
    {
        TouchParticleMesh();

        GetComponent<ParticleSystemRenderer>().mesh = m_particleMesh;
    }
	
	void Update () {
        DoUpdate();
	}
}
