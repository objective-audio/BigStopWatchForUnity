using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class CircleMesh : MonoBehaviour {
	
	public float radius = 50.0f;
	public int triangleCount = 6;
	public int textureOffsetX = 40;
	public int textureOffsetY = 0;
	
	float prevRadius = 0.0f;
	int prevTriangleCount = 0;
	int prevTextureOffsetX = -1;
	int prevTextureOffsetY = -1;
	Vector2 uvPoint;
	
	MeshFilter meshFilter;
	
	// Use this for initialization
	void Start () {
		UpdateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if (IsPropertyChanged()) {
			UpdateMesh();
		}
	}
	
	bool IsPropertyChanged() {
		
		if (radius != prevRadius ||
			triangleCount != prevTriangleCount ||
			textureOffsetX != prevTextureOffsetX ||
			textureOffsetY != prevTextureOffsetY) {
			
			return true;
		}
		
		return false;
	}
	
	void UpdateMesh() {
		
		if (meshFilter == null) {
			meshFilter = GetComponent<MeshFilter>();
		}
		
		Mesh mesh = meshFilter.sharedMesh;
		
		if (mesh == null) {
			return;
		}
		
		mesh.Clear();
		
		Material mat = renderer.sharedMaterial;
		
		if (mat == null) {
			return;
		}
		
		Texture tex = mat.mainTexture;
		
		if (tex == null) {
			return;
		}
		
		Texture2D tex2d = (Texture2D)tex;
		
		if (tex2d == null) {
			return;
		}
		
		// Draw Texture
		
		int texWidth = tex2d.width;
		int texHeight = tex2d.height;
		Vector2 size = Vector2.zero;
		int padding = 1;
		int border = 0;
		int scale = 1;
		
		uvPoint = new Vector2(
			(float)(textureOffsetX + padding) / (float)texWidth, 
			(float)(textureOffsetY + padding) / (float)texHeight);
		
		Rect clearRect = BSWUtility.CreateRectForClear(textureOffsetX, textureOffsetY, size, padding, border, scale);
		BSWUtility.DrawRect(tex2d, clearRect, Color.clear);
		
		tex2d.Apply();
		
		// Create Mesh
		
		if (triangleCount < 1) {
			triangleCount = 1;
		}
		
		int verticesCount = triangleCount + 2;
		Vector3[] vertices = new Vector3[verticesCount];
		int[] triangles = new int[triangleCount * 3];
		Vector2[] uv = new Vector2[verticesCount];
		
		for (int i = 0; i < verticesCount; i++) {
			
			float phase = Mathf.PI * 2.0f / (float)verticesCount * (float)i;
			vertices[i] = new Vector3(Mathf.Sin(phase) * radius, Mathf.Cos(phase) * radius, 0.0f);
			uv[i] = uvPoint;
			
		}
		
		for (int i = 0; i < triangleCount; i++) {
			
			triangles[i * 3] = 0;
			triangles[i * 3 + 1] = i + 1;
			triangles[i * 3 + 2] = (i + 2) % verticesCount;
			
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		prevRadius = radius;
		prevTriangleCount = triangleCount;
		prevTextureOffsetX = textureOffsetX;
		prevTextureOffsetY = textureOffsetY;
	}
}
