// Simplified Bumped Specular shader supporting 2 textures. Differences from regular Bumped Specular one:
// - no Main Color nor Specular Color
// - specular lighting directions are approximated per vertex
// - writes zero to alpha channel
// - Normalmap uses Tiling/Offset of the Base texture
// - no Deferred Lighting support
// - no Lightmap support
// - supports ONLY 1 directional light. Other lights are completely ignored.

Shader "Mobile/Bumped Specular (1 Directional Light) 2 Tex" {
Properties {
	_Shininess ("Shininess", Range (0.03, 10)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Crossfade("Crossfade", Range(0, 1)) = 0.0
	_MainTex2("Base (RGB) 2", 2D) = "white" {}
	_BumpMap2("Normalmap 2", 2D) = "bump" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 250
	
CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview novertexlights

inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir));
	fixed nh = max (0, dot (s.Normal, halfDir));
	fixed spec = pow (nh, s.Specular*128) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
}

sampler2D _MainTex;
sampler2D _BumpMap;
half _Shininess;
half _Crossfade;
sampler2D _MainTex2;
sampler2D _BumpMap2;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_MainTex2, IN.uv_MainTex), _Crossfade);
	o.Albedo = c.rgb;
	o.Gloss = c.a;
	o.Alpha = c.a;
	o.Specular = _Shininess;
	o.Normal = lerp(UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)), UnpackNormal(tex2D(_BumpMap2, IN.uv_MainTex)), _Crossfade);
}
ENDCG
}

FallBack "Mobile/VertexLit"
}
