Shader "Sprites/LaserShip"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_DetailTex("Detail Tex", 2D) = "black" {}
		
		_DetailColor("Detail Color", Color) = (1,1,1,1)

		_Cutoff("Cutoff Value", Range(0.01,0.99)) = 0

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
			sampler2D _AlphaTex;

			sampler2D _DetailTex;
			float _Cutoff;
			float4 _DetailColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				if (abs(IN.texcoord.y*2-1) > _Cutoff) {
					discard;
				}

				fixed4 d = tex2D(_DetailTex, IN.texcoord);
				fixed4 c = tex2D(_MainTex, IN.texcoord);
				if (d.a >= _Cutoff) {
					if (d.g == 0) {
						return c + (fixed4(1, 1, 1, d.a) * _DetailColor * d.r);
					}
					else {
						c.rgb *= _DetailColor.rgb * d.r;
					}				
				}
				
				return c;
			}
		ENDCG
		}
	}
}
