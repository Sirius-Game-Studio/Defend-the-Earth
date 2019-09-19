using UnityEngine;

public class SkinPicker : MonoBehaviour
{
    public bool randomized = true;
    public Texture[] textures = new Texture[0];
    public int texture = 0;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (textures.Length > 0)
        {
            if (randomized) texture = Random.Range(0, textures.Length - 1);
            meshRenderer.material.SetTexture("_MainTex", textures[texture]);
        }
        enabled = false;
    }
}
