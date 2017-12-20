// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/LevelFail"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_MinLightRadius("Min light Radius", Range(0,10)) = 0
		_MaxLightRadius("Max light Radius", Range(0,10)) = 0

		_TransitionTexture("Transition Texture", 2D) = "white" {}

		_Color("Tint", Color) = (1,1,1,1)
		_InnerColor("Inner Color", Color) = (1,1,1,1)
		_OuterColor("Outer Color", Color) = (1,1,1,1)
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
				float2 worldpos : TEXCOORD1;
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

				OUT.worldpos = mul(unity_ObjectToWorld, IN.vertex);

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _TransitionTexture;

			float _MinLightRadius;
			float _MaxLightRadius;

			fixed4 _InnerColor;
			fixed4 _OuterColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				float dist = distance(float2(0,0), IN.worldpos);
				fixed4 c = _OuterColor;
				if (dist < _MinLightRadius) {
					c = _InnerColor * IN.color;
				}
				else if (dist < _MaxLightRadius) {
					float pct = (dist - _MinLightRadius) / (_MaxLightRadius-_MinLightRadius);
					c = tex2D(_TransitionTexture, float2(pct, 0)) * IN.color;
				}
				c.rgb *= lerp(0.5, 1, (_MaxLightRadius - dist)/_MaxLightRadius);
				return c;
			}
		ENDCG
		}
	}
}
