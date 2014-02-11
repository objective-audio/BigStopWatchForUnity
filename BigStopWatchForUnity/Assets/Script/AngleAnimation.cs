using UnityEngine;
using System.Collections;

public class AngleAnimation : MonoBehaviour {
	
	float fromAngle;
	float toAngle;
	float duration;
	float time = 0.0f;
	bool animating = false;
	bool isLerpAngle = false;
	
	void Update () {
		
		if (animating) {
			
			time += Time.deltaTime;
			
			float t = time / duration;
			
			if (t >= 1.0f) {
				t = 1.0f;
				animating = false;
			}
			
			t = BSWUtility.EaseOutValue(t);
			
			float angle = (isLerpAngle) ? Mathf.LerpAngle(fromAngle, toAngle, t) : Mathf.Lerp(fromAngle, toAngle, t);
			
			transform.localEulerAngles = new Vector3(0, 0, angle);
			
		}
		
	}
	
	public void SetAngle(float fromAngle, float toAngle, bool animate = false, float duration = 0.5f, bool isLerpAngle = true) {
		
		if (animate) {
			
			this.duration = duration;
			this.toAngle = toAngle;
			this.fromAngle = fromAngle;
			time = 0.0f;
			animating = true;
			this.isLerpAngle = isLerpAngle;
			
		} if (animating) {
			
			this.toAngle = toAngle;
			
		} else {
			
			transform.localEulerAngles = new Vector3(0, 0, toAngle);
			
		}
	}
	
	public void SetAngle(float angle, bool animate = false, float duration = 0.5f) {
		
		SetAngle(transform.localEulerAngles.z, angle, animate, duration);
		
	}
}