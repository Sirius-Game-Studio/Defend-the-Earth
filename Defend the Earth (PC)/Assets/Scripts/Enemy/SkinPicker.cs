using UnityEngine;

public class SkinPicker : MonoBehaviour
{
    public string skin = "Default";
    [Tooltip("Default Skin")] [SerializeField] private Texture defaultAlbedo = null;
    [Tooltip("Green Skin")] [SerializeField] private Texture greenAlbedo = null;
    [Tooltip("White Skin")] [SerializeField] private Texture whiteAlbedo = null;

    private new Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        float random = Random.value;
        if (random <= 0.33)
        {
            renderer.material.SetTexture("_MainTex", defaultAlbedo);
            skin = "Default";
        } else if (random <= 0.66)
        {
            renderer.material.SetTexture("_MainTex", greenAlbedo);
            skin = "Green";
        } else
        {
            renderer.material.SetTexture("_MainTex", whiteAlbedo);
            skin = "White";
        }
        enabled = false;
    }
}
