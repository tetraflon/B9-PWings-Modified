Shader "KSP/Specular Layered"
{
	Properties
	{
		_MainTex("Diffuse (4x grayscale in RGBA)", 2D) = "white" {}
		_Emissive("Color mask (4x grayscale in RGBA)", 2D) = "white" {}

		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125

		_Opacity("_Opacity", Range(0,1)) = 1
		_RimFalloff("_RimFalloff", Range(0.01,5)) = 0.1
		_RimColor("_RimColor", Color) = (0,0,0,0)

		_TemperatureColor("_TemperatureColor", Color) = (0,0,0,0)
		_BurnColor("Burn Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}

		ZWrite On
		ZTest LEqual

		CGPROGRAM

		#pragma surface surf NormalizedBlinnPhong
		#pragma target 3.0

		#include "LegacyToStandard.cginc"
		#include "Lighting.cginc"

		inline void LightingNormalizedBlinnPhong_GI (
			SurfaceOutput s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination (data, 1.0, s.Normal);
		}
		
		// Unity5 firefly fix
		// U5 lighting function doesn't normalize properly, so we have to write our own...
		// borrowed bits of Unity BlinnPhong functions
		inline fixed4 LightingNormalizedBlinnPhong (SurfaceOutput s, half3 viewDir, UnityGI gi)
		{
			 UnityLight light = gi.light;

			fixed3 normalizedSurfNormal = normalize(s.Normal);
			fixed3 halfDir = normalize(light.dir + viewDir);

			fixed diff = max(0, dot(normalizedSurfNormal, light.dir));

			fixed nh = max(0, dot(normalizedSurfNormal, halfDir));
			fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

			fixed4 c;
			c.rgb = (_LightColor0.rgb * ((s.Albedo * diff) + (spec *_SpecColor.rgb)));
			c.rgb += s.Albedo * gi.indirect.diffuse;
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec;

			return c;
		}

		inline half4 LightingNormalizedBlinnPhong_Deferred (SurfaceOutput s, float3 viewDir, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
		{
			UnityStandardData data;
			data.occlusion      = 1.0;
			data.normalWorld    = s.Normal;

			GetStandardSpecularPropertiesFromLegacy(s.Gloss, 0.45, s.Specular, data.smoothness, data.specularColor);

			half oneMinusReflectivity;
			data.diffuseColor = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, data.specularColor, oneMinusReflectivity);

			UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

			half4 emission = half4(s.Emission, 1);
			return emission;
		}
		

		half _Shininess;
		sampler2D _MainTex;
		sampler2D _Emissive;

		float _Opacity;
		float _RimFalloff;
		float4 _RimColor;

		float4 _TemperatureColor;
		float4 _BurnColor;

		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;
			float2 uv2_Emissive : TEXCOORD1;
			float3 viewDir;
			float4 color : COLOR;
		};

		// Vertex colors
		// R(x) - partial fill color R
		// G(y) - partial fill color G
		// B(z) - partial fill color B
		// A(w) - fill factor
		// U    - layer factor
		// V    - masked fill/uniform fill factor
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 color = tex2D(_MainTex, IN.uv_MainTex);
			float4 mask = tex2D(_Emissive, IN.uv_MainTex);
			float3 normal = float3 (0, 0, 1);

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), normal));
			float3 emission = (_RimColor.rgb * pow(rim, _RimFalloff)) * _RimColor.a;
			emission += _TemperatureColor.rgb * _TemperatureColor.a;

			
			float2 albedoGrayscaleAndMask =
				lerp
				(
					lerp
					(
						float2 (color.x, mask.x),
						float2 (color.y, mask.y),
						saturate(IN.uv2_Emissive.x * 3)
					),
					lerp
					(
						float2 (color.z, mask.z),
						float2 (color.w, mask.w),
						saturate(IN.uv2_Emissive.x * 3 - 2)
					),
					saturate(IN.uv2_Emissive.x * 3 - 1)
				);
			
			// composite:
			//  uv2_Emissive.y == 1 -> uniform/coat fill: pure vertex color
			//  uv2_Emissive.y == 0 -> material layer: grayscale pattern kept, tinted by vertex color
			//  with a colour floor so every layer (incl. ones with a dark mask channel) shows the tint:
			//   mColor = saturate(mask + 0.25)  (higher floor = more colour, less raw pattern)
			float3 composite =
				lerp
				(
					albedoGrayscaleAndMask.xxx,
					lerp
					(
						lerp
						(
							saturate(albedoGrayscaleAndMask.x + ((1 - albedoGrayscaleAndMask.y) * pow(albedoGrayscaleAndMask.x, 2))).xxx,
							IN.color.xyz,
							saturate(albedoGrayscaleAndMask.y + 0.25)
						),
						IN.color.xyz,
						IN.uv2_Emissive.y
					),
					saturate(IN.color.w)
				);

			o.Albedo = composite * _BurnColor;
			o.Emission = emission;

			o.Gloss = (pow(1 - albedoGrayscaleAndMask.y + 0.4, 3) + 0.2) * albedoGrayscaleAndMask.y;
			// Gloss output is weird - it's a workaround to extracting a decent-looking specular map out of colormask and some constants, required for the wing to look right next to stock parts

			o.Specular = _Shininess;
			o.Normal = normal;
			o.Emission *= _Opacity;
			o.Alpha = _Opacity;

#if UNITY_PASS_DEFERRED
			// In deferred rendering do not use the flat ambient because Deferred adds its own ambient as a composite of flat ambient and probe
			unity_SHAr = 0.0.xxxx;
			unity_SHAg = 0.0.xxxx;
			unity_SHAb = 0.0.xxxx;
#endif
		}
	ENDCG
	}
	Fallback "Standard"
}