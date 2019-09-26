using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas spaceshipsMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas selectGamemodeMenu = null;
    [SerializeField] private Canvas selectDifficultyMenu = null;
    [SerializeField] private Text currentLevelText = null;
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [SerializeField] private Text loadingTip = null;
    [SerializeField] private GameObject anyKeyPrompt = null;
    [SerializeField] private AudioMixer audioMixer = null;

    private AudioSource audioSource;
    private Controls input;
    private string currentLoadingTip = "";
    private bool loading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        input = new Controls();
        if (audioSource) audioSource.ignoreListenerPause = true;
        currentLoadingTip = "";
        loading = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        }
        mainMenu.enabled = true;
        shopMenu.enabled = false;
        spaceshipsMenu.enabled = false;
        upgradesMenu.enabled = false;
        settingsMenu.enabled = false;
        selectGamemodeMenu.enabled = false;
        selectDifficultyMenu.enabled = false;
        ShopManager.instance.page = 1;
        ShopManager.instance.open = false;
    }

    void OnEnable()
    {
        input.Enable();
        input.Gameplay.Fullscreen.performed += context => toggleFullscreen();
        input.Menu.CloseMenu.performed += context => closeMenu();
    }

    void OnDisable()
    {
        input.Disable();
        input.Gameplay.Fullscreen.performed -= context => toggleFullscreen();
        input.Menu.CloseMenu.performed -= context => closeMenu();
    }

    void Update()
    {
        //Updates the money counter
        if (PlayerPrefs.GetString("Money") != "")
        {
            moneyCount.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            moneyCount.text = "$0";
        }

        if (PlayerPrefs.GetInt("Level") > 0)
        {
            currentLevelText.text = "Level: " + PlayerPrefs.GetInt("Level");
        } else
        {
            currentLevelText.text = "Level: 1";
        }
        if (PlayerPrefs.HasKey("HighScore"))
        {
            long highScore = long.Parse(PlayerPrefs.GetString("HighScore"));
            if (highScore > 0)
            {
                highScoreText.text = "High Score: " + highScore;
            } else
            {
                highScoreText.text = "High Score: 0";
            }
        } else
        {
            highScoreText.text = "High Score: 0";
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
        if (PlayerPrefs.GetInt("Level") < 1) //Checks if the current level is less than 1
        {
            PlayerPrefs.SetInt("Level", 1);
        } else if (PlayerPrefs.GetInt("Level") > PlayerPrefs.GetInt("MaxLevels")) //Checks if the current level is more than the maximum amount of levels
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("MaxLevels"));
        }

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

    void toggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void closeMenu()
    {
        if (shopMenu.enabled)
        {
            shopMenu.enabled = false;
            mainMenu.enabled = true;
        } else if (spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            ShopManager.instance.page = 1;
            ShopManager.instance.open = false;
        } else if (upgradesMenu.enabled)
        {
            upgradesMenu.enabled = false;
            shopMenu.enabled = true;
        } else if (settingsMenu.enabled)
        {
            settingsMenu.enabled = false;
            mainMenu.enabled = true;
        } else if (selectGamemodeMenu.enabled)
        {
            selectGamemodeMenu.enabled = false;
            mainMenu.enabled = true;
        } else if (selectDifficultyMenu.enabled)
        {
            selectDifficultyMenu.enabled = false;
            selectGamemodeMenu.enabled = true;
        }
    }

    public void clickPlayGame()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
        }
        if (!selectGamemodeMenu.enabled)
        {
            selectGamemodeMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            selectGamemodeMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickShop()
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
        if (!shopMenu.enabled)
        {
            shopMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            shopMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickSettings()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);;
            } else
            {
                audioSource.Play();
            }
        }
        if (!settingsMenu.enabled)
        {
            settingsMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            settingsMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickQuitGame()
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
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void clickCampaign()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
        }
        if (!selectDifficultyMenu.enabled)
        {
            selectDifficultyMenu.enabled = true;
            selectGamemodeMenu.enabled = false;
        } else
        {
            selectDifficultyMenu.enabled = false;
            selectGamemodeMenu.enabled = true;
        }
    }

    public void clickSpaceships()
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
        if (!spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = true;
            shopMenu.enabled = false;
            ShopManager.instance.open = true;
        } else
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            ShopManager.instance.page = 1;
            ShopManager.instance.open = false;
        }
    }

    public void clickUpgrades()
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
        if (!upgradesMenu.enabled)
        {
            upgradesMenu.enabled = true;
            shopMenu.enabled = false;
            ShopManager.instance.open = true;
        } else
        {
            upgradesMenu.enabled = false;
            shopMenu.enabled = true;
            ShopManager.instance.open = false;
        }
    }

    public void startGame(int difficulty)
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
        if (PlayerPrefs.GetInt("Level") > 0)
        {
            PlayerPrefs.SetInt("Difficulty", difficulty);
            if (PlayerPrefs.GetInt("Difficulty") < 1)
            {
                PlayerPrefs.SetInt("Difficulty", 1);
            } else if (PlayerPrefs.GetInt("Difficulty") > 4)
            {
                PlayerPrefs.SetInt("Difficulty", 4);
            }
            StartCoroutine(loadScene("Level " + PlayerPrefs.GetInt("Level")));
        } else
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("Difficulty", difficulty);
            if (PlayerPrefs.GetInt("Difficulty") < 1)
            {
                PlayerPrefs.SetInt("Difficulty", 1);
            } else if (PlayerPrefs.GetInt("Difficulty") > 4)
            {
                PlayerPrefs.SetInt("Difficulty", 4);
            }
            StartCoroutine(loadScene("Level 1"));
        }
        PlayerPrefs.Save();
    }

    public void startEndless()
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
        StartCoroutine(loadScene("Endless"));
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
                mainMenu.enabled = false;
                shopMenu.enabled = false;
                spaceshipsMenu.enabled = false;
                upgradesMenu.enabled = false;
                settingsMenu.enabled = false;
                selectGamemodeMenu.enabled = false;
                selectDifficultyMenu.enabled = false;
                yield return null;
            }
        }
    }
}