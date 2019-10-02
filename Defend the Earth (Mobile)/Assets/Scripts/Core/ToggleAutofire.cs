using UnityEngine;
using UnityEngine.UI;

public class ToggleAutofire : MonoBehaviour
{
    [SerializeField] private Text info = null;
    [SerializeField] private Vector2 enableTextSize = new Vector2(192, 41);
    [SerializeField] private Vector2 disableTextSize = new Vector2(202, 41);
    [SerializeField] private AudioClip buttonClick = null;

    private Text toggleText;
    private AudioSource audioSource;

    void Start()
    {
        toggleText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Autofire") <= 0)
        {
            toggleText.text = "Enable Autofire";
            toggleText.rectTransform.sizeDelta = enableTextSize;
        } else if (PlayerPrefs.GetInt("Autofire") >= 1)
        {
            toggleText.text = "Disable Autofire";
            toggleText.rectTransform.sizeDelta = disableTextSize;
        }
    }

    public void toggle()
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
        if (PlayerPrefs.GetInt("Autofire") <= 0)
        {
            PlayerPrefs.SetInt("Autofire", 1);
            if (info)
            {
                info.text = "Autofire has been enabled. You will now always fire while moving.";
                CancelInvoke("resetInfo");
                Invoke("resetInfo", 2);
            }
        } else
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
}
