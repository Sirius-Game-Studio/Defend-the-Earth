using UnityEngine;
using UnityEngine.UI;

public class ToggleTips : MonoBehaviour
{
    [SerializeField] private Vector2 enableTextSize = new Vector2(143, 41);
    [SerializeField] private Vector2 disableTextSize = new Vector2(152, 41);
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
        if (PlayerPrefs.GetInt("Tips") <= 0)
        {
            toggleText.text = "Enable Tips";
            toggleText.rectTransform.sizeDelta = enableTextSize;
        } else
        {
            toggleText.text = "Disable Tips";
            toggleText.rectTransform.sizeDelta = disableTextSize;
        }
    }

    public void toggleTips()
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
        if (PlayerPrefs.GetInt("Tips") <= 0)
        {
            PlayerPrefs.SetInt("Tips", 1);
        } else
        {
            PlayerPrefs.SetInt("Tips", 0);
        }
        PlayerPrefs.Save();
    }
}