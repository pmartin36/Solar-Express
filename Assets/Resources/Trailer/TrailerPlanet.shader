Shader "Unlit/TrailerPlanet"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_NextTex("Next", 2D) = "white" {}
		_Frac("Next Frac", Range(0,1)) = 0

		_Color("Tint", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0,1)) = 0
		_Seed("Seed", Range(20,30)) = 20

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
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
		fixed4 color : COLOR;
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
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	sampler2D _NextTex;
	float _Frac;
	float _Cutoff;
	float _Seed;

	float random(float2 _st) {
		return frac(sin(dot(_st.xy,
			float2(12.9898, 78.233)))*
			43758.5453123);
	}

	float random(float x) {
		return frac(sin(x)*1.0);
	}

	float ripple(float d) {
		float x = d * _Seed;
		float i = floor(x);  // integer
		float f = frac(x);  // fraction
		return lerp(random(i), random(i + 1.0), smoothstep(0, 1, f));
	}

	fixed4 frag(v2f IN) : SV_Target
	{

	float dist = distance(float2(0.5, 0.5), IN.texcoord);

	fixed4 c = lerp(
				tex2D(_MainTex, IN.texcoord), 
				tex2D(_NextTex, IN.texcoord), 
				_Frac) * IN.color;

	fixed4 grayscale = dot(fixed3(.299, .587, .114), c.rgb);
	fixed3 diff = smoothstep(0,1,(_Cutoff - dist) * 5);

	c.rgb = lerp(grayscale.rgb, c.rgb, diff);

	

	c.rgb *= c.a;
	return c;
	}
		ENDCG
	}
	}
}