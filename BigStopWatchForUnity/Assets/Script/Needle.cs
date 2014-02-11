using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class Needle : MonoBehaviour {
	
	public Vector2 size = new Vector2(10.0f, 30.0f);
	public float bottomMargin = 4.0f;
	public Color color = Color.white;
	public int textureOffsetX = 0;
	public int textureOffsetY = 0;
	
	Vector2 prevSize = Vector2.zero;
	float prevBottomMargin = 0.0f;
	Color prevColor = Color.clear;
	int prevTextureOffsetX = -1;
	int prevTextureOffsetY = -1;
	
	MeshFilter meshFilter;
	
	void Start () {
		UpdateNeedle();
	}
	
	void Update () {
		
		if (IsNeedlePropertyChanged()) {
			UpdateNeedle();
		}
	}
	
	bool IsNeedlePropertyChanged()
	{
		if (size != prevSize ||
			color != prevColor ||
			bottomMargin != prevBottomMargin ||
			textureOffsetX != prevTextureOffsetX ||
			textureOffsetY != prevTextureOffsetY) {
			
			return true;
		}
		return false;
	}
	
	void UpdateNeedle()
	{
		if (meshFilter == null) {
			meshFilter = GetComponent<MeshFilter>();
		}
		
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null) return;
		
		Material mat = renderer.sharedMaterial;
		if (mat == null) return;
		
		Texture tex = mat.mainTexture;
		if (tex == null) return;
		
		Texture2D tex2d = (Texture2D)tex;
		if (tex2d == null) return;
		
		// Draw Texture
		
		int padding = 1;
		int border = 1;
		int scale = 1;
		
		float nHeight = size.y;
		float nHalfWidth = size.x * 0.5f;
		
		int drawHeight = (int)Mathf.Sqrt(nHeight * nHeight + nHalfWidth * nHalfWidth * 0.25f);
		float rad = Mathf.Atan2(nHalfWidth, nHeight);
		int drawWidth = Mathf.CeilToInt(nHeight * Mathf.Sin(rad));
		
		Vector2 drawSize = new Vector2(drawWidth, drawHeight);
		
		int texWidth = tex2d.width;
		int texHeight = tex2d.height;
		
		Rect clearRect = BSWUtility.CreateRectForClear(textureOffsetX, textureOffsetY, drawSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, clearRect, Color.clear);
		
		Rect drawRect = BSWUtility.CreateRectForDraw(textureOffsetX, textureOffsetY, drawSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, drawRect, color);
		
		float bottomRate = bottomMargin / size.y;
		Vector2[] needleUv = CreateUv(textureOffsetX, textureOffsetY, drawSize, texWidth, texHeight, padding, border, scale, bottomRate);
		
		tex2d.Apply();
		
		
		// Create Mesh
		
		float halfHeight = size.y * 0.5f;
		float halfWidth = size.x * 0.5f;
		float yBorder = Mathf.Sin(rad);
		float xBorder = Mathf.Cos(rad);
		float bottomY = - halfHeight + size.y * bottomRate;
		float bottomHalfWidth = halfWidth * bottomRate;
		
		Vector3[] vertices = new Vector3[] {
			
			// triangle 0 ~ 3
			new Vector3(0.0f, bottomY, 0.0f),
			new Vector3(- halfWidth, halfHeight, 0.0f),
			new Vector3(0.0f, halfHeight, 0.0f),
			new Vector3(halfWidth, halfHeight, 0.0f),
			
			// left border 4 ~ 5
			new Vector3(- bottomHalfWidth - xBorder, bottomY - yBorder, 0.0f),
			new Vector3(- xBorder - halfWidth, halfHeight - yBorder, 0.0f),
			
			// right border 6 ~ 7
			new Vector3(bottomHalfWidth + xBorder, bottomY - yBorder, 0.0f),
			new Vector3(xBorder + halfWidth, halfHeight - yBorder, 0.0f),
			
			// top border 8 ~ 10
			new Vector3(- halfWidth, halfHeight + border, 0.0f),
			new Vector3(0.0f, halfHeight + border, 0.0f),
			new Vector3(halfWidth, halfHeight + border, 0.0f),
			
			// bottom side 11 ~ 12
			new Vector3(- bottomHalfWidth, bottomY, 0.0f),
			new Vector3(bottomHalfWidth, bottomY, 0.0f),
			
			// bottom border 13 ~ 15
			new Vector3(- bottomHalfWidth, bottomY - border, 0.0f),
			new Vector3(bottomHalfWidth, bottomY - border, 0.0f),
			new Vector3(0.0f, bottomY - border, 0.0f)
		};
		
		int[] triangles = new int[] {
			// triangle
			11, 1, 2,
			11, 2, 0,
			0, 2, 3,
			0, 3, 12,
			// side
			4, 5, 11, 11, 5, 1,
			12, 3, 6, 6, 3, 7,
			// top
			1, 8, 2, 2, 8, 9,
			2, 9, 3, 3, 9, 10,
			// bottom
			13, 11, 0, 13, 0, 15,
			15, 0, 12, 15, 12, 14,
			// corner
			11, 13, 4,
			14, 12, 6,
			1, 5, 8,
			3, 10, 7
		};
		
		Vector2[] uv = new Vector2[] {
			// triangle
			needleUv[7],
			needleUv[1],
			needleUv[2],
			needleUv[1],
			// side
			needleUv[3],
			needleUv[4],
			needleUv[3],
			needleUv[4],
			// top
			needleUv[5],
			needleUv[6],
			needleUv[5],
			// bottom
			needleUv[0],
			needleUv[0],
			
			needleUv[8],
			needleUv[8],
			needleUv[9]
		};
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		// Keep properties
		
		prevSize = size;
		prevColor = color;
		prevTextureOffsetX = textureOffsetX;
		prevTextureOffsetY = textureOffsetY;
	}
	
	static public Vector2[] CreateUv(int originX, int originY, Vector2 squareSize, int texWidth, int texHeight, int padding, int border, int scale, float bottomRate) {
		
		float minX = (float)(originX + padding) / texWidth;
		float minY = (float)(originY + padding) / texHeight;
		float maxY = (float)(originY + padding + (border * 2 + squareSize.y) * scale) / (float)texHeight;
		
		float sqMinX = (float)(originX + padding + border * scale) / (float)texWidth;
		float sqMidX = (float)(originX + padding + (border + squareSize.x * bottomRate) * scale) / (float)texWidth;
		float sqMaxX = (float)(originX + padding + (border + squareSize.x - 1) * scale) / (float)texWidth;
		float sqMinY = (float)(originY + padding + border * scale) / (float)texHeight;
		float sqMaxY = (float)(originY + padding + (border + squareSize.y) * scale) / (float)texHeight;
		
		Vector2[] uv = new Vector2[] {
			new Vector2(sqMinX, sqMinY),
			new Vector2(sqMinX, sqMaxY),
			new Vector2(sqMaxX, sqMaxY),
			new Vector2(minX, sqMinY),
			new Vector2(minX, sqMaxY),
			new Vector2(sqMinX, maxY),
			new Vector2(sqMaxX, maxY),
			new Vector2(sqMidX, sqMinY),
			new Vector2(sqMinX, minY),
			new Vector2(sqMidX, minY),
		};
		
		return uv;
	}
}
