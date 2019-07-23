using UnityEngine;

public class SkinPicker : MonoBehaviour
{
    public int skin = 1; //1 is default, 2 is green, 3 is white
    [Tooltip("Default Skin")] [SerializeField] private Texture defaultAlbedo = null;
    [Tooltip("Green Skin")] [SerializeField] private Texture greenAlbedo = null;
    [Tooltip("White Skin")] [SerializeField] private Texture whiteAlbedo = null;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        float random = Random.value;
        if (random <= 0.33)
        {
            meshRenderer.material.SetTexture("_MainTex", defaultAlbedo);
            skin = 1;
        } else if (random <= 0.66)
        {
            meshRenderer.material.SetTexture("_MainTex", greenAlbedo);
            skin = 2;
        } else
        {
            meshRenderer.material.SetTexture("_MainTex", whiteAlbedo);
            skin = 3;
        }
        enabled = false;
    }
}
