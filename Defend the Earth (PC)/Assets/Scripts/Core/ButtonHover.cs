using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] private bool isImage = false;
    [SerializeField] private Color32 normalColor = new Color32(200, 200, 200, 255);
    [SerializeField] private Color32 hoverColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Text[] textsToShow = new Text[0];

    private Text text;
    private Image image;
    private Outline outline;

    void Start()
    {
        if (!isImage)
        {
            text = GetComponent<Text>();
            outline = GetComponent<Outline>();
            text.color = normalColor;
            if (outline) outline.effectColor = new Color32((byte)(normalColor.r * 0.5), (byte)(normalColor.g * 0.5), (byte)(normalColor.b * 0.5), 255);
        } else
        {
            image = GetComponent<Image>();
            image.color = normalColor;
        }
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = false;
        }
    }

    public void OnMouseEnter()
    {
        if (!isImage)
        {
            text.color = hoverColor;
            if (outline) outline.effectColor = new Color32((byte)(hoverColor.r * 0.5), (byte)(hoverColor.g * 0.5), (byte)(hoverColor.b * 0.5), 255);
        } else
        {
            image.color = hoverColor;
        }
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = true;
        }
    }

    public void OnMouseExit()
    {
        if (!isImage)
        {
            text.color = normalColor;
            if (outline) outline.effectColor = new Color32((byte)(normalColor.r * 0.5), (byte)(normalColor.g * 0.5), (byte)(normalColor.b * 0.5), 255);
        } else
        {
            image.color = normalColor;
        }
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = false;
        }
    }
}