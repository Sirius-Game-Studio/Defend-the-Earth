#ifndef PLANETSHARED_CGINC
# define PLANETSHARED_CGINC

uniform sampler2D _MainTex;
uniform sampler2D _MainTexRing;
uniform sampler2D _BumpMap;
uniform sampler2D _SpecMap;
uniform sampler2D _LightMap;
uniform sampler2D _CloudMap;
uniform float4 _BumpMap_ST;
uniform float4 _SpecColor;
uniform float _Shininess;
uniform float _LightMapIntensity;
uniform float	_CloudStrength;
uniform float	_CloudShadowsStrength;
uniform float	_CloudSpeed;
uniform float4	_Color1;
uniform float4	_Color2;
uniform float	_AirGlowStrength;

// ATMOSPHERE VARIABLE

float	_Gamma;
float	atmoSize;
int		_atmosphere;
float3	v3Translate;
float3 	v3InvWavelength;
float 	fOuterRadius;
float 	fInnerRadius;
float 	fKrESun;
float 	fKmESun;
float	fKr4PI;
float 	fKm4PI;
float 	fScale;
float 	fScaleDepth;
float 	fScaleOverScaleDepth;
float 	fHdrExposure;

// SHADOW VARIABLE

#define LIGHT_DISTANCE 0.0025
// change this for sharper/smoother shadows

float	shadowNumber;

float3	planetShadowPos0;
float3	planetShadowPos1;
float3	planetShadowPos2;
float3	planetShadowPos3;
float3	planetShadowPos4;
float3	planetShadowPos5;
float3	planetShadowPos6;
float3	planetShadowPos7;
float3	planetShadowPos8;
float3	planetShadowPos9;

float	planetShadowSca0;
float	planetShadowSca1;
float	planetShadowSca2;
float	planetShadowSca3;
float	planetShadowSca4;
float	planetShadowSca5;
float	planetShadowSca6;
float	planetShadowSca7;
float	planetShadowSca8;
float	planetShadowSca9;

float 	scale(float fCos)
{
	float x = 1.0 - fCos;
	return fScaleDepth * exp(-0.00287 + x * (0.459 + x * (3.83 + x * (-6.80 + x * 5.25))));
}

// Calculates the Mie phase function
float 	getMiePhase(float fCos, float fCos2)
{
	return 0.04 * (1.0 + fCos2) / pow(2 + 2 * fCos, 1.5);
}

// Calculates the Rayleigh phase function
float 	getRayleighPhase(float fCos2)
{
	return (fCos2 + 0.75) * 0.75;
}

float2 	getShadow(float3 posWorld, float3 planetShadowPos, float planetShadowSca)
{
	// SOFT SPHERE SHADOW
	// port of https://en.wikibooks.org/wiki/Cg_Programming/Unity/Soft_Shadows_of_Spheres

	float3 	viewDirection = normalize(_WorldSpaceCameraPos - posWorld);
	float3 	lightDirection = normalize(_WorldSpaceLightPos0.xyz);

	// computation of level of shadowing w  
	float3 sphereDirection = planetShadowPos - posWorld;
	float sphereDistance = length(sphereDirection);
	sphereDirection = sphereDirection / sphereDistance;
	float d = length(cross(lightDirection, sphereDirection)) - (planetShadowSca / sphereDistance);
	float w = 0;
	if (dot(lightDirection, sphereDirection) > 0)
	{
		w = smoothstep(-1, 1, -d / LIGHT_DISTANCE);
	}
	return w;
}

#endif