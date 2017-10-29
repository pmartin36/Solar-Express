// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/EMPExplosionCenter"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_DetailTex("Detail Texture", 2D) = "white" {}
		_TopLayerPrimaryColor("Top Layer Primary Color", Color) = (1,1,1,1)
		_TopLayerSecondaryColor("Top Layer Secondary Color", Color) = (1,1,1,1)
		_BotLayerPrimaryColor("Bot Layer Primary Color", Color) = (1,1,1,1)
		_BotLayerSecondaryColor("Bot Layer Secondary Color", Color) = (1,1,1,1)
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
			float2 _MainTex_TexelSize;
			sampler2D _AlphaTex;
			sampler2D _DetailTex;
			float _AlphaModifier;

			/*sampler2D _TargetTex;
			float _TexTransition;*/

			float4 _TopLayerPrimaryColor;
			float4 _TopLayerSecondaryColor;
			float4 _BotLayerPrimaryColor;
			float4 _BotLayerSecondaryColor;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 main = tex2D(_MainTex, IN.texcoord);// *(1 - _TexTransition) + tex2D(_TargetTex, IN.texcoord) * _TexTransition;

				float4 topLayer = tex2D(_DetailTex, IN.texcoord + _Time.y / 5);
				float4 botLayer = tex2D(_DetailTex, IN.texcoord + float2(1.5,-1.5) * _Time.y / 5);

				float4 topcolor = lerp(_TopLayerPrimaryColor, _TopLayerSecondaryColor, topLayer.r);
				topcolor.a = topLayer.g * _AlphaModifier;
				float4 botcolor = lerp(_BotLayerPrimaryColor, _BotLayerSecondaryColor, botLayer.b);
				botcolor.a = saturate(botLayer.a-topLayer.a) * _AlphaModifier;
				//botcolor.a = botLayer.a;

				return (topcolor + botcolor) * main.a;
			}
		ENDCG
		}
	}
}
