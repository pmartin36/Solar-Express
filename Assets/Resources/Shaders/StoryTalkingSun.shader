Shader "Sprites/StoryTalkingSun"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_RainbowTexture("Rainbow", 2D) = "white" {}
		_CloudTexture("Cloud", 2D) = "white" {}
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
				OUT.uv = IN.texcoord;
				OUT.color = IN.color * _Color * 
						((sin(_Time.z) + 1) * 0.10 + 0.75); //osciallate alpha

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _RainbowTexture;
			sampler2D _CloudTexture;

			float VectorToAngle(float2 v)
			{
				v.x = sign(v.x) * max(0.000000000001, abs(v.x));
				v.y = sign(v.y) * max(0.000000000001, abs(v.y));
				float d = degrees(atan2(v.y, v.x));
				//d = fmod(d + 360, 360);
				return d;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 circle = tex2D(_MainTex, i.uv);		

				float dist = distance(float2(0.5, 0.5), i.uv) / .707;
				float angle = VectorToAngle(i.uv * 2 - 1);

				fixed4 clouds1 = tex2D(_CloudTexture, float2(angle/360, dist+2*_Time.x));
				angle = fmod(angle + 360 - dist*90 + clouds1.r * 30, 360);

				float x = (round(angle / 15) / 6);
				fixed4 r = tex2D(_RainbowTexture, float2(x+_Time.x*2, 0)) * i.color;

				fixed4 clouds = tex2D(_CloudTexture, float2(-dist + x*0.5 + _Time.x, x));
				r.a *= circle.a * pow(clouds.r, 3) * pow(1.3 - dist, 6);
							+ ((circle.a+0.15) * saturate(pow(clouds.r+0.2,1.7)-0.8));

				return r;
			}
		ENDCG
		}
	}
}
