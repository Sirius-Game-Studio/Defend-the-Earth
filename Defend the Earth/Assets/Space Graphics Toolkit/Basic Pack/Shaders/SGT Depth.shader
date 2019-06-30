Shader "Space Graphics Toolkit/Depth"
{
	Properties
	{
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Transparent+1"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite On
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				struct a2v
				{
					float4 vertex : POSITION;
				};
				
				struct v2f
				{
					float4 vertex : SV_POSITION;
				};
				
				struct f2g
				{
					float4 color : SV_TARGET;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex = UnityObjectToClipPos(i.vertex);
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.color = float4(0.0f, 0.0f, 0.0f, 0.0f);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader