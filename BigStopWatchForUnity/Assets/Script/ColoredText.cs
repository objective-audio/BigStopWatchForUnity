using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(MeshRenderer))]

public class ColoredText : MonoBehaviour {
	
	public string text = "empty";
	public Color color = Color.white;
	
	string prevText = null;
	Color prevColor = Color.clear;
	
	float duration = 0;
	bool animating = false;
	float fromAlpha;
	float toAlpha;
	float time = 0;
	
	TextMesh textMesh;
	
	// Use this for initialization
	void Start () {
		UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
		
		bool needsUpdate = false;
		
		if (animating) {
			
			time += Time.deltaTime;
			float t = time / duration;
			
			if (t >= 1.0f) {
				t = 1.0f;
				animating = false;
			}
			
			float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
			color.a = alpha;
			
			needsUpdate = true;
			
		} else if (IsPropertyChanged()) {
			
			needsUpdate = true;
			
		}
			
		if (needsUpdate) {
			UpdateText();
		}
	}
	
	bool IsPropertyChanged() {
		if (text != prevText ||
			color != prevColor) {
			return true;
		}
		return false;
	}
	
	void UpdateText() {
		
		if (textMesh == null) {
			textMesh = GetComponent<TextMesh>();
			textMesh.richText = true;
		}
		
		textMesh.text = HtmlColorText(text, color);
		
		prevText = text;
		prevColor = color;
	}
	
	public void SetAlpha(float alpha, float duration = 0.0f) {
		
		if (duration <= 0.0f) {
			
			animating = false;
			color.a = alpha;
			UpdateText();
			
		} else {
			
			fromAlpha = color.a;
			toAlpha = alpha;
			animating = true;
			time = 0;
			this.duration = duration;
		}
	}
	
	static private string HtmlColorText(string text, Color col) {
		
		int redValue = (int)(col.r * 255.0f);
		int greenValue = (int)(col.g * 255.0f);
		int blueValue = (int)(col.b * 255.0f);
		int alphaValue = (int)(col.a * 255.0f);
		string colorText = string.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", redValue, greenValue, blueValue, alphaValue);
		return string.Format("<color={0}>{1}</color>", colorText, text);
		
	}
}
