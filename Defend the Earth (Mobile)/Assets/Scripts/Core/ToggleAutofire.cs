using UnityEngine;
using UnityEngine.UI;

public class ToggleAutofire : MonoBehaviour
{
    [SerializeField] private Text info = null;
    [SerializeField] private AudioClip buttonClick = null;

    private Text main;
    private AudioSource audioSource;

    void Start()
    {
        main = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Autofire") <= 0)
        {
            main.text = "Enable Autofire";
            main.rectTransform.sizeDelta = new Vector2(192, 41);
        } else if (PlayerPrefs.GetInt("Autofire") >= 1)
        {
            main.text = "Disable Autofire";
            main.rectTransform.sizeDelta = new Vector2(202, 41);
        }
    }

    public void toggle()
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
        if (PlayerPrefs.GetInt("Autofire") <= 0)
        {
            PlayerPrefs.SetInt("Autofire", 1);
            if (info)
            {
                info.text = "Autofire has been enabled. You will now always fire while moving.";
                CancelInvoke("resetInfo");
                Invoke("resetInfo", 2);
            }
        } else if (PlayerPrefs.GetInt("Autofire") >= 1)
        {
            PlayerPrefs.SetInt("Autofire", 0);
            if (info)
            {
                info.text = "Autofire has been disabled. You can fire by pressing with two fingers.";
                CancelInvoke("resetInfo");
                Invoke("resetInfo", 2);
            }
        }
    }

    void resetInfo()
    {
        if (info) info.text = "";
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
