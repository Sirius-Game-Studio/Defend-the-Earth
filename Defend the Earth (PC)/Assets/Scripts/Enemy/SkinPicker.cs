using UnityEngine;

public class SkinPicker : MonoBehaviour
{
    public int skin = 1; //1 is default, 2 is green, 3 is white
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
            skin = 1;
        } else if (random <= 0.66)
        {
            renderer.material.SetTexture("_MainTex", greenAlbedo);
            skin = 2;
        } else
        {
            renderer.material.SetTexture("_MainTex", whiteAlbedo);
            skin = 3;
        }
        enabled = false;
    }
}
