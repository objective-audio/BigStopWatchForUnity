using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateEmptyMesh : MonoBehaviour {

	[MenuItem ("Assets/Create/Empty Mesh")]
	static void Create() {
		
		string path = BSWEditorUtility.GetSelectedDirectoryPath() + "/New Empty Mesh.asset";
		path = AssetDatabase.GenerateUniqueAssetPath(path);
		
		Mesh mesh = new Mesh();
		
		AssetDatabase.CreateAsset(mesh, path);
	}
}
