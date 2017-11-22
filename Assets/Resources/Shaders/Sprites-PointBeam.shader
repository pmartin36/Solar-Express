Shader "Sprites/PointBeam"
{
	Properties
	{
		[PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
		_DetailTex("Detail Tex", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,1)
		_CenterColor("Center Color", Color) = (1,1,1,1)
		_ExteriorColor("Exterior Color", Color) = (1,1,1,1)
		_Offset("Offset", Range(0,2)) = 0
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
			#pragma shader_feature ONEAXIS
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			float4 _Color;

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

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

			fixed4 _CenterColor;
			fixed4 _ExteriorColor;

			float _Offset;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;	

				float dist;
				dist = distance(IN.texcoord, float2(0.5, 0.5));

				c.rgb *= lerp(_CenterColor, _ExteriorColor, dist * 3 - _Offset);

				/*
				float of2 = (_Offset / 2) + 0.1;
				float absy = abs(IN.texcoord.y - 0.5);
				if (IN.texcoord.x < 0.5 && absy < of2) {
					c.rgb += lerp(float4(0.25,0.25,0.25,1), float4(0,0,0,1), saturate(absy*2.2 / of2)) * c.a;
				}
				*/

				c.rgb += float4(1, 1, 1, 1) * (tex2D(_DetailTex, IN.texcoord).a + _Offset) / 5;
				
				return c;
			}
		ENDCG
		}
	}
}
