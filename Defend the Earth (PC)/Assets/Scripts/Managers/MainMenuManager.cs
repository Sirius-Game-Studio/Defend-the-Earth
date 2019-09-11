using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [Header("Upgrades Menu")]
    [SerializeField] private Text damageText = null;
    [SerializeField] private Text fireRateText = null;
    [SerializeField] private Text speedText = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private Text damagePrice = null;
    [SerializeField] private Text speedPrice = null;
    [SerializeField] private Text healthPrice = null;
    [SerializeField] private Text moneyPrice = null;
    [SerializeField] private Text upgradeDamageButton = null;
    [SerializeField] private Text upgradeSpeedButton = null;
    [SerializeField] private Text upgradeHealthButton = null;
    [SerializeField] private Text upgradeMoneyButton = null;
    
    [Header("Sound Menu")]
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private Slider musicSlider = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas spaceshipsMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas graphicsQualityMenu = null;
    [SerializeField] private Canvas soundMenu = null;
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

    private AudioSource audioSource;
    private string currentLoadingTip = "";
    private bool loading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
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
            soundSlider.value = getVolumeData(true);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
            musicSlider.value = getVolumeData(false);
        }
        PlayerPrefs.Save();
        mainMenu.enabled = true;
        shopMenu.enabled = false;
        spaceshipsMenu.enabled = false;
        upgradesMenu.enabled = false;
        settingsMenu.enabled = false;
        graphicsQualityMenu.enabled = false;
        soundMenu.enabled = false;
        selectGamemodeMenu.enabled = false;
        selectDifficultyMenu.enabled = false;
        loadingTip.text = "";
        ShopManager.instance.page = 1;
    }

    void Update()
    {
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) // B/Circle (Xbox/PS Controller)
        {
            if (shopMenu.enabled)
            {
                shopMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (spaceshipsMenu.enabled)
            {
                spaceshipsMenu.enabled = false;
                shopMenu.enabled = true;
                if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
                {
                    ShopManager.instance.page = 1;
                } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
                {
                    ShopManager.instance.page = 2;
                } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
                {
                    ShopManager.instance.page = 3;
                } else if (PlayerPrefs.GetString("Spaceship") == "QuadShooter")
                {
                    ShopManager.instance.page = 4;
                } else if (PlayerPrefs.GetString("Spaceship") == "PointVoidBreaker")
                {
                    ShopManager.instance.page = 5;
                } else if (PlayerPrefs.GetString("Spaceship") == "Annihilator")
                {
                    ShopManager.instance.page = 6;
                } else
                {
                    ShopManager.instance.page = 1;
                }
            } else if (upgradesMenu.enabled)
            {
                upgradesMenu.enabled = false;
                shopMenu.enabled = true;
            } else if (settingsMenu.enabled)
            {
                settingsMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (graphicsQualityMenu.enabled)
            {
                graphicsQualityMenu.enabled = false;
                settingsMenu.enabled = true;
            } else if (soundMenu.enabled)
            {
                soundMenu.enabled = false;
                settingsMenu.enabled = true;
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

        //Updates volume data to match the slider values
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();

        //Updates the money counter
        if (PlayerPrefs.GetString("Money") != "")
        {
            moneyCount.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            moneyCount.text = "$0";
        }

        //Updates the upgrade price text
        damageText.text = "+" + PlayerPrefs.GetInt("DamagePercentage") + "% Damage";
        speedText.text = "+" + PlayerPrefs.GetInt("SpeedPercentage") + "% Speed";
        healthText.text = "+" + PlayerPrefs.GetInt("HealthPercentage") + "% Health";
        fireRateText.text = "+" + PlayerPrefs.GetInt("MoneyPercentage") + "% Money";

        //Sets the states of upgrade price text
        priceTextState(damagePrice, upgradeDamageButton, true, true, true, "DamagePercentage", "DamagePrice", 8, 50, false);
        priceTextState(speedPrice, upgradeSpeedButton, true, true, true, "SpeedPercentage", "SpeedPrice", 5, 20, false);
        priceTextState(healthPrice, upgradeHealthButton, true, true, true, "HealthPercentage", "HealthPrice", 7, 100, false);
        priceTextState(moneyPrice, upgradeMoneyButton, true, true, true, "MoneyPercentage", "MoneyPrice", 4, 200, false);

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

    public void clickPlayGame()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick, getVolumeData(true));
        } else
        {
            audioSource.volume = getVolumeData(true);
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
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
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
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
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
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #endif
    }

    public void clickCampaign()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick, getVolumeData(true));
        } else
        {
            audioSource.volume = getVolumeData(true);
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
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = true;
            shopMenu.enabled = false;
        } else
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
            {
                ShopManager.instance.page = 1;
            } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
            {
                ShopManager.instance.page = 2;
            } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
            {
                ShopManager.instance.page = 3;
            } else if (PlayerPrefs.GetString("Spaceship") == "QuadShooter")
            {
                ShopManager.instance.page = 4;
            } else if (PlayerPrefs.GetString("Spaceship") == "PointVoidBreaker")
            {
                ShopManager.instance.page = 5;
            } else if (PlayerPrefs.GetString("Spaceship") == "Annihilator")
            {
                ShopManager.instance.page = 6;
            } else
            {
                ShopManager.instance.page = 1;
            }
        }
    }

    public void clickUpgrades()
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
        if (!upgradesMenu.enabled)
        {
            upgradesMenu.enabled = true;
            shopMenu.enabled = false;
        } else
        {
            upgradesMenu.enabled = false;
            shopMenu.enabled = true;
        }
    }

    public void clickGraphicsQuality()
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
        if (!graphicsQualityMenu.enabled)
        {
            graphicsQualityMenu.enabled = true;
            settingsMenu.enabled = false;
        } else
        {
            graphicsQualityMenu.enabled = false;
            settingsMenu.enabled = true;
        }
    }

    public void clickSound()
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
        if (!soundMenu.enabled)
        {
            soundMenu.enabled = true;
            settingsMenu.enabled = false;
        } else
        {
            soundMenu.enabled = false;
            settingsMenu.enabled = true;
        }
    }

    public void startGame(int difficulty)
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
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        StartCoroutine(loadScene("Endless"));
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
            if (LoadingTipArray.instance && LoadingTipArray.instance.tips.Length > 0) currentLoadingTip = LoadingTipArray.instance.tips[Random.Range(0, LoadingTipArray.instance.tips.Length)];
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
                    if (Input.anyKeyDown)
                    {
                        loading = false;
                        load.allowSceneActivation = true;
                    }
                    loadingSlider.value = 1;
                    loadingPercentage.text = "100%";
                    anyKeyPrompt.SetActive(true);
                }
                mainMenu.enabled = false;
                shopMenu.enabled = false;
                spaceshipsMenu.enabled = false;
                upgradesMenu.enabled = false;
                settingsMenu.enabled = false;
                graphicsQualityMenu.enabled = false;
                soundMenu.enabled = false;
                selectGamemodeMenu.enabled = false;
                selectDifficultyMenu.enabled = false;
                yield return null;
            }
        }
    }

    void spaceshipButtonState(Text button, string spaceship)
    {
        if (button && spaceship != "")
        {
            if (PlayerPrefs.GetInt("Has" + spaceship) <= 0)
            {
                button.text = "Buy";
                button.rectTransform.sizeDelta = new Vector2(58, 41);
            } else
            {
                if (PlayerPrefs.GetString("Spaceship") != spaceship)
                {
                    button.text = "Use";
                    button.rectTransform.sizeDelta = new Vector2(46, 41);
                } else if (PlayerPrefs.GetString("Spaceship") == spaceship)
                {
                    button.text = "Using";
                    button.rectTransform.sizeDelta = new Vector2(68, 41);
                }
            }
        }
    }

    void priceTextState(Text main, Text button, bool isUpgrade, bool change, bool useDataKey, string statKey, string priceKey, int price, int max, bool isFloat)
    {
        if (main && statKey != "")
        {
            if (!isFloat)
            {
                if (PlayerPrefs.GetInt(statKey) < max)
                {
                    if (useDataKey)
                    {
                        if (PlayerPrefs.GetInt(priceKey) > 0)
                        {
                            main.text = "$" + PlayerPrefs.GetInt(priceKey);
                        } else
                        {
                            main.text = "Free";
                        }
                    } else
                    {
                        if (price > 0)
                        {
                            main.text = "$" + price;
                        } else
                        {
                            main.text = "Free";
                        }
                    }
                    main.color = new Color32(133, 187, 101, 255);
                    main.GetComponent<Outline>().effectColor = new Color32(67, 94, 50, 255);
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = new Vector2(100, 41);
                        button.text = "Upgrade";
                    }
                } else if (PlayerPrefs.GetInt(statKey) >= max)
                {
                    if (isUpgrade)
                    {
                        main.text = "Maxed out!";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    } else
                    {
                        main.text = "Owned";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    }
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = Vector2.zero;
                        button.text = "";
                    }
                }
            } else
            {
                if (PlayerPrefs.GetFloat(statKey) < max)
                {
                    if (PlayerPrefs.GetInt(priceKey) > 0)
                    {
                        main.text = "$" + PlayerPrefs.GetInt(priceKey);
                    } else
                    {
                        main.text = "Free";
                    }
                    main.color = new Color32(133, 187, 101, 255);
                    main.GetComponent<Outline>().effectColor = new Color32(67, 94, 50, 255);
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = new Vector2(100, 41);
                        button.text = "Upgrade";
                    }
                } else if (PlayerPrefs.GetFloat(statKey) >= max)
                {
                    if (isUpgrade)
                    {
                        main.text = "Maxed out!";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    } else
                    {
                        main.text = "Owned";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    }
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = Vector2.zero;
                        button.text = "";
                    }
                }
            }
        }
    }
}