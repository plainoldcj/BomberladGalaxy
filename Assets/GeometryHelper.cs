using UnityEngine;
using System.Collections;

public class GeometryHelper : MonoBehaviour {

	public static Mesh CreatePlaneXY(int numEdgeVerts, float edgeLength, float zCoord = 0.0f) {
		int numVerts = numEdgeVerts * numEdgeVerts;
		
		float ds = edgeLength / (numEdgeVerts - 1);
		float dt = 1.0f / (numEdgeVerts - 1);
		
		Debug.Log("zCoord = " + zCoord + ", ds = " + ds);
		
		// create vertices
		
		// row-major grid layout
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] texCoords = new Vector2[numVerts];
		
		Vector3 pos = new Vector3(0.0f, 0.0f, zCoord);
		Vector2 tcoord = Vector2.zero;
		for(int i = 0; i < numEdgeVerts; ++i) {
			for(int j = 0; j < numEdgeVerts; ++j) {
				int vidx = numEdgeVerts * i + j;
				vertices[vidx] = pos;
				texCoords[vidx] = tcoord;
				pos.x += ds;
				tcoord.x += dt;
			}
			pos.x = 0.0f;
			pos.y -= ds;
			tcoord.x = 0.0f;
			tcoord.y += dt;
		}
		
		// create indices
		
		int numTris = 2 * (numEdgeVerts - 1) * (numEdgeVerts - 1);
		int numIndices = 3 * numTris;
		
		int[] indices = new int[numIndices];
		
		int cur = 0;
		for(int i = 0; i < numEdgeVerts - 1; ++i) {
			for(int j = 0; j < numEdgeVerts - 1; ++j) {
				indices[cur + 0] = numEdgeVerts * i + j;
				indices[cur + 1] = numEdgeVerts * i + (j + 1);
				indices[cur + 2] = numEdgeVerts * (i + 1) + j;
				
				indices[cur + 3] = numEdgeVerts * (i + 1) + j;
				indices[cur + 4] = numEdgeVerts * i + (j + 1);
				indices[cur + 5] = numEdgeVerts * (i + 1) + (j + 1);
				
				cur += 6;
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = texCoords;
		mesh.triangles = indices;
		
		return mesh;
	}

}
