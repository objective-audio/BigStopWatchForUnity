using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class BackgroundColor : MonoBehaviour {
	
	public Color[] colors;
	public Color offColor = Color.black;
	float flashDuration = 1.0f;
	
	Color prevOffColor = Color.clear;
	
	bool animating = false;
	float duration;
	int colorIndex;
	float time;
	
	// Use this for initialization
	void Start () {
		
		if (camera != null) {
			
			camera.backgroundColor = offColor;
			
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (camera == null) return;
		
		if (animating) {
			
			time += Time.deltaTime;
			float t = time / flashDuration;
			
			if (t >= 1.0f) {
				t = 1.0f;
				animating = false;
			}
			
			camera.backgroundColor = Color.Lerp(colors[colorIndex], offColor, t);
			
		} else if (IsPropertyChanged()) {
			
			camera.backgroundColor = offColor;
			
			prevOffColor = offColor;
			
		}
	}
	
	public bool IsPropertyChanged() {
		
		if (offColor != prevOffColor) {
			
			return true;
		}
		
		return false;
		
	}
	
	public void Flash(int colorIndex) {
		
		if (colorIndex < 0 || colors.Length <= colorIndex) {
			return;
		}
		
		this.colorIndex = colorIndex;
		animating = true;
		time = 0.0f;
		
	}
}
