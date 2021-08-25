Shader "Custom/BzKovSoft/FadeToDiffTex"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SubstituteTex ("Substitute Texture (RGB)", 2D) = "white" {}
		_SubstituteBumpMap ("Substitute Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Rate ("Rate", Range(0,1)) = 0
        _NoiseScale ("Noise Scale", Range(0,3)) = 0
        _NoiseFrequency ("Noise Frequency", Range(0,10)) = 3
        _noiseOffset ("Noise Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        #pragma target 3.0

		#include "noiseSimplex.cginc"

        sampler2D _MainTex;
        sampler2D _SubstituteTex;
		sampler2D _SubstituteBumpMap;
        half _Rate;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _NoiseScale;
		float _NoiseFrequency;
		float3 _noiseOffset;


		struct apdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
		};

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_SubstituteBumpMap;
        };

		void vert(inout apdata v)
		{
			float3 bitangent = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3 v0 = v.vertex.xyz;
			float3 v1 = v0 + (v.tangent.xyz * 0.01) * v.tangent.w;
			float3 v2 = v0 + (bitangent * 0.01);

			float xOffset = _Time.y;

			float ns0 = snoise(float3(v0.x + _noiseOffset.x + xOffset, v0.y + _noiseOffset.y, v0.z + _noiseOffset.z) * _NoiseFrequency);
			v0.xyz += _NoiseScale * ns0 * v.normal;

			float ns1 = snoise(float3(v1.x + _noiseOffset.x + xOffset, v1.y + _noiseOffset.y, v1.z + _noiseOffset.z) * _NoiseFrequency);
			v1.xyz += _NoiseScale * ns1 * v.normal;

			float ns2 = snoise(float3(v2.x + _noiseOffset.x + xOffset, v2.y + _noiseOffset.y, v2.z + _noiseOffset.z) * _NoiseFrequency);
			v2.xyz += _NoiseScale * ns2 * v.normal;

			float3 vn = cross(v1-v0, v2-v0);

			v.normal = normalize(vn);
			v.vertex.xyz = v0;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c1 = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 c2 = tex2D (_SubstituteTex, IN.uv_MainTex) * _Color;
			fixed4 c = lerp(c1, c2, _Rate);

			fixed3 n = UnpackNormal (tex2D (_SubstituteBumpMap, IN.uv_SubstituteBumpMap));
			n = lerp(o.Normal, n, _Rate);

            o.Albedo = c;
            o.Metallic = _Metallic;
			o.Normal = n;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
