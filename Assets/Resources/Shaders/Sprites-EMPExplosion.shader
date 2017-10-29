// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/EMPExplosion"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_SecondaryColor("Secondary Color", Color) = (1,1,1,1)
		_Center("Center Point", Vector) = (0,0,0,0)
		_Radius ( "Radius", Range(0,2) ) = 0
		_Ring("Ring", 2D) = "white" {}
		_CenterCircle("Center Circle Texture", 2D) = "white" {}
		_AlphaModifier("Alpha Modifier", float) = 1
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
				float2 worldpos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _SecondaryColor;

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
			sampler2D _Ring;
			sampler2D _CenterCircle;
			float2 _MainTex_TexelSize;
			float4 _Center;
			float _Radius;
			float _SmallRadius;
			float _AlphaModifier;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 main = SampleSpriteTexture(IN.texcoord);
				fixed4 center = tex2D(_CenterCircle, IN.texcoord);

				float radiussqr = pow(_Radius, 4);
				float distfromcenter = distance(_Center.xy, IN.worldpos);
				float distfromradius = distfromcenter - _Radius;
				float absdistfromradius = abs(distfromradius);

				if (absdistfromradius <= 0.1) {
					float4 ring = tex2D(_Ring, float2(absdistfromradius * 10, 0.5));
					ring.rgb = lerp(_Color, _SecondaryColor, ring.r).rgb;
					ring.a *= _AlphaModifier;
					return ring;
				}
				else if (distfromradius < 0.0) {
					main.rgb = float3(1,1,1) * center.a;
					
					distfromradius = distfromcenter - radiussqr;
					absdistfromradius = abs(distfromradius);

					if (absdistfromradius > 0.1) {
						main.rgb *= _Color.rgb;
						main.a = 0.5 * _AlphaModifier;
					}
					else {
						//main.rgb *= 2;
						main.a = 0.5 * _AlphaModifier;
					}
					return main;
				}
				return float4(0, 0, 0, 0);
			}
		ENDCG
		}
	}
}
