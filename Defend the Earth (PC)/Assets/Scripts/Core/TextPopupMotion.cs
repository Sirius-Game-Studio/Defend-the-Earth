using UnityEngine;
using TMPro;

public class TextPopupMotion : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshPro textMesh;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        textMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, 1.25f) * Time.deltaTime;
        textMesh.alpha -= 1.25f * Time.deltaTime;
    }
}
