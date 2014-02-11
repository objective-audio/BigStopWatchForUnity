using UnityEngine;
using System.Collections;

public class BSWUtility : MonoBehaviour {

	static public void DrawRect(Texture2D tex, Rect rect, Color col) {
		
		col.r *= col.a;
		col.g *= col.a;
		col.b *= col.a;
		
		int minX = (int)rect.x;
		int minY = (int)rect.y;
		int maxX = (int)rect.xMax;
		int maxY = (int)rect.yMax;
		
		for (int x = minX; x < maxX; x++) {
			for (int y = minY; y < maxY; y++) {
				tex.SetPixel(x, y, col);
			}
		}
	}
	
	static public Vector2[] CreateUv(int originX, int originY, Vector2 lineSize, int texWidth, int texHeight, int padding, int border, int scale) {
		
		float minX = (float)(originX + padding) / texWidth;
		float maxX = (float)(originX + padding + (border * 2 + lineSize.x) * scale) / (float)texWidth;
		float minY = (float)(originY + padding) / texHeight;
		float maxY = (float)(originY + padding + (border * 2 + lineSize.y) * scale) / (float)texHeight;
		
		Vector2[] uv = new Vector2[] {
			new Vector2(minX, minY),
			new Vector2(maxX, minY),
			new Vector2(minX, maxY),
			new Vector2(maxX, maxY)
		};
		
		return uv;
	}
	
	static public Rect CreateRectForClear(int originX, int originY, Vector2 lineSize, int padding, int border, int scale) {
		
		return new Rect(
			originX, 
			originY,
			padding * 2 + (lineSize.x + border * 2) * scale,
			padding * 2 + (lineSize.y + border * 2) * scale);
	}
	
	static public Rect CreateRectForDraw(int originX, int originY, Vector2 lineSize, int padding, int border, int scale) {
		
		return new Rect(
			originX + padding + border * scale, 
			originY + padding + border * scale,
			lineSize.x * scale,
			lineSize.y * scale);
	}
	
	static public float EaseOutValue(float t) {
		
		return Mathf.Sin(Mathf.PI * 0.5f * t);
		
	}
}
