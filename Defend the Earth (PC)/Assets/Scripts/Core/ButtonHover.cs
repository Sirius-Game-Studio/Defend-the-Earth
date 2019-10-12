using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] private bool isImage = false;
    public Color32 normalColor = new Color32(200, 200, 200, 255);
    public Color32 hoverColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Text[] textsToShow = new Text[0];

    private Text text;
    private Image image;
    private Outline outline;
    private bool hovering = false;

    void OnEnable()
    {
        text = GetComponent<Text>();
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
        hovering = false;
        setState(false);
        CancelInvoke("onClickColor");
    }

    void OnDisable()
    {
        hovering = false;
        setState(false);
        CancelInvoke("onClickColor");
    }

    public void OnMouseEnter()
    {
        hovering = true;
        setState(true);
    }

    public void OnMouseExit()
    {
        hovering = false;
        setState(false);
    }

    public void OnMouseClick()
    {
        setState(false);
        CancelInvoke("onClickColor");
        Invoke("onClickColor", 0.15f);
        if (textsToShow.Length > 0)
        {
            foreach (Text t in textsToShow) if (t) t.enabled = true;
        }
    }

    void setState(bool state)
    {
        if (state)
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
        } else
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

    void onClickColor()
    {
        if (hovering)
        {
            if (!isImage)
            {
                text.color = hoverColor;
                if (outline) outline.effectColor = new Color32((byte)(hoverColor.r * 0.5), (byte)(hoverColor.g * 0.5), (byte)(hoverColor.b * 0.5), 255);
            } else
            {
                image.color = hoverColor;
            }
        }
    }
}