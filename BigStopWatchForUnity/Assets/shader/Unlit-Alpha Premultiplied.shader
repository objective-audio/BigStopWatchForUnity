Shader "Unlit/Transparent Premultiplied Alpha" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		Blend One OneMinusSrcAlpha
	
		Pass {
			Lighting Off
	        
			SetTexture [_MainTex] { combine texture } 
		}
	}
}
