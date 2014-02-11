using UnityEngine;
using System.Collections;

public enum ButtonType {
	Background,
	Reset
}

public class Button : MonoBehaviour {
	
	public ButtonType type = ButtonType.Reset;
	
}
