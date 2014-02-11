using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class TimeCircle : MonoBehaviour {
	
	public float lineRadius = 640.0f;
	public Vector2 largeLineSize = new Vector2(2.0f, 12.0f);
	public Vector2 smallLineSize = new Vector2(1.5f, 8.0f);
	public int largeLineCount = 60;
	public int partLineCount = 10;
	public Color largeLineColor = Color.white;
	public Color smallLineColor = Color.gray;
	public int textureOffsetX = 0;
	public int textureOffsetY = 0;
	
	float prevLineRadius = 0;
	Vector2 prevLargeLineSize = Vector2.zero;
	Vector2 prevSmallLineSize = Vector2.zero;
	int prevLargeLineCount = 0;
	int prevPartLineCount = 0;
	Color prevLargeLineColor = Color.clear;
	Color prevSmallLineColor = Color.clear;
	int prevTextureOffsetX = -1;
	int prevTextureOffsetY = -1;
	
	public GameObject numberPrefab;
	public float numberRadius = 620.0f;
	
	float prevNumberRadius = 0;
	
	GameObject numberRoot = null;
	string numberRootName = "NumberRoot";
	
	MeshFilter meshFilter;
	
	void Start () {
		
		UpdateLines();
		UpdateNumbers();
		
	}
	
	void Update () {
		
		if (IsLinePropertyChanged()) {
			
			UpdateLines();
		}
		
		if (IsNumberPropertyChanged()) {
			
			UpdateNumbers();
			
		}
	}
	
	bool IsLinePropertyChanged() {
		
		if (lineRadius != prevLineRadius ||
			largeLineSize != prevLargeLineSize ||
			smallLineSize != prevSmallLineSize ||
			largeLineCount != prevLargeLineCount ||
			partLineCount != prevPartLineCount ||
			largeLineColor != prevLargeLineColor ||
			smallLineColor != prevSmallLineColor ||
			textureOffsetX != prevTextureOffsetX ||
			textureOffsetY != prevTextureOffsetY) {
			
			return true;
		}
		
		return false;
	}
	
	bool IsNumberPropertyChanged() {
		
		if (numberRadius != prevNumberRadius) {
			
			return true;
		}
		
		return false;
	}
	
	void UpdateLines()
	{
		if (meshFilter == null) {
			meshFilter = GetComponent<MeshFilter>();
		}
		
		Mesh mesh = meshFilter.sharedMesh;
		
		if (mesh == null) return;
		
		mesh.Clear();
		
		Material mat = renderer.sharedMaterial;
		
		if (mat == null) return;
		
		Texture tex = mat.mainTexture;
		
		if (tex == null) return;
		
		Texture2D tex2d = (Texture2D)mat.mainTexture;
		
		if (tex2d == null) return;
		
		// Draw Texture
		
		int padding = 1;
		int border = 1;
		int scale = 2;
		int originX = textureOffsetX;
		int originY = textureOffsetY;
		int texWidth = tex2d.width;
		int texHeight = tex2d.height;
		
		Rect largeClearRect = BSWUtility.CreateRectForClear(originX, originY, largeLineSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, largeClearRect, Color.clear);
		
		Rect largeDrawRect = BSWUtility.CreateRectForDraw(originX, originY, largeLineSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, largeDrawRect, largeLineColor);
		
		Vector2[] largeUv = BSWUtility.CreateUv(originX, originY, largeLineSize, texWidth, texHeight, padding, border, scale);
		
		originX += (int)(padding * 2 + (largeLineSize.x + border * 2) * scale);
		
		Rect smallClearRect = BSWUtility.CreateRectForClear(originX, originY, smallLineSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, smallClearRect, Color.clear);
		
		Rect smallDrawRect = BSWUtility.CreateRectForDraw(originX, originY, smallLineSize, padding, border, scale);
		BSWUtility.DrawRect(tex2d, smallDrawRect, smallLineColor);
		
		Vector2[] smallUv = BSWUtility.CreateUv(originX, originY, smallLineSize, texWidth, texHeight, padding, border, scale);
		
		tex2d.Apply();
		
		renderer.sharedMaterial.mainTexture = tex2d;
		
		// Create Mesh
		
		int lineCount = largeLineCount * partLineCount;
		int vertexCount = lineCount * 4;
		int triangleCount = lineCount * 6;
		
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[triangleCount];
		Vector2[] uv = new Vector2[vertexCount];
		
		Vector3[] largeLinePos = CreateLinePositions(largeLineSize, border, lineRadius);
		Vector3[] smallLinePos = CreateLinePositions(smallLineSize, border, lineRadius);
		
		for (int i = 0; i < lineCount; i++) {
			
			int vTopIndex = i * 4;
			int tTopIndex = i * 6;
			
			float angle = - ((float)i / lineCount) * 360.0f;
			var rot = Quaternion.Euler(0, 0, angle);
       		var m = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
			
			Vector3[] currentPos;
			Vector2[] currentUv;
			
			if (i % partLineCount == 0) {
				
				currentPos = largeLinePos;
				currentUv = largeUv;
			
			} else {
				
				currentPos = smallLinePos;
				currentUv = smallUv;
				
			}
			
			for (int j = 0; j < 4; j++) {
				
				int index = vTopIndex + j;
				vertices[index] = m.MultiplyPoint3x4(currentPos[j]);
				uv[index] = currentUv[j];
				
			}
			
			triangles[tTopIndex] = vTopIndex;
			triangles[tTopIndex + 2] = triangles[tTopIndex + 3] = vTopIndex + 1;
			triangles[tTopIndex + 1] = triangles[tTopIndex + 4] = vTopIndex + 2;
			triangles[tTopIndex + 5] = vTopIndex + 3;
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		// Keep Properties
		
		prevLineRadius = lineRadius;
		prevLargeLineSize = largeLineSize;
		prevSmallLineSize = smallLineSize;
		prevLargeLineCount = largeLineCount;
		prevPartLineCount = partLineCount;
		prevLargeLineColor = largeLineColor;
		prevSmallLineColor = smallLineColor;
		prevTextureOffsetX = textureOffsetX;
		prevTextureOffsetY = textureOffsetY;
	}
	
	static public Vector3[] CreateLinePositions(Vector2 lineSize, int border, float radius) {
		
		float minX = - lineSize.x * 0.5f - border;
		float maxX = lineSize.x * 0.5f + border;
		float minY = - lineSize.y * 0.5f - border + radius;
		float maxY = lineSize.y * 0.5f + border + radius;
		
		return new Vector3[] {
			new Vector3(minX, minY, 0.0f),
			new Vector3(maxX, minY, 0.0f),
			new Vector3(minX, maxY, 0.0f),
			new Vector3(maxX, maxY, 0.0f)
		};
		
	}
	
	void UpdateNumbers()
	{
		if (numberPrefab == null)
			return;
		
		foreach (Transform child in transform) {
			if (child.gameObject.name == numberRootName) {
				DestroyImmediate(child.gameObject);
			}
		}
		
		numberRoot = new GameObject(numberRootName);
		numberRoot.transform.parent = transform;
		numberRoot.transform.localScale = Vector3.one;
		numberRoot.transform.localPosition = Vector3.zero;
		numberRoot.transform.localRotation = Quaternion.identity;
		
		for (int i = 0; i < largeLineCount; i++) {
			
			GameObject handleObj = new GameObject("NumberHandle_"+i);
			handleObj.transform.parent = numberRoot.transform;
			handleObj.transform.localRotation = Quaternion.Euler(0, 0, - (float)i / largeLineCount * 360.0f);
			handleObj.transform.localScale = Vector3.one;
			handleObj.transform.localPosition = Vector3.zero;
			
			GameObject numObj = Instantiate(numberPrefab) as GameObject;
			numObj.transform.parent = handleObj.transform;
			numObj.transform.localScale = Vector3.one;
			numObj.transform.localRotation = Quaternion.identity;
			
			TextMesh textMesh = numObj.GetComponent<TextMesh>();
			
			if (textMesh != null) {
				
				textMesh.text = i.ToString();
				numObj.name = "Number_" + textMesh.text;
				textMesh.transform.localPosition = new Vector3(0, numberRadius, 0);
				
			}
		}
		
		prevNumberRadius = numberRadius;
	}
}
