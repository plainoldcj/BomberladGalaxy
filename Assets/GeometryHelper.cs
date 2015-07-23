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
        
        
        /*
        we store adjacent vertices in (tangent,texcoord1), so normals
        may be computed in the shader. this is used by block mantles.
        */
        Vector4[] tangents = new Vector4[numVerts];
        Vector2[] texCoords1 = new Vector2[numVerts];
        
        for(int i = 0; i < numVertsY; ++i) {
            for(int j = 0; j < numVertsX; ++j) {
                int vidx = numVertsX * i + j;
                Vector3 v0, v1;
                if((numVertsY - 1 == i) || (numVertsX - 1 == j)) {
                    // just pretend block was slightly bigger. whatever.
                    v0 = vertices[vidx] + dsx * distort.MultiplyVector(Vector3.right);
                    v1 = vertices[vidx] + dsy * distort.MultiplyVector(Vector3.down);
                } else {
                    v0 = vertices[numVertsX * i + (j + 1)];
                    v1 = vertices[numVertsX * (i + 1) + j];
                }
                
                tangents[vidx].x    = v0.x;
                tangents[vidx].y    = v0.y;
                tangents[vidx].z    = v0.z;
                
                tangents[vidx].w    = v1.x;
                texCoords1[vidx].x  = v1.y;
                texCoords1[vidx].y  = v1.z;
            }
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
        mesh.uv2 = texCoords1;
        mesh.tangents = tangents;
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
