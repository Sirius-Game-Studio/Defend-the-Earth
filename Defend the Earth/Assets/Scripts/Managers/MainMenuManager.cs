using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Spaceships Menu")]
    //Pages
    [SerializeField] private GameObject page1 = null;
    [SerializeField] private GameObject page2 = null;

    //Buy Buttons
    [SerializeField] private Text buySpaceFighterButton = null;
    [SerializeField] private Text buyAlienMowerButton = null;
    [SerializeField] private Text buyBlazingRocketButton = null;
    [SerializeField] private Text buyQuadShooterButton = null;
    [SerializeField] private Text buyPointVoidBreakerButton = null;
    [SerializeField] private Text buyAnnihilatorButton = null;

    //Price Text
    [SerializeField] private Text spaceFighterPrice = null;
    [SerializeField] private Text alienMowerPrice = null;
    [SerializeField] private Text blazingRocketPrice = null;
    [SerializeField] private Text quadShooterPrice = null;
    [SerializeField] private Text pointVoidBreakerPrice = null;
    [SerializeField] private Text annihilatorPrice = null;

    [Header("Upgrades Menu")]
    [SerializeField] private Text moneyText = null;
    [SerializeField] private Text damageText = null;
    [SerializeField] private Text fireRateText = null;
    [SerializeField] private Text upgradeDamageButton = null;
    [SerializeField] private Text upgradeSpeedButton = null;
    [SerializeField] private Text upgradeHealthButton = null;
    [SerializeField] private Text upgradeMoneyButton = null;
    [SerializeField] private Text speedText = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private Text damagePrice = null;
    [SerializeField] private Text speedPrice = null;
    [SerializeField] private Text healthPrice = null;
    [SerializeField] private Text moneyPrice = null;
    
    [Header("Sound Menu")]
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private Slider musicSlider = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas spaceshipsMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas graphicsQualityMenu = null;
    [SerializeField] private Canvas soundMenu = null;
    [SerializeField] private Canvas selectDifficultyMenu = null;
    [SerializeField] private GameObject loadingText = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;

    private int page = 1;
    private bool loading = false;

    void Awake()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        PlayerPrefs.DeleteKey("Difficulty");
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            soundSlider.value = 1;
        } else
        {
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            musicSlider.value = 1;
        } else
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        PlayerPrefs.Save();
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        mainMenu.enabled = true;
        shopMenu.enabled = false;
        spaceshipsMenu.enabled = false;
        upgradesMenu.enabled = false;
        settingsMenu.enabled = false;
        graphicsQualityMenu.enabled = false;
        soundMenu.enabled = false;
        selectDifficultyMenu.enabled = false;
        page1.SetActive(true);
        page2.SetActive(false);
        page = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopMenu.enabled)
            {
                clickShop();
            } else if (spaceshipsMenu.enabled)
            {
                clickSpaceships();
            } else if (upgradesMenu.enabled)
            {
                clickUpgrades();
            } else if (settingsMenu.enabled)
            {
                clickSettings();
            } else if (graphicsQualityMenu.enabled)
            {
                clickGraphicsQuality();
            } else if (soundMenu.enabled)
            {
                clickSound();
            } else if (selectDifficultyMenu.enabled)
            {
                clickPlayGame();
            }
        }
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");

        //Updates volume data to match the slider values
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();

        //Updates the money counter and upgrade price text
        moneyText.text = "$" + PlayerPrefs.GetString("Money");
        damageText.text = "+" + PlayerPrefs.GetInt("DamagePercentage") + "% Damage";
        speedText.text = "+" + PlayerPrefs.GetInt("SpeedPercentage") + "% Speed";
        healthText.text = "+" + PlayerPrefs.GetInt("HealthPercentage") + "% Health";
        fireRateText.text = "+" + PlayerPrefs.GetInt("MoneyPercentage") + "% Money";

        //Sets the states of spaceship buy buttons
        spaceshipButtonState(buySpaceFighterButton, "SpaceFighter");
        spaceshipButtonState(buyAlienMowerButton, "AlienMower");
        spaceshipButtonState(buyBlazingRocketButton, "BlazingRocket");
        spaceshipButtonState(buyQuadShooterButton, "QuadShooter");
        spaceshipButtonState(buyPointVoidBreakerButton, "PointVoidBreaker");
        spaceshipButtonState(buyAnnihilatorButton, "Annihilator");

        //Sets the states of spaceship price text
        priceTextState(spaceFighterPrice, buySpaceFighterButton, false, false, false, "HasSpaceFighter", "", 0, 1, false);
        priceTextState(alienMowerPrice, buyAlienMowerButton, false, false, false, "HasAlienMower", "", 300, 1, false);
        priceTextState(blazingRocketPrice, buyBlazingRocketButton, false, false, false, "HasBlazingRocket", "", 950, 1, false);
        priceTextState(quadShooterPrice, buyQuadShooterButton, false, false, false, "HasQuadShooter", "", 1500, 1, false);
        priceTextState(pointVoidBreakerPrice, buyPointVoidBreakerButton, false, false, false, "HasPointVoidBreaker", "", 2300, 1, false);
        priceTextState(annihilatorPrice, buyAnnihilatorButton, false, false, false, "HasAnnihilator", "", 5000, 1, false);

        //Sets the states of upgrade price text
        priceTextState(damagePrice, upgradeDamageButton, true, true, true, "DamagePercentage", "DamagePrice", 8, 75, false);
        priceTextState(speedPrice, upgradeSpeedButton, true, true, true, "SpeedPercentage", "SpeedPrice", 5, 30, false);
        priceTextState(healthPrice, upgradeHealthButton, true, true, true, "HealthPercentage", "HealthPrice", 7, 150, false);
        priceTextState(moneyPrice, upgradeMoneyButton, true, true, true, "MoneyPercentage", "MoneyPrice", 3, 300, false);

        if (!loading)
        {
            loadingText.SetActive(false);
            moneyText.gameObject.SetActive(true);
        } else
        {
            loadingText.SetActive(true);
            moneyText.gameObject.SetActive(false);
        }

        //Checks if the player upgrades are above maximum values
        if (PlayerPrefs.GetFloat("DamageMultiplier") > 1.75f)
        {
            PlayerPrefs.SetFloat("DamageMultiplier", 1.75f);
            PlayerPrefs.SetInt("DamagePercentage", 75);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("SpeedMultiplier") > 1.3f)
        {
            PlayerPrefs.SetFloat("SpeedMultiplier", 1.3f);
            PlayerPrefs.SetInt("SpeedPercentage", 30);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("HealthMultiplier") > 2.5f)
        {
            PlayerPrefs.SetFloat("HealthMultiplier", 2.5f);
            PlayerPrefs.SetInt("HealthPercentage", 150);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("MoneyMultiplier") > 4)
        {
            PlayerPrefs.SetFloat("MoneyMultiplier", 4);
            PlayerPrefs.SetInt("MoneyPercentage", 300);
            PlayerPrefs.Save();
        }

        //Checks if money is below 0
        if (PlayerPrefs.GetInt("Money") < 0)
        {
            PlayerPrefs.SetInt("Money", 0);
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
    }

    public void clickPlayGame()
    {
        if (!selectDifficultyMenu.enabled)
        {
            selectDifficultyMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            selectDifficultyMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickShop()
    {
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
        Application.Quit();
    }

    public void clickSpaceships()
    {
        if (!spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = true;
            shopMenu.enabled = false;
        } else
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            page1.SetActive(true);
            page2.SetActive(false);
            page = 1;
        }
    }

    public void clickUpgrades()
    {
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

    public void startGame(int difficulty)
    {
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
            PlayerPrefs.Save();
            StartCoroutine(loadScene("Level " + PlayerPrefs.GetInt("Level")));
        } else
        {
            PlayerPrefs.SetInt("Difficulty", difficulty);
            PlayerPrefs.Save();
            StartCoroutine(loadScene("Level 1"));
        }
    }

    public void changeSpaceshipsPage()
    {
        if (page <= 1)
        {
            page1.SetActive(false);
            page2.SetActive(true);
            page = 2;
        } else if (page >= 2)
        {
            page1.SetActive(true);
            page2.SetActive(false);
            page = 1;
        }
    }

    public void buySpaceFighter()
    {
        if (PlayerPrefs.GetInt("HasSpaceFighter") <= 0)
        {
            PlayerPrefs.SetInt("HasSpaceFighter", 1);
            PlayerPrefs.Save();
        }
    }

    public void buyAlienMower()
    {
        if (PlayerPrefs.GetInt("HasAlienMower") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 300)
            {
                money -= 300;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasAlienMower", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyBlazingRocket()
    {
        if (PlayerPrefs.GetInt("HasBlazingRocket") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 950)
            {
                money -= 950;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasBlazingRocket", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyQuadShooter()
    {
        if (PlayerPrefs.GetInt("HasQuadShooter") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 1500)
            {
                money -= 1500;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasQuadShooter", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyPointVoidBreaker()
    {
        if (PlayerPrefs.GetInt("HasPointVoidBreaker") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 2300)
            {
                money -= 2300;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasPointVoidBreaker", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyAnnihilator()
    {
        if (PlayerPrefs.GetInt("HasAnnihilator") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 5000)
            {
                money -= 5000;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasAnnihilator", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void equipSpaceship(string spaceship)
    {
        if (spaceship != "")
        {
            if (PlayerPrefs.HasKey("Has" + spaceship))
            {
                if (PlayerPrefs.GetInt("Has" + spaceship) >= 1 && PlayerPrefs.GetString("Spaceship") != spaceship)
                {
                    PlayerPrefs.SetString("Spaceship", spaceship);
                }
            } else
            {
                if (PlayerPrefs.GetString("Spaceship") != "SpaceFighter") PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            }
            PlayerPrefs.Save();
       }
    }
     
    public void upgradeDamage()
    {
        if (PlayerPrefs.GetInt("DamagePercentage") < 75)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("DamagePrice"))
            {
                money -= PlayerPrefs.GetInt("DamagePrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("DamagePrice", (int)(PlayerPrefs.GetInt("DamagePrice") * 1.45f));
                PlayerPrefs.SetFloat("DamageMultiplier", PlayerPrefs.GetFloat("DamageMultiplier") + 0.05f);
                PlayerPrefs.SetInt("DamagePercentage", PlayerPrefs.GetInt("DamagePercentage") + 5);
                PlayerPrefs.Save();
            }
        }
    }

    public void upgradeSpeed()
    {
        if (PlayerPrefs.GetInt("SpeedPercentage") < 30)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("SpeedPrice"))
            {
                money -= PlayerPrefs.GetInt("SpeedPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("SpeedPrice", (int)(PlayerPrefs.GetInt("SpeedPrice") * 1.4f));
                PlayerPrefs.SetFloat("SpeedMultiplier", PlayerPrefs.GetFloat("SpeedMultiplier") + 0.02f);
                PlayerPrefs.SetInt("SpeedPercentage", PlayerPrefs.GetInt("SpeedPercentage") + 2);
                PlayerPrefs.Save();
            }
        }
    }

    public void upgradeHealth()
    {
        if (PlayerPrefs.GetInt("HealthPercentage") < 150)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("HealthPrice"))
            {
                money -= PlayerPrefs.GetInt("HealthPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HealthPrice", (int)(PlayerPrefs.GetInt("HealthPrice") * 1.5f));
                PlayerPrefs.SetFloat("HealthMultiplier", PlayerPrefs.GetFloat("HealthMultiplier") + 0.1f);
                PlayerPrefs.SetInt("HealthPercentage", PlayerPrefs.GetInt("HealthPercentage") + 10);
                PlayerPrefs.Save();
            }
        }
    }

    public void upgradeMoney()
    {
        if (PlayerPrefs.GetInt("MoneyPercentage") < 300)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("MoneyPrice"))
            {
                money -= PlayerPrefs.GetInt("MoneyPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("MoneyPrice", (int)(PlayerPrefs.GetInt("MoneyPrice") * 1.35f));
                PlayerPrefs.SetFloat("MoneyMultiplier", PlayerPrefs.GetFloat("MoneyMultiplier") + 0.1f);
                PlayerPrefs.SetInt("MoneyPercentage", PlayerPrefs.GetInt("MoneyPercentage") + 10);
                PlayerPrefs.Save();
            }
        }
    }

    public void clickGraphicsQuality()
    {
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

    public void changeQualityLevel(int qualityLevel)
    {
        if (graphicsQualityMenu.enabled && qualityLevel >= 0) QualitySettings.SetQualityLevel(qualityLevel, true);
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
                mainMenu.enabled = false;
                shopMenu.enabled = false;
                spaceshipsMenu.enabled = false;
                upgradesMenu.enabled = false;
                settingsMenu.enabled = false;
                graphicsQualityMenu.enabled = false;
                soundMenu.enabled = false;
                selectDifficultyMenu.enabled = false;
                yield return null;
            }
            loading = false;
            loadingSlider.value = 0;
            loadingPercentage.text = "0%";
        } else
        {
            StopCoroutine("loadScene");
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
                }
                else if (PlayerPrefs.GetFloat(statKey) >= max)
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