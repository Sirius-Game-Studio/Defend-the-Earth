using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreen : MonoBehaviour
{
    [SerializeField] private Vector2 fullscreenTextSize = new Vector2(260, 41);
    [SerializeField] private Vector2 windowedModeTextSize = new Vector2(330, 41);
    [SerializeField] private AudioClip buttonClick = null;

    private Text toggleText;
    private AudioSource audioSource;

    void Start()
    {
        toggleText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
    }

    void Update()
    {
        if (!Screen.fullScreen)
        {
            toggleText.text = "Change to Fullscreen";
            toggleText.rectTransform.sizeDelta = fullscreenTextSize;
        } else
        {
            toggleText.text = "Change to Windowed Mode";
            toggleText.rectTransform.sizeDelta = windowedModeTextSize;
        }
    }

    public void changeFullscreen()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);
            } else
            {
                audioSource.Play();
            }
        }
        Screen.fullScreen = !Screen.fullScreen;
    }
}