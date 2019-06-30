Shader "Space Graphics Toolkit/Additive"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		[Toggle(SGT_A)] _PowerRGB("PowerRGB", Float) = 0
		[Toggle(SGT_B)] _Near("Near", Float) = 0
		_NearDistance("    Distance", Float) = 10000.0
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Transparent"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			//Blend SrcAlpha One
			Blend One OneMinusSrcColor
			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile __ SGT_A // Power RGB
			#pragma multi_compile __ SGT_B // Near

			sampler2D _MainTex;
			float     _NearDistance;

			struct a2v
			{
				float4 vertex    : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float4 color     : COLOR;
			};

			struct v2f
			{
				float4 vertex    : SV_POSITION;
				float2 texcoord0 : TEXCOORD0;
				float4 color     : COLOR;
				float3 direction : TEXCOORD1;
			};

			struct f2g
			{
				float4 color : SV_TARGET;
			};

			void Vert(a2v i, out v2f o)
			{
				float4 vertM = mul(unity_ObjectToWorld, i.vertex);

				o.vertex    = UnityObjectToClipPos(i.vertex);
				o.texcoord0 = i.texcoord0;
				o.color     = i.color;
				o.direction = vertM.xyz - _WorldSpaceCameraPos;
			}

			void Frag(v2f i, out f2g o)
			{
				float4 main = tex2D(_MainTex, i.texcoord0);
#if SGT_A // Power RGB
				main.rgb = pow(main.rgb, float3(1.0f, 1.0f, 1.0f) + (1.0f - i.color.rgb) * 10.0f);
#else
				main.rgb *= i.color.rgb;
#endif
#if SGT_B // Near
				i.color.a *= smoothstep(0.0f, 1.0f, length(i.direction) / _NearDistance);
#endif
				o.color = main * i.color.a;
			}
			ENDCG
		} // Pass
	} // SubShader
} // Shader