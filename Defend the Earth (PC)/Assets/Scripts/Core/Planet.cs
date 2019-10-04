using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public bool mobileShader = false;
    public bool atmosphere = true;
    public bool updateChangeInRealTime = true;

    public Material PlanetMaterial;
    public Material ringMaterial;

    public float hdrExposure = 1.0f;
    public Color atmoColor = new Color(0, 0, 0);
    public float atmoStrength = 10.0f;

    private float kr = 0.0025f;
    private float km = 0.0010f;
    private float outerScaleFactor = 1.015f;
    private float innerRadius;
    private float outerRadius;
    private float scaleDepth = 0.25f;
    private float scale;
    private float gamma = 1.0f;

    private Transform[] shadow = new Transform[10];
    private int shadowNumber = 0;
    // only 10 planet shadow allowed (hard coded in shader) but you can state less for better performance

    public void setShadowNumber(int n)
    {
        n = (int)Mathf.Clamp(n, 0, 10);
        shadowNumber = n;
    }

    public void setShadow(Transform p, int n)
    {
        if (n < 10 && n >= 0)
        {
            shadow[n] = p;
            if (PlanetMaterial)
                InitMaterial(PlanetMaterial);
            if (ringMaterial)
                InitRingMaterial(ringMaterial);
        }
    }

    void Start()
    {
        if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
        {
            gamma = 2.2f;
            #if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			gamma = 1.8f;
            #endif
        }
        innerRadius = transform.localScale.x;
        outerRadius = outerScaleFactor * transform.localScale.x;

        scale = 1.0f / (outerRadius - innerRadius);
        if (PlanetMaterial)
            InitMaterial(PlanetMaterial);
        if (ringMaterial)
            InitRingMaterial(ringMaterial);
    }

    void LateUpdate()
    {
        if (updateChangeInRealTime)
        {
            if (PlanetMaterial)
                InitMaterial(PlanetMaterial);
            if (ringMaterial)
                InitRingMaterial(ringMaterial);
        }
    }

    void InitRingMaterial(Material mat)
    {
        string[] keyword = new string[1];
        keyword[0] = mobileShader == true ? "MOBILE_ON" : "MOBILE_OFF";
        mat.shaderKeywords = keyword;
        mat.SetVector("v3Translate", transform.position);
        mat.SetFloat("fInnerRadius", innerRadius);
        mat.SetFloat("shadowNumber", shadowNumber);
        int i = -1;
        while (++i < shadowNumber && shadow[i])
        {
            mat.SetVector("planetShadowPos" + i, shadow[i].position);
            mat.SetFloat("planetShadowSca" + i, shadow[i].localScale.x);
        }
    }

    void InitMaterial(Material mat)
    {
        string[] keyword = new string[2];
        keyword[0] = mobileShader == true ? "MOBILE_ON" : "MOBILE_OFF";
        keyword[1] = atmosphere == true ? "ATMO_ON" : "ATMO_OFF";
        mat.shaderKeywords = keyword;
        Vector3 invWL4 = new Vector3(1 - atmoColor.linear.r, 1 - atmoColor.linear.g, 1 - atmoColor.linear.b);
        invWL4 = new Vector3(1.0f / Mathf.Pow(invWL4.x, 4),
                             1.0f / Mathf.Pow(invWL4.y, 4),
                             1.0f / Mathf.Pow(invWL4.z, 4));
        mat.SetFloat("_Gamma", gamma);
        mat.SetVector("v3InvWavelength", invWL4);
        mat.SetFloat("fOuterRadius", outerRadius);
        mat.SetFloat("fInnerRadius", innerRadius);
        mat.SetFloat("fKrESun", kr * atmoStrength);
        mat.SetFloat("fKmESun", km * atmoStrength);
        mat.SetFloat("fKr4PI", kr * 4.0f * Mathf.PI);
        mat.SetFloat("fKm4PI", km * 4.0f * Mathf.PI);
        mat.SetFloat("fScale", scale);
        mat.SetFloat("fScaleDepth", scaleDepth);
        mat.SetFloat("fScaleOverScaleDepth", scale / scaleDepth);
        mat.SetFloat("fHdrExposure", hdrExposure);
        mat.SetVector("v3Translate", transform.position);
        mat.SetFloat("shadowNumber", shadowNumber);
        int i = -1;
        while (++i < shadowNumber && shadow[i])
        {
            mat.SetVector("planetShadowPos" + i, shadow[i].position);
            mat.SetFloat("planetShadowSca" + i, shadow[i].localScale.x);
        }
    }
}

