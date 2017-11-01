Shader "Sprites/EMPBomb"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaCutoffTex("Alpha Cutoff Tex", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_SecondaryColor("Secondary Color", Color) = (1,1,1,1)
		_Angle("Angle", Range(0,360)) = 0
		_Cutoff("Alpha Cutoff", Range(0,0.99)) = 0
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
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaCutoffTex;
			float4 _SecondaryColor;
			float _Angle;
			float _Cutoff;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				return color;
			}

			float VectorToAngle(float2 v)
			{
				v -= 0.5;
				v.x = sign(v.x) * max(0.000000000001, abs(v.x));
				v.y = sign(v.y) * max(0.000000000001, abs(v.y));
				float d = degrees(-atan2(v.y, v.x));
				d = fmod(d + 360, 360);
				return d;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord);
				fixed4 alphaTex = tex2D(_AlphaCutoffTex, IN.texcoord/2);


				float angle = VectorToAngle(IN.texcoord);
				c.a *= ceil(alphaTex.r - _Cutoff);

				if (angle < _Angle) {
					c *= _Color;
				}
				else {
					c *= _SecondaryColor;
				}

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
