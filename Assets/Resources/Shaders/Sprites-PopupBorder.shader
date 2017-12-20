Shader "Sprites/PopupBorder"
{
	Properties
	{
		_BorderTex("Border Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float VectorToAngle(float2 v)
			{
				v.x = sign(v.x) * max(0.000000000001, abs(v.x));
				v.y = sign(v.y) * max(0.000000000001, abs(v.y));
				float d = degrees(atan2(v.y, v.x));
				d = fmod(d + 360, 360);
				return d;
			}

			sampler2D _BorderTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uvn = i.uv * 2 - 1;
				float uva = saturate(VectorToAngle(uvn) / 360.0) + _Time.x;

				float4 c = tex2D(_BorderTex, float2(uva, 1));
				c.rgb *= 0.75;

				return c;
			}
		ENDCG
		}
	}
}
