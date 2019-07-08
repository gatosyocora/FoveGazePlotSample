Shader "Unlit/ColorChange"
{
	Properties
	{
		_Color ("Color", Color) = (1,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Overlay+2"}
		LOD 100

		ZTest Always
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;

			float3 hsv2rgb(float3 hsv)
			{
				float h = hsv.x;
				float s = hsv.y;
				float v = hsv.z;

				float4 color = 1;

				h *= 6.0;
				float j = floor(h);
				float f = h - j;
				float aa = v * (1 - s);
				float bb = v * (1 - (s*f));
				float cc = v * (1 - (s*(1 - f)));
				if (j < 1)
				{
					color.r = v;
					color.g = cc;
					color.b = aa;
				}
				else if (j < 2)
				{
					color.r = bb;
					color.g = v;
					color.b = aa;
				}
				else if (j < 3)
				{
					color.r = aa;
					color.g = v;
					color.b = cc;
				}
				else if (j < 4)
				{
					color.r = aa;
					color.g = bb;
					color.b = v;
				}
				else if (j < 5)
				{
					color.r = cc;
					color.g = aa;
					color.b = v;
				}
				else
				{
					color.r = v;
					color.g = aa;
					color.b = bb;
				}

				return color;
			}


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(hsv2rgb(float3(1-i.uv.y, 1, 1)), 1);
			}
			ENDCG
		}
	}
}
