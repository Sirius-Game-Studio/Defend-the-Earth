using UnityEngine;

public class SkinPicker : MonoBehaviour
{
    public Texture[] textures = new Texture[0];
    public Texture[] emissionMaps = new Texture[0];
    public Color32[] lightColors = new Color32[0];
    public int texture = 0;

    private new Renderer renderer;
    private new Light light;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        light = GetComponent<Light>();
        if (textures.Length > 0)
        {
            texture = Random.Range(0, textures.Length);
            renderer.material.SetTexture("_MainTex", textures[texture]);
        }
        if (emissionMaps.Length > 0 && emissionMaps[texture]) renderer.material.SetTexture("_EmissionMap", emissionMaps[texture]);
        if (light && lightColors.Length > 0) light.color = lightColors[texture];
        enabled = false;
    }
}
