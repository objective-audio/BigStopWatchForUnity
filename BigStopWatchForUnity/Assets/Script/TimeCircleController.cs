using UnityEngine;
using System;
using System.Collections;

public class TimeCircleController : MonoBehaviour {

	public AngleAnimation secCircleAngleAnimation;
	public AngleAnimation minCircleAngleAnimation;
	public float circleAnimDuration = 0.2f;
	
	public AngleAnimation secNeedleAngleAnimation;
	public AngleAnimation minNeedleAngleAnimation;
	public float needleAnimDuration = 0.1f;
	
	public void SetTime(TimeSpan ts, bool animate = false)
	{
		float secAngle = (float)((ts.TotalMinutes - Math.Truncate(ts.TotalMinutes)) * 360.0);
		
		if (secCircleAngleAnimation) {
			secCircleAngleAnimation.SetAngle(secAngle, animate, circleAnimDuration);
		}
		
		if (secNeedleAngleAnimation) {
			secNeedleAngleAnimation.SetAngle(-secAngle, animate, needleAnimDuration);
		}
		
		float minAngle = (float)((ts.TotalHours - Math.Truncate(ts.TotalHours)) * 360.0);
		
		if (minCircleAngleAnimation) {
			minCircleAngleAnimation.SetAngle(minAngle, animate, circleAnimDuration);
		}
		
		if (minNeedleAngleAnimation) {
			minNeedleAngleAnimation.SetAngle(-minAngle, animate, needleAnimDuration);
		}
	}
}
