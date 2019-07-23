using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private Canvas endingMenu = null;
    [SerializeField] private Canvas creditsMenu = null;
    [SerializeField] private RectTransform credits = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text controllerSpeedUpButton = null;
    [SerializeField] private GameObject loadingText = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [Tooltip("The Y position credits start at.")] [SerializeField] private float creditsY = 570;
    [SerializeField] private float creditsScrollSpeed = 0.25f;

    private bool spedupCredits = false;
    private bool loading = false;

    void Awake()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        PlayerPrefs.DeleteKey("Difficulty");
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
        }
        endingMenu.enabled = true;
        creditsMenu.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
        {
            if (!spedupCredits)
            {
                spedupCredits = true;
                creditsScrollSpeed *= 2;
                controllerSpeedUpButton.text = "Slow Down";
            }
        } else if (!Input.GetKey(KeyCode.Space) || !Input.GetKey(KeyCode.JoystickButton0))
        {
            if (spedupCredits)
            {
                spedupCredits = false;
                creditsScrollSpeed *= 0.5f;
                controllerSpeedUpButton.text = "Speed Up";
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (creditsMenu.enabled) clickCredits();
        }
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
        if (!creditsMenu.enabled) credits.anchoredPosition = new Vector2(0, creditsY);
        if (PlayerPrefs.GetString("Money") != "")
        {
            moneyCount.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            moneyCount.text = "$0";
        }
        if (!loading)
        {
            loadingText.SetActive(false);
            moneyCount.gameObject.SetActive(true);
        } else
        {
            loadingText.SetActive(true);
            moneyCount.gameObject.SetActive(false);
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
    }

    public void clickCredits()
    {
        if (!creditsMenu.enabled)
        {
            creditsMenu.enabled = true;
            endingMenu.enabled = false;
            StartCoroutine(scrollCredits());
        } else
        {
            creditsMenu.enabled = false;
            endingMenu.enabled = true;
            StopCoroutine(scrollCredits());
        }
    }

    public void exitToMainMenu()
    {
        StartCoroutine(loadScene("Main Menu"));
    }

    IEnumerator scrollCredits()
    {
        while (creditsMenu.enabled)
        {
            yield return new WaitForEndOfFrame();
            if (creditsMenu.enabled) credits.anchoredPosition -= new Vector2(0, creditsScrollSpeed);
            if (credits.anchoredPosition.y <= -creditsY)
            {
                endingMenu.enabled = true;
                creditsMenu.enabled = false;
                yield break;
            }
        }
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

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                loadingSlider.value = load.progress;
                loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                endingMenu.enabled = false;
                creditsMenu.enabled = false;
                yield return null;
            }
            loading = false;
            loadingSlider.value = 0;
            loadingPercentage.text = "0%";
        } else
        {
            StopCoroutine(loadScene(scene));
        }
    }
}
