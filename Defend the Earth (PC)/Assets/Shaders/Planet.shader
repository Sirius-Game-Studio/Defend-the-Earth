// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Planet/Planet"
{
	Properties
	{
		_MainTex("Diffuse Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_SpecMap("Specular Map", 2D) = "white" {}
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(1, 50)) = 20
		_LightMap("Night lights Map", 2D) = "black" {}
		_LightMapIntensity("Night lights Intensity", Float) = 1.0
		_CloudMap("Cloud Map", 2D) = "black" {}
		_CloudSpeed("Cloud speed", Float) = 0.01
		_CloudStrength("Cloud strength", Range(0, 5)) = 1.0
		_CloudShadowsStrength("Cloud shadows strength", Range(0, 1)) = 0.0
		_Color1("Color 1", Color) = (0, 255, 31, 0)
		_Color2("Color 2", Color) = (226, 183, 25, 0)
		_AirGlowStrength("Air Glow strength", Range(0, 2)) = 0.2
	}
		SubShader
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}

			Pass
			{
				Cull Back
				ZWrite On

				CGPROGRAM
				#include "UnityCG.cginc"

				#include "PlanetShared.cginc"
				#pragma vertex vert  
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile MOBILE_ON MOBILE_OFF
				#pragma multi_compile ATMO_ON ATMO_OFF

				uniform float4 _LightColor0;

				struct vertexInput
				{
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
				};

				struct vertexOutput {
					float4	pos : SV_POSITION;
					float4	posWorld : TEXCOORD0;
					// position of the vertex (and fragment) in world space 
					float4	tex : TEXCOORD1;
					float3	tangentWorld : TEXCOORD2;
					float3	normalWorld : TEXCOORD3;
					float3	binormalWorld : TEXCOORD4;
					float3	c0 : COLOR0;
					float3	c1 : COLOR1;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					float4x4 modelMatrix = unity_ObjectToWorld;
					float4x4 modelMatrixInverse = unity_WorldToObject;

					output.tangentWorld = normalize(
						mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
					output.normalWorld = normalize(
						mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
					output.binormalWorld = normalize(
						cross(output.normalWorld, output.tangentWorld)
						* input.tangent.w); // tangent.w is specific to Unity

					output.posWorld = mul(modelMatrix, input.vertex);
					output.tex = input.texcoord;
					output.pos = UnityObjectToClipPos(input.vertex);

					// ATMOSPHERE

					float3 v3CameraPos = _WorldSpaceCameraPos - v3Translate;
					float fCameraHeight = length(v3CameraPos);
					float fCameraHeight2 = fCameraHeight*fCameraHeight;

					float3 v3Pos = mul(unity_ObjectToWorld, input.vertex).xyz - v3Translate;
					float3 v3Ray = v3Pos - v3CameraPos;
					float fFar = length(v3Ray);
					v3Ray /= fFar;

					float B = 2.0 * dot(v3CameraPos, v3Ray);
					float C = fCameraHeight2 - fOuterRadius * fOuterRadius;
					float fDet = max(0.0, B*B - 4.0 * C);
					float fNear = 0.5 * (-B - sqrt(fDet));

					float3 v3Start = v3CameraPos + v3Ray * fNear;
					fFar -= fNear;
					float fDepth = exp((fInnerRadius - fOuterRadius) / fScaleDepth);
					float fCameraAngle = dot(-v3Ray, v3Pos) / length(v3Pos);
					float fLightAngle = dot(_WorldSpaceLightPos0, v3Pos) / length(v3Pos);
					float fCameraScale = scale(fCameraAngle);
					float fLightScale = scale(fLightAngle);
					float fCameraOffset = fDepth*fCameraScale;
					float fTemp = (fLightScale + fCameraScale);

					const float fSamples = 2.0;

					float fSampleLength = fFar / fSamples;
					float fScaledLength = fSampleLength * fScale;
					float3 v3SampleRay = v3Ray * fSampleLength;
					float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;

					float3 v3FrontColor = float3(0.0, 0.0, 0.0);
					float3 v3Attenuate;
					for (int i = 0; i<int(fSamples); i++)
					{
						float fHeight = length(v3SamplePoint);
						float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));
						float fScatter = fDepth*fTemp - fCameraOffset;
						v3Attenuate = exp(-fScatter * (v3InvWavelength * fKr4PI + fKm4PI));
						v3FrontColor += v3Attenuate * (fDepth * fScaledLength);
						v3SamplePoint += v3SampleRay;
					}
					output.c0 = v3FrontColor * (v3InvWavelength * fKrESun + fKmESun);
					output.c1 = v3Attenuate;

					return output;
				}

				// fragment shader for pass 2 without ambient lighting 
				float4 frag(vertexOutput input) : COLOR
				{
					float w = 0;

				// SHADOW HANDLING
				#if MOBILE_OFF

				if (shadowNumber > 0)
				{
					w = max(w, getShadow(input.posWorld, planetShadowPos0, planetShadowSca0));
					if (shadowNumber > 1)
					{
						w = max(w, getShadow(input.posWorld, planetShadowPos1, planetShadowSca1));
						if (shadowNumber > 2)
						{
							w = max(w, getShadow(input.posWorld, planetShadowPos2, planetShadowSca2));
							if (shadowNumber > 3)
							{
								w = max(w, getShadow(input.posWorld, planetShadowPos3, planetShadowSca3));
								if (shadowNumber > 4)
								{
									w = max(w, getShadow(input.posWorld, planetShadowPos4, planetShadowSca4));
									if (shadowNumber > 5)
									{
										w = max(w, getShadow(input.posWorld, planetShadowPos5, planetShadowSca5));
										if (shadowNumber > 6)
										{
											w = max(w, getShadow(input.posWorld, planetShadowPos6, planetShadowSca6));
											if (shadowNumber > 7)
											{
												w = max(w, getShadow(input.posWorld, planetShadowPos7, planetShadowSca7));
												if (shadowNumber > 8)
												{
													w = max(w, getShadow(input.posWorld, planetShadowPos8, planetShadowSca8));
													if (shadowNumber > 9)
													{
														w = max(w, getShadow(input.posWorld, planetShadowPos9, planetShadowSca9));
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				_LightColor0.rgb *= (1 - w);

				#endif
				// in principle we have to normalize tangentWorld,
				// binormalWorld, and normalWorld again; however, the  
				// potential problems are small since we use this 
				// matrix only to compute "normalDirection", 
				// which we normalize anyways

				float4 encodedNormal;
				float3 localCoords;

				#if MOBILE_OFF

				encodedNormal = tex2D(_BumpMap,
				_BumpMap_ST.xy *  input.tex.xy + _BumpMap_ST.zw);
				localCoords = float3(2.0 * encodedNormal.a - 1.0,
				2.0 * encodedNormal.g - 1.0, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));

				// approximation without sqrt:  localCoords.z = 
				// 1.0 - 0.5 * dot(localCoords, localCoords);

				#endif

				#if MOBILE_ON

				// use this if on mobile        
				encodedNormal = tex2D(_BumpMap,
				_BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw);
				localCoords = ((2.0 * encodedNormal.rgb) - float3(1.0, 1.0, 1.0));

				#endif


				float3x3 local2WorldTranspose = float3x3(
					input.tangentWorld,
					input.binormalWorld,
					input.normalWorld);
				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;

				lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				float3 diff = _LightColor0.rgb * max(0, dot(lightDirection, normalize(normalDirection + (0.2 * lightDirection))));
				float3 spec;

				spec = _SpecColor.rgb * pow(max(0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);

				// diffuse without normal for cloud (cloud hide the relief of the planet)
				float3 diffWn = _LightColor0.rgb * max(0, dot(lightDirection, normalize(input.normalWorld + (0.2 * lightDirection))));

				// DIFFUSE + NORMAL

				float3 c = diff * pow(tex2D(_MainTex, input.tex.xy), _Gamma);
				// SPECULAR
				c += spec * diff * pow(tex2D(_SpecMap, input.tex.xy).r, _Gamma);
				// CLOUD
				float2 uv = input.tex.xy;
				uv.x += _Time * _CloudSpeed;

				fixed3 cloud = -pow(tex2D(_CloudMap, uv), _Gamma) * _CloudShadowsStrength * _CloudStrength; // add cloud shadows
				uv.x += 0.001;
				cloud += pow(tex2D(_CloudMap, uv), _Gamma) * (1 + _CloudShadowsStrength) * _CloudStrength; // add cloud
				cloud *= diffWn;
				#if ATMO_ON
				cloud *= input.c1;
				#endif
				c += cloud;
				//ATMOSPHERE
				#if ATMO_ON
				c += input.c0 * _LightColor0.rgb;
				c *= Luminance(input.c1);
				#endif
				// NIGHT LIGHT
				c += saturate(0.2 - diff) * _LightMapIntensity * pow(tex2D(_LightMap, input.tex.xy), _Gamma);
				// EXPOSURE
				c = 1.0 - exp(c * -fHdrExposure);
				// GAMMA CORRECTION
				c = pow(c, 1 / _Gamma);
				return float4(c, 1.0);
			}
		ENDCG
		}
		Pass
		{

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Front

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "PlanetShared.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile ATMO_ON ATMO_OFF
			#pragma multi_compile MOBILE_ON MOBILE_OFF
			#pragma target 3.0

			float4 _LightColor0;

			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float3 	t0 : TEXCOORD1;
				float3 	c0 : COLOR0;
				float3 	c1 : COLOR1;
				float3 	posWorld : COLOR2;
				float3  c2 : COLOR3;
				float3  c3 : COLOR4;
			};

			v2f vert(appdata_base v)
			{
				v2f OUT;

				OUT.c0 = 0;
				OUT.c1 = 0;
				OUT.c2 = 0;
				OUT.c3 = 0;
				OUT.t0 = 0;

				#if ATMO_ON
				v.vertex.xyz += v.normal * 0.015;
				float3 v3CameraPos = _WorldSpaceCameraPos - v3Translate;
				float fCameraHeight = length(v3CameraPos);
				float fCameraHeight2 = fCameraHeight*fCameraHeight;

				float3 v3Pos = mul(unity_ObjectToWorld, v.vertex).xyz - v3Translate;
				float3 v3Ray = v3Pos - v3CameraPos;
				float fFar = length(v3Ray);
				v3Ray /= fFar;

				float B = 2.0 * dot(v3CameraPos, v3Ray);
				float C = fCameraHeight2 - fOuterRadius * fOuterRadius;
				float fDet = max(0.0, B*B - 4.0 * C);
				float fNear = 0.5 * (-B - sqrt(fDet));

				float3 v3Start = v3CameraPos + v3Ray * fNear;
				fFar -= fNear;
				float fStartAngle = dot(v3Ray, v3Start) / fOuterRadius;
				float fStartDepth = exp(-1.0 / fScaleDepth);
				float fStartOffset = fStartDepth*scale(fStartAngle);

				const float fSamples = 2.0;

				float fSampleLength = fFar / fSamples;
				float fScaledLength = fSampleLength * fScale;
				float3 v3SampleRay = v3Ray * fSampleLength;
				float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;

				float3 v3FrontColor = float3(0, 0, 0);
				float v3BackColor = float3(0, 0, 0);
				float fDepth = 1;
				for (int i = 0; i<int(fSamples); i++)
				{
					float fHeight = length(v3SamplePoint);
					fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));
					float fLightAngle = dot(_WorldSpaceLightPos0, v3SamplePoint) / fHeight;
					float fLightAngle2 = -dot(_WorldSpaceLightPos0, v3SamplePoint) / fHeight;
					float fCameraAngle = dot(v3Ray, v3SamplePoint) / fHeight;
					float fScatter = (fStartOffset + fDepth*(scale(fLightAngle) - scale(fCameraAngle)));
					float fScatter2 = (fStartOffset + fDepth*(scale(fLightAngle2) - scale(fCameraAngle)));
					float3 v3Attenuate = exp(-fScatter * (v3InvWavelength * fKr4PI + fKm4PI));
					float3 v3Attenuate2 = exp(-fScatter2 * (v3InvWavelength * fKr4PI + fKm4PI));
					v3FrontColor += v3Attenuate * (fDepth * fScaledLength);// +(fDepth * fScaledLength * _AirGlowStrength * 0.1);
					v3BackColor += v3Attenuate2 * ((1 / (fDepth * 1000.)) * fScaledLength);
					v3SamplePoint += v3SampleRay;
				}
				OUT.c0 = v3FrontColor * (v3InvWavelength * fKrESun);
				OUT.c2 = saturate(2. * v3BackColor - (fDepth * 20.));
				OUT.c3 = saturate(v3BackColor * 2.);
				OUT.c1 = v3FrontColor * fKmESun;
				OUT.t0 = v3CameraPos - v3Pos;
				#endif

				OUT.pos = UnityObjectToClipPos(v.vertex);
				OUT.uv = v.texcoord.xy;
				OUT.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;

				return OUT;
			}

			half4 frag(v2f input) : COLOR
			{
				float3	col = 0;
				float	a = 0;
				float	w = 0;

				#if MOBILE_OFF

				// SHADOW HANDLING
				if (shadowNumber > 0)
				{
					w = max(w, getShadow(input.posWorld, planetShadowPos0, planetShadowSca0));
					if (shadowNumber > 1)
					{
						w = max(w, getShadow(input.posWorld, planetShadowPos1, planetShadowSca1));
						if (shadowNumber > 2)
						{
							w = max(w, getShadow(input.posWorld, planetShadowPos2, planetShadowSca2));
							if (shadowNumber > 3)
							{
								w = max(w, getShadow(input.posWorld, planetShadowPos3, planetShadowSca3));
								if (shadowNumber > 4)
								{
									w = max(w, getShadow(input.posWorld, planetShadowPos4, planetShadowSca4));
									if (shadowNumber > 5)
									{
										w = max(w, getShadow(input.posWorld, planetShadowPos5, planetShadowSca5));
										if (shadowNumber > 6)
										{
											w = max(w, getShadow(input.posWorld, planetShadowPos6, planetShadowSca6));
											if (shadowNumber > 7)
											{
												w = max(w, getShadow(input.posWorld, planetShadowPos7, planetShadowSca7));
												if (shadowNumber > 8)
												{
													w = max(w, getShadow(input.posWorld, planetShadowPos8, planetShadowSca8));
													if (shadowNumber > 9)
													{
														w = max(w, getShadow(input.posWorld, planetShadowPos9, planetShadowSca9));
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}

				_LightColor0.rgb *= (1 - w);

				#endif

				#if ATMO_ON

				float fCos = dot(_WorldSpaceLightPos0, input.t0) / length(input.t0);
				float fCos2 = fCos*fCos;
				col = getRayleighPhase(fCos2) * input.c0 + getMiePhase(fCos, fCos2) * input.c1;
				col += input.c2 * _Color1 *_AirGlowStrength;
				col += input.c3 * _Color2 * _AirGlowStrength * 0.5;
				col *= _LightColor0.rgb;
				col = 1 - exp(col * -fHdrExposure);
				col = pow(col, 1 / _Gamma);
				a = Luminance(col);
				#endif

				return float4(col.rgb, a);
			}
		ENDCG
		}
		}
}