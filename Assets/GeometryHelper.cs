using UnityEngine;
using System.Collections;

public class GeometryHelper : MonoBehaviour {

	// TODO: parameter 'distort' unused at the moment
	public static Mesh CreatePlaneXY(int numVertsX, int numVertsY, float edgeLength, Matrix4x4 distort) {	
		int numVerts = numVertsX * numVertsY;
		
		float dsx = edgeLength / (numVertsX - 1);
		float dsy = edgeLength / (numVertsY - 1);
		float dtx = 1.0f / (numVertsX - 1);
		float dty = 1.0f / (numVertsY - 1);
		
		// create vertices
		
		// row-major grid layout
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] texCoords = new Vector2[numVerts];
		
		Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
		Vector2 tcoord = Vector2.zero;
		for(int i = 0; i < numVertsY; ++i) {
			for(int j = 0; j < numVertsX; ++j) {
				int vidx = numVertsX * i + j;
				vertices[vidx] = distort.MultiplyPoint(pos);
				texCoords[vidx] = tcoord;
				pos.x += dsx;
				tcoord.x += dtx;
			}
			pos.x = 0.0f;
			pos.y -= dsy;
			tcoord.x = 0.0f;
			tcoord.y += dty;
		}
		
		// create indices
		
		int numTris = 2 * (numVertsX - 1) * (numVertsY - 1);
		int numIndices = 3 * numTris;
		
		int[] indices = new int[numIndices];
		
		int cur = 0;
		for(int i = 0; i < numVertsY - 1; ++i) {
			for(int j = 0; j < numVertsX - 1; ++j) {
				indices[cur + 0] = numVertsX * i + j;
				indices[cur + 1] = numVertsX * i + (j + 1);
				indices[cur + 2] = numVertsX * (i + 1) + j;
				
				indices[cur + 3] = numVertsX * (i + 1) + j;
				indices[cur + 4] = numVertsX * i + (j + 1);
				indices[cur + 5] = numVertsX * (i + 1) + (j + 1);
				
				cur += 6;
			}
		}
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = texCoords;
		mesh.triangles = indices;
		
		// logging
		Debug.Log("CreatePlaneXY: " +
		          "numVertsX = " + numVertsX + ", " +
		          "numVertsY = " + numVertsY + ", " +
		          "edgeLength = " + edgeLength + ", " +
		          "ds = (" + dsx + ", " + dsy + ")");
		
		return mesh;
	}
	
}
