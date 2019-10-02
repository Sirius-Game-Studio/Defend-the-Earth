using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip cannotAfford = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas spaceshipsMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas selectGamemodeMenu = null;
    [SerializeField] private Canvas selectDifficultyMenu = null;
    [SerializeField] private Canvas buyMoneyMenu = null;
    [SerializeField] private GameObject openBuyMoneyButton = null;
    [SerializeField] private Text currentLevelText = null;
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text purchaseNotification = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [SerializeField] private GameObject anyKeyPrompt = null;
    [SerializeField] private Text loadingTip = null;
    [SerializeField] private AudioMixer audioMixer = null;

    private AudioSource audioSource;
    private string currentLoadingTip = "";
    private bool loading = false;

    void Awake()
    {
        Application.targetFrameRate = 60;
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
        loading = false;
        currentLoadingTip = "";
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
        buyMoneyMenu.enabled = false;
        if (purchaseNotification) purchaseNotification.text = "";
        currentLoadingTip = "";
        ShopManager.instance.page = 1;
        ShopManager.instance.open = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!buyMoneyMenu.enabled)
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
                    openBuyMoneyButton.SetActive(true);
                } else if (upgradesMenu.enabled)
                {
                    upgradesMenu.enabled = false;
                    shopMenu.enabled = true;
                    openBuyMoneyButton.SetActive(true);
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
            } else
            {
                buyMoneyMenu.enabled = false;
                openBuyMoneyButton.SetActive(true);
            }
        }

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
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
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
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
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
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
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
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
        }
        if (!spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = true;
            shopMenu.enabled = false;
            ShopManager.instance.open = true;
            openBuyMoneyButton.SetActive(false);
        } else
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            ShopManager.instance.page = 1;
            ShopManager.instance.open = false;
            openBuyMoneyButton.SetActive(true);
        }
    }

    public void clickUpgrades()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
        }
        if (!upgradesMenu.enabled)
        {
            upgradesMenu.enabled = true;
            shopMenu.enabled = false;
            ShopManager.instance.open = true;
            openBuyMoneyButton.SetActive(false);
        } else
        {
            upgradesMenu.enabled = false;
            shopMenu.enabled = true;
            ShopManager.instance.open = false;
            openBuyMoneyButton.SetActive(true);
        }
    }

    public void openBuyMoneyMenu()
    {
        if (buttonClick)
        {
            audioSource.PlayOneShot(buttonClick);
        } else
        {
            audioSource.Play();
        }
        if (!buyMoneyMenu.enabled)
        {
            buyMoneyMenu.enabled = true;
            openBuyMoneyButton.SetActive(false);
        } else
        {
            buyMoneyMenu.enabled = false;
            openBuyMoneyButton.SetActive(true);
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
    #endregion

    #region IAP Functions
    public void grantMoney(int amount)
    {
        if (PlayerPrefs.GetString("Money") != "")
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            money += amount;
            PlayerPrefs.SetString("Money", money.ToString());
        } else
        {
            PlayerPrefs.SetString("Money", amount.ToString());
        }
        PlayerPrefs.Save();
    }

    public void onPurchaseFailure()
    {
        if (audioSource && cannotAfford) audioSource.PlayOneShot(cannotAfford);
        if (purchaseNotification)
        {
            CancelInvoke("resetPurchaseNotification");
            Invoke("resetPurchaseNotification", 1);
        }
    }

    public void resetPurchaseNotification()
    {
        if (purchaseNotification) purchaseNotification.text = "";
    }
    #endregion

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