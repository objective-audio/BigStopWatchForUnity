using UnityEngine;
using System;
using System.Collections;

public class Stopwatch : MonoBehaviour {
	
	enum StopwatchState {
		Zero,
		Play,
		Pause
	}
	
	public TextMesh timeText;
	public TimeCircleController timeCircleController;
	
	public AngleAnimation resetButtonAngleAnimation;
	public ColoredText resetButtonText;
	public float buttonAnimDuration = 0.2f;
	
	public BackgroundColor bgColor;
	
	StopwatchState state = StopwatchState.Zero;
	TimeSpan lastStopTimeSpan;
	DateTime startDateTime;
	
	void Awake() {
		Load();
	}
	
	void OnApplicationQuit() {
		PlayerPrefs.Save();
	}
	
	const string lastStopTimeKey = "LastStopTime";
	const string startDateTimeKey = "StartDateTime";
	const string stateKey = "State";
	
	void Save() {
		string lastStopTimeString = lastStopTimeSpan.Ticks.ToString();
		string dateTimeString = startDateTime.Ticks.ToString();
		PlayerPrefs.SetString(lastStopTimeKey, lastStopTimeString);
		PlayerPrefs.SetString(startDateTimeKey, dateTimeString);
		PlayerPrefs.SetInt(stateKey, (int)state);
	}
	
	void Load() {
		
		if (PlayerPrefs.HasKey(lastStopTimeKey) && 
			PlayerPrefs.HasKey(startDateTimeKey) && 
			PlayerPrefs.HasKey(stateKey)) {
			
			string lastStopTimeString = PlayerPrefs.GetString(lastStopTimeKey);
			if (!string.IsNullOrEmpty(lastStopTimeString)) {
				lastStopTimeSpan = new TimeSpan(long.Parse(lastStopTimeString));
			}
			
			string dateTimeString = PlayerPrefs.GetString(startDateTimeKey);
			if (!string.IsNullOrEmpty(dateTimeString)) {
				startDateTime = new DateTime(long.Parse(dateTimeString));
			}
			
			state = (StopwatchState)PlayerPrefs.GetInt(stateKey);
		}
	}
	
	void Start () {
		float alpha = (state == StopwatchState.Pause) ? 1.0f : 0.0f;
		resetButtonText.SetAlpha(alpha, 0.0f);
	}
	
	void Update () {
		
		bool circleAnim = false;
		
		if (Input.GetMouseButtonDown(0)) {
			ChangeState(ref circleAnim);
		}
		
		UpdateTime(circleAnim);
	}
	
	void ChangeState(ref bool circleAnim) {
		
		ButtonType buttonType = ButtonType.Background;
			
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, 2.0f)) {
			Button button = hit.collider.gameObject.GetComponent<Button>();
			if (button != null) {
				buttonType = button.type;
			}
		}
		
		if (buttonType == ButtonType.Reset && state == StopwatchState.Pause) {
			
			lastStopTimeSpan = new TimeSpan(0);
			startDateTime = DateTime.UtcNow;
			
			state = StopwatchState.Zero;
			
			circleAnim = true;
			
			SetVisibleResetButton(false);
			FlashBackground(1);
			
		} else if (state == StopwatchState.Play) {
			
			TimeSpan ts = DateTime.UtcNow - startDateTime;
			lastStopTimeSpan = ts + lastStopTimeSpan;
			
			state = StopwatchState.Pause;
			
			SetVisibleResetButton(true);
			FlashBackground(0);
			
		} else {
			
			startDateTime = DateTime.UtcNow;
			
			state = StopwatchState.Play;
			
			SetVisibleResetButton(false);
			FlashBackground(0);
			
		}
		
		Save();
	}
	
	void UpdateTime(bool circleAnim) {
		
		TimeSpan currentTs;
		
		if (state == StopwatchState.Play) {
			
			TimeSpan ts = DateTime.UtcNow - startDateTime;
			currentTs = ts + lastStopTimeSpan;
			
		} else {
			
			currentTs = lastStopTimeSpan;
			
		}
		
		if (timeText != null) {
			
			timeText.text = ConvertTimeSpanToString(currentTs);
			
		}
		
		if (timeCircleController != null) {
			
			timeCircleController.SetTime(currentTs, circleAnim);
			
		}
		
	}
	
	void SetVisibleResetButton(bool visible) {
		
		if (resetButtonText != null) {
			
			float alpha = visible ? 1.0f : 0.0f;
			resetButtonText.SetAlpha(alpha, buttonAnimDuration);
			
		}
		
		if (resetButtonAngleAnimation != null) {
			
			float fromAngle = visible ? 360.0f : 0.0f;
			float toAngle = visible ? 0.0f : -360.0f;
			resetButtonAngleAnimation.SetAngle(fromAngle, toAngle, true, buttonAnimDuration, false);
			
		}
		
	}
	
	void FlashBackground(int colorIndex) {
		
		if (bgColor != null) {
			
			bgColor.Flash(colorIndex);
			
		}
	}
	
	static public string ConvertTimeSpanToString(TimeSpan ts) {
		
		if (ts.Hours > 0 || ts.Days > 0) {
			return string.Format("{0}:{1:D2}:{2:D2}.{3}", ts.Days * 24 + ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds.ToString("000").Substring(0, 2));
		} else {
			return string.Format("{0}:{1:D2}.{2}", ts.Minutes, ts.Seconds, ts.Milliseconds.ToString("000").Substring(0, 2));
		}
	}
}
