Shader "Sprites/Core"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_MaxLightRadius("Max light Radius", Range(0,1)) = 0

		_ColorMap("Color Map",2D) = "white" {}
		_Rotation("Rotation", Range(0,6.28)) = 0
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
				float2 color_uv : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			float _Rotation;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				OUT.uv = IN.texcoord;

				//pivot point
				float2 pivot = float2(0.5, 0.5);

				// Rotation Matrix
				float cosAngle = cos(_Rotation);
				float sinAngle = sin(_Rotation);
				float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);

				// Rotation consedering pivot
				float2 uv = IN.texcoord.xy - pivot;
				OUT.color_uv = mul(rot, uv);
				OUT.color_uv += pivot;

				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _ColorMap;

			float _MaxLightRadius;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				//color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.uv) * IN.color;
				c.rgb *= c.a;

				float dist = distance(float2(0.5,0.5), IN.uv);
				float intensity = pow(1 - abs(dist - _MaxLightRadius), 8);
			
				float regions = tex2D(_ColorMap, IN.uv).r;
				float3 highlight = intensity *
					((regions.r * normalize(float3(IN.color_uv, dist * 2))) +((1 - regions.r) * float3(dist, 0, 0)));

				c.a *= regions;
				c.rgb += highlight;

				return c;
			}
		ENDCG
		}
	}
}
