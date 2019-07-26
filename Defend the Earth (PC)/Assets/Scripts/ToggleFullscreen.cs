using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreen : MonoBehaviour
{
    [SerializeField] private Vector2 fullscreenTextSize = new Vector2(260, 41);
    [SerializeField] private Vector2 windowedModeTextSize = new Vector2(330, 41);
    [SerializeField] private AudioClip buttonClick = null;

    private Text fullscreenText;
    private AudioSource audioSource;

    void Start()
    {
        fullscreenText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!Screen.fullScreen)
        {
            fullscreenText.text = "Change to Fullscreen";
            fullscreenText.rectTransform.sizeDelta = fullscreenTextSize;
        } else
        {
            fullscreenText.text = "Change to Windowed Mode";
            fullscreenText.rectTransform.sizeDelta = windowedModeTextSize;
        }
    }

    public void changeFullscreen()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        Screen.fullScreen = !Screen.fullScreen;
    }

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }
}