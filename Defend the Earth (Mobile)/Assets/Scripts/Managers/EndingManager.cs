using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [Header("Credits Settings")]
    [Tooltip("The Y position credits start at.")] [SerializeField] private float creditsY = 530;
    [SerializeField] private float creditsScrollSpeed = 0.375f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas endingMenu = null;
    [SerializeField] private Canvas creditsMenu = null;
    [SerializeField] private RectTransform credits = null;
    [SerializeField] private Text moneyCount = null;
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
        Application.targetFrameRate = 60;
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
        if (Input.touchCount > 0)
        {
            if (!spedupCredits)
            {
                spedupCredits = true;
                creditsScrollSpeed *= 2;
            }
        } else
        {
            if (spedupCredits)
            {
                spedupCredits = false;
                creditsScrollSpeed *= 0.5f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && creditsMenu.enabled)
        {
            creditsMenu.enabled = false;
            endingMenu.enabled = true;
            StopCoroutine(scrollCredits());
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
        if (PlayerPrefs.GetInt("Level") > PlayerPrefs.GetInt("MaxLevels")) //Checks if current level is more than the maximum amount
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("MaxLevels"));
        } else if (PlayerPrefs.GetInt("Level") < 1) //Checks if current level is less than 1
        {
            PlayerPrefs.SetInt("Level", 1);
        }

        //Checks if the player has a unowned spaceship equipped
        if (PlayerPrefs.GetInt("Has" + PlayerPrefs.GetString("Spaceship")) <= 0) PlayerPrefs.SetString("Spaceship", "SpaceFighter");

        //Checks if the player upgrades are above maximum values
        if (PlayerPrefs.GetFloat("DamageMultiplier") > 1.5f)
        {
            PlayerPrefs.SetFloat("DamageMultiplier", 1.5f);
            PlayerPrefs.SetInt("DamagePercentage", 50);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("SpeedMultiplier") > 1.2f)
        {
            PlayerPrefs.SetFloat("SpeedMultiplier", 1.2f);
            PlayerPrefs.SetInt("SpeedPercentage", 20);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("HealthMultiplier") > 2)
        {
            PlayerPrefs.SetFloat("HealthMultiplier", 2);
            PlayerPrefs.SetInt("HealthPercentage", 100);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("MoneyMultiplier") > 3)
        {
            PlayerPrefs.SetFloat("MoneyMultiplier", 3);
            PlayerPrefs.SetInt("MoneyPercentage", 200);
            PlayerPrefs.Save();
        }

        //Checks if money is below 0
        if (long.Parse(PlayerPrefs.GetString("Money")) < 0)
        {
            PlayerPrefs.SetString("Money", "0");
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
    }

    #region Menu Functions
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
    #endregion

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (LoadingTipArray.instance && LoadingTipArray.instance.tips.Length > 0 && PlayerPrefs.GetInt("Tips") >= 1) currentLoadingTip = LoadingTipArray.instance.tips[Random.Range(0, LoadingTipArray.instance.tips.Length)];
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                if (load.progress < 0.9f)
                {
                    load.allowSceneActivation = false;
                    loadingSlider.value = load.progress;
                    loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                    anyKeyPrompt.SetActive(false);
                } else
                {
                    if (PlayerPrefs.GetInt("Tips") >= 1)
                    {
                        if (Input.anyKeyDown) load.allowSceneActivation = true;
                        loadingSlider.value = 1;
                        loadingPercentage.text = "100%";
                        anyKeyPrompt.SetActive(true);
                    } else
                    {
                        load.allowSceneActivation = true;
                        loadingSlider.value = 1;
                        loadingPercentage.text = "100%";
                        anyKeyPrompt.SetActive(false);
                    }
                }
                endingMenu.enabled = false;
                creditsMenu.enabled = false;
                yield return null;
            }
        }
    }
}