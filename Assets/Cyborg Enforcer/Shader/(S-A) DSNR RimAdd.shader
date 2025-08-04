Shader "Parabole/Reflective/(S-A) DSNR RimAdd" {
Properties {

		_Color ("Base Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Strength", Float) = 3.0
		_Shininess ("Glossiness", Float) = 0.078125
		_SpecColor ("Gloss Color", Color) = (0, 0, 0, 1)
		_SpecStrength ("Specular Strength", Float) = 1
	_MainTex ("Base (RGB) Specular (A)", 2D) = "white" {}

		_CubeMapColor ("Reflection Color", Color) = (1,1,1,1)
		_CubeMapRoughness ("Reflection Roughness", Float) = 0
		_CubeMapStrength ("Reflection Strength", Float) = 1
	_CubeMap ("Reflection", Cube) = "grey" { TexGen CubeReflect }

	_NormalMap ("Normal", 2D) = "bump" {}
}

SubShader { 
	Tags { "RenderType"="Opaque"}
	LOD 400
	
// You can add flags below (ex: #pragma surf BlinnPhong fullforwardshadows)
	// fullforwardshadows 	// This will five you full forward rendering shadows, including point and spot lights.
	// noambient 			// To ignore render settings ambient color
	
CGPROGRAM
#pragma surface surf BlinnPhong 
#pragma target 3.0

sampler2D _MainTex;
sampler2D _NormalMap;
samplerCUBE _CubeMap;

fixed3 _Color;
half3 _CubeMapColor; 
half3 _SpecularColor;
half3 _RimColor;

half _SpecStrength;
half _CubeMapStrength;
half _CubeMapRoughness;
half _Shininess;
half _RimPower;


struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	float3 viewDir;
	INTERNAL_DATA
		
};

void surf (Input IN, inout SurfaceOutput o) {

	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
	half rim = 1 - saturate(dot (normalize(IN.viewDir), o.Normal));
	o.Albedo =  tex.rgb * _Color + (_RimColor * pow(rim, _RimPower));
	
	o.Gloss = tex.a * _SpecStrength;
	
	o.Specular = _Shininess;
	
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 reflcol = texCUBEbias (_CubeMap, float4(worldRefl, _CubeMapRoughness));
	
	o.Emission =  o.Gloss * reflcol.rgb * _CubeMapColor * _CubeMapStrength;
}
ENDCG
}

FallBack "Specular"
}