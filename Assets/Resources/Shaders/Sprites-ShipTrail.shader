Shader "Sprites/ShipTrail"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_RainboxTexture("Rainbow", 2D) = "white" {}
		_CloudTex("Cloud", 2D) = "white" {}
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

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _RainboxTexture;
			sampler2D _CloudTex;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f i) : SV_Target
			{
				float lut_y = i.uv.y + 0.1 * (1-i.uv.x) * sin((_Time.y + i.uv.x)*5);
				fixed4 c = tex2D(_MainTex, float2(i.uv.x, lut_y));

				//the image has the highest red value of 156 at the fattest point (with is 156 width), so we divide by the color to get the scale
				float yneg = i.uv.y * 2 - 1; //-1 to 1
				yneg /= c.r;
				float y = (yneg + 1) / 2;

				fixed4 r = tex2D(_RainboxTexture, float2(frac(y+_Time.x*3), 0)) * i.color;
				
				float ycloud = (i.uv.y + _Time.x/10);
				fixed4 clouds = tex2D(_CloudTex, float2(i.uv.x + _Time.x*2, ycloud));

				r.a *= c.a * max(0.7, pow(clouds.r + 0.6, 4)) * pow(1.1 - abs(yneg),1.4);

				return r;
			}
		ENDCG
		}
	}
}
