using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [Header("Credits Settings")]
    [Tooltip("The Y position credits start at.")] [SerializeField] private float creditsY = 620;
    [SerializeField] private float creditsScrollSpeed = 0.25f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas endingMenu = null;
    [SerializeField] private Canvas creditsMenu = null;
    [SerializeField] private RectTransform credits = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text controllerSpeedUpButton = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [SerializeField] private GameObject anyKeyPrompt = null;
    [SerializeField] private Text loadingTip = null;
    [SerializeField] private AudioMixer audioMixer = null;

    private AudioSource audioSource;
    private bool spedupCredits = false;
    private string currentLoadingTip = "";
    private bool loading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
        currentLoadingTip = "";
        loading = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
        PlayerPrefs.SetInt("Level", 1);
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
        } else
        {
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
        } else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        }
        PlayerPrefs.Save();
        endingMenu.enabled = true;
        creditsMenu.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0)) // X/Cross (Xbox/PS Controller)
        {
            if (!spedupCredits)
            {
                spedupCredits = true;
                creditsScrollSpeed *= 2;
                controllerSpeedUpButton.text = "Slow Down";
            }
        } else if (!Input.GetKey(KeyCode.Space) || !Input.GetKey(KeyCode.JoystickButton0)) // X/Cross (Xbox/PS Controller)
        {
            if (spedupCredits)
            {
                spedupCredits = false;
                creditsScrollSpeed *= 0.5f;
                controllerSpeedUpButton.text = "Speed Up";
            }
        }
        if (creditsMenu.enabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) // B/Circle (Xbox/PS Controller)
            {
                creditsMenu.enabled = false;
                endingMenu.enabled = true;
                StopCoroutine(scrollCredits());
            }
        }
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
            loadingScreen.SetActive(false);
            loadingTip.text = "";
            moneyCount.gameObject.SetActive(true);
        } else
        {
            loadingScreen.SetActive(true);
            loadingTip.text = currentLoadingTip; 
            moneyCount.gameObject.SetActive(false);
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
    }

    public void clickCredits()
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

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (LoadingTipArray.instance && LoadingTipArray.instance.tips.Length > 0) currentLoadingTip = LoadingTipArray.instance.tips[Random.Range(0, LoadingTipArray.instance.tips.Length)];
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                if (load.progress < 0.9f)
                {
                    load.allowSceneActivation = false;
                    loadingSlider.value = load.progress;
                    loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                    anyKeyPrompt.SetActive(false);
                } else
                {
                    if (Input.anyKeyDown) load.allowSceneActivation = true;
                    loadingSlider.value = 1;
                    loadingPercentage.text = "100%";
                    anyKeyPrompt.SetActive(true);
                }
                endingMenu.enabled = false;
                creditsMenu.enabled = false;
                yield return null;
            }
        }
    }
}