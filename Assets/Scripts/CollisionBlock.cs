using UnityEngine;
using System.Collections;

public class CollisionBlock : MonoBehaviour {

    private static Mesh m_mantleMesh = null;

    public Vector2i tilePosition { get; set; }

    private static void TouchMantleMesh() {
        if(null == m_mantleMesh) {
            m_mantleMesh = GeometryHelper.CreateBlockMantleMesh(2);
            m_mantleMesh.name = "mantle";
        }
    }

	void Start () {
        TouchMantleMesh();
        GetComponent<MeshFilter>().mesh = m_mantleMesh;

        // fit bounding box to mesh
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = m_mantleMesh.bounds.center;
        boxCollider.size = m_mantleMesh.bounds.size;
	}
	
	void Update () {
	
	}
}
