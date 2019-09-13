using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    public Color32 normalColor = new Color32(200, 200, 200, 255);
    public Color32 hoverColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Text[] textsToShow = new Text[0];

    private Text text;
    private Outline outline;

    void Start()
    {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        text.color = normalColor;
        if (outline) outline.effectColor = new Color32((byte)(normalColor.r * 0.5), (byte)(normalColor.g * 0.5), (byte)(normalColor.b * 0.5), 255);
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = false;
        }
    }

    public void OnMouseEnter()
    {
        text.color = hoverColor;
        if (outline) outline.effectColor = new Color32((byte)(hoverColor.r * 0.5), (byte)(hoverColor.g * 0.5), (byte)(hoverColor.b * 0.5), 255);
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = true;
        }
    }

    public void OnMouseExit()
    {
        text.color = normalColor;
        if (outline) outline.effectColor = new Color32((byte)(normalColor.r * 0.5), (byte)(normalColor.g * 0.5), (byte)(normalColor.b * 0.5), 255);
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = false;
        }
    }
}