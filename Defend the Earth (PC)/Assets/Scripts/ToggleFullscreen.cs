using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreen : MonoBehaviour
{
    private Text fullscreenText;

    void Start()
    {
        fullscreenText = GetComponent<Text>();
    }

    void Update()
    {
        if (!Screen.fullScreen)
        {
            fullscreenText.text = "Change to Fullscreen";
            fullscreenText.rectTransform.sizeDelta = new Vector2(260, 41);
        } else
        {
            fullscreenText.text = "Change to Windowed Mode";
            fullscreenText.rectTransform.sizeDelta = new Vector2(330, 41);
        }
    }

    public void changeFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}