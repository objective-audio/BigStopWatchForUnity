using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateTexture : ScriptableWizard {
	
	public string textureName;
	public int width = 128;
	public int height = 128;
	public TextureFormat textureFormat = TextureFormat.ARGB32;
	
	[MenuItem ("Assets/Create/Texture...")]
	static void CreateWizard()
	{
       ScriptableWizard.DisplayWizard("Create Texture", typeof(CreateTexture));
	}
	
	void OnWizardCreate() {
		
		if (string.IsNullOrEmpty(textureName)) {
			textureName = "New Texture " + width + "x" + height;
		}
		
		string path = BSWEditorUtility.GetSelectedDirectoryPath() + "/" + textureName + ".asset";
		path = AssetDatabase.GenerateUniqueAssetPath(path);
		
		Texture2D tex = new Texture2D(width, height, textureFormat, false);
		tex.name = textureName;
		
		ClearTexture(tex);
		
		AssetDatabase.CreateAsset(tex, path);
	}
	
	static public void ClearTexture(Texture2D tex) {
		
		int texWidth = tex.width;
		int texHeight = tex.height;
		
		for (int x = 0; x < texWidth; x++) {
			for (int y = 0; y < texHeight; y++) {
				tex.SetPixel(x, y, Color.clear);
			}
		}
	}
}
