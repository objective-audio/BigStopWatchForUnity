using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class BSWEditorUtility : MonoBehaviour {

	static public string GetSelectedDirectoryPath() {
		
		string path = null;
		
		foreach (UnityEngine.Object obj in Selection.objects) {
			
			path = AssetDatabase.GetAssetPath(obj);
			
			if (!string.IsNullOrEmpty(path) && !Directory.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}
			
			break;
		}
		
		if (string.IsNullOrEmpty(path)) {
			return "Assets";
		}
		
		return path;
	}
}
