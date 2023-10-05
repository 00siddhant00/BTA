Shader "Unlit/Fill"
{
	Properties
	{
		_Health("Health", Range(0, 1)) = 1
		_Direction("Direction", Range(0, 1)) = 1
		_Axis("Axis", Range(0, 1)) = 1
		_flashSpeed("Flash Speed", Range(0, 10)) = 4
		_flashFadeAmount("Flash Fade Amount", Range(0, 0.9)) = .5
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			struct Meshdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolator
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Health;
			float _flashSpeed;
			float _flashFadeAmount;
			float _Direction;
			float _Axis;

			Interpolator vert(Meshdata v)
			{
				Interpolator o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float InverseLerp(float a, float b, float v)
			{
				return (v - a) / (b - a);
			}

			#define Clamp01 saturate

			float4 frag(Interpolator i) : SV_Target
			{
				float barMask;

				if (_Direction > 0.5)
				{
					if (_Axis > 0.5) barMask = _Health > i.uv.x;
					else barMask = _Health < i.uv.x;
				}
				else
				{
					if (_Axis > 0.5) barMask = _Health > i.uv.y;
					else barMask = _Health < i.uv.y;
				}

				clip(barMask - 0.5);

				float3 outColor = lerp(float3 (0, 0, 0), float3 (1, 1, 1), barMask);

				if (_Health < 0.25)
				{
					float flash = cos(_Time.y * _flashSpeed) * _flashFadeAmount + 1;
					outColor *= flash;
				}

				return float4 (outColor, 1);
			}
			ENDCG
		}
	}
}
