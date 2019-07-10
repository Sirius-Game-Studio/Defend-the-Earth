using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Spaceships Menu")]
    //Pages
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;

    //Buy Buttons
    [SerializeField] private Text buySpaceFighterButton;
    [SerializeField] private Text buyAlienMowerButton;
    [SerializeField] private Text buyBlazingRocketButton;
    [SerializeField] private Text buyHeavyCannonButton;

    //Prices
    [SerializeField] private Text spaceFighterPrice;
    [SerializeField] private Text alienMowerPrice;
    [SerializeField] private Text blazingRocketPrice;
    [SerializeField] private Text heavyCannonPrice;

    [Header("Upgrades Menu")]
    [SerializeField] private Text moneyText;
    [SerializeField] private Text damageText;
    [SerializeField] private Text fireRateText;
    [SerializeField] private Text upgradeDamageButton;
    [SerializeField] private Text upgradeFireRateButton;
    [SerializeField] private Text upgradeSpeedButton;
    [SerializeField] private Text upgradeHealthButton;
    [SerializeField] private Text speedText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text damagePrice;
    [SerializeField] private Text fireRatePrice;
    [SerializeField] private Text speedPrice;
    [SerializeField] private Text healthPrice;

    [Header("Settings Menu")]
    [SerializeField] private Text fullscreenText;

    [Header("Sound Menu")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas shopUI;
    [SerializeField] private Canvas upgradesUI;
    [SerializeField] private Canvas spaceshipsUI;
    [SerializeField] private Canvas settingsUI;
    [SerializeField] private Canvas graphicsQualityUI;
    [SerializeField] private Canvas soundUI;
    [SerializeField] private Text loadingText;

    private int page = 1;
    private bool loading = false;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
            soundSlider.value = 1;
        } else
        {
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
            musicSlider.value = 1;
        } else
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        mainMenu.enabled = true;
        shopUI.enabled = false;
        spaceshipsUI.enabled = false;
        upgradesUI.enabled = false;
        settingsUI.enabled = false;
        graphicsQualityUI.enabled = false;
        soundUI.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        moneyText.text = "$" + PlayerPrefs.GetString("Money");
        damageText.text = "+" + PlayerPrefs.GetInt("DamagePercentage") + "% Damage";
        fireRateText.text = "+" + PlayerPrefs.GetInt("FireRatePercentage") + "% Fire Rate";
        speedText.text = "+" + PlayerPrefs.GetInt("SpeedPercentage") + "% Speed";
        healthText.text = "+" + PlayerPrefs.GetInt("HealthPercentage") + "% Health";

        //Sets the states of spaceship buy buttons
        spaceshipButtonState(buySpaceFighterButton, "SpaceFighter");
        spaceshipButtonState(buyAlienMowerButton, "AlienMower");
        spaceshipButtonState(buyBlazingRocketButton, "BlazingRocket");
        spaceshipButtonState(buyHeavyCannonButton, "HeavyCannon");

        //Sets the states of spaceship price text
        priceTextState(spaceFighterPrice, buySpaceFighterButton, false, false, false, "HasSpaceFighter", "", 0, 1, false);
        priceTextState(alienMowerPrice, buyAlienMowerButton, false, false, false, "HasAlienMower", "", 600, 1, false);
        priceTextState(blazingRocketPrice, buyBlazingRocketButton, false, false, false, "HasBlazingRocket", "", 1300, 1, false);
        priceTextState(heavyCannonPrice, buyHeavyCannonButton, false, false, false, "HasHeavyCannon", "", 3500, 1, false);

        //Sets the states of upgrade price text
        priceTextState(damagePrice, upgradeDamageButton, true, true, true, "DamagePercentage", "DamagePrice", 7, 100, false);
        priceTextState(fireRatePrice, upgradeFireRateButton, true, true, true, "FireRatePercentage", "FireRatePrice", 5, 50, false);
        priceTextState(speedPrice, upgradeSpeedButton, true, true, true, "SpeedPercentage", "SpeedPrice", 4, 50, false);
        priceTextState(healthPrice, upgradeHealthButton, true, true, true, "HealthPercentage", "HealthPrice", 6, 200, false);
        if (!Screen.fullScreen)
        {
            fullscreenText.text = "Change to Fullscreen";
            fullscreenText.rectTransform.sizeDelta = new Vector2(260, 41);
        } else
        {
            fullscreenText.text = "Change to Windowed Mode";
            fullscreenText.rectTransform.sizeDelta = new Vector2(330, 41);
        }
        if (!loading)
        {
            loadingText.enabled = false;
        } else
        {
            loadingText.enabled = true;
        }
        if (PlayerPrefs.GetFloat("DamageMultiplier") > 2)
        {
            PlayerPrefs.SetFloat("DamageMultiplier", 2);
            PlayerPrefs.SetInt("DamagePercentage", 100);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("FireRateMultiplier") < 0.25f)
        {
            PlayerPrefs.SetFloat("FireRateMultiplier", 0.25f);
            PlayerPrefs.SetInt("FireRatePercentage", 50);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("SpeedMultiplier") > 1.5f)
        {
            PlayerPrefs.SetFloat("SpeedMultiplier", 1.5f);
            PlayerPrefs.SetInt("SpeedPercentage", 50);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("HealthMultiplier") > 3)
        {
            PlayerPrefs.SetFloat("HealthMultiplier", 3);
            PlayerPrefs.SetInt("HealthPercentage", 200);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetInt("Money") < 0)
        {
            PlayerPrefs.SetInt("Money", 0);
            PlayerPrefs.Save();
        }
    }

    public void clickPlayGame()
    {
        if (!loading)
        {
            loading = true;
            mainMenu.enabled = false;
            shopUI.enabled = false;
            spaceshipsUI.enabled = false;
            upgradesUI.enabled = false;
            settingsUI.enabled = false;
            graphicsQualityUI.enabled = false;
            soundUI.enabled = false;
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            StartCoroutine(loadScene("SampleScene"));
        }
    }

    public void clickShop()
    {
        if (!shopUI.enabled)
        {
            shopUI.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            shopUI.enabled = false;
            mainMenu.enabled = true;
        }
    }


    public void clickSettings()
    {
        if (!settingsUI.enabled)
        {
            settingsUI.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            settingsUI.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickQuitGame()
    {
        Application.Quit();
    }

    public void clickSpaceships()
    {
        if (!spaceshipsUI.enabled)
        {
            spaceshipsUI.enabled = true;
            shopUI.enabled = false;
        }
        else
        {
            spaceshipsUI.enabled = false;
            shopUI.enabled = true;
            page1.SetActive(true);
            page2.SetActive(false);
            page = 1;
        }
    }

    public void clickUpgrades()
    {
        if (!upgradesUI.enabled)
        {
            upgradesUI.enabled = true;
            shopUI.enabled = false;
        }
        else
        {
            upgradesUI.enabled = false;
            shopUI.enabled = true;
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
            if (money >= 600)
            {
                money -= 600;
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
            if (money >= 1400)
            {
                money -= 1400;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasBlazingRocket", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyHeavyCannon()
    {
        if (PlayerPrefs.GetInt("HasHeavyCannon") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 3500)
            {
                money -= 3500;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasHeavyCannon", 1);
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
        if (PlayerPrefs.GetInt("DamagePercentage") < 100)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("DamagePrice"))
            {
                money -= PlayerPrefs.GetInt("DamagePrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("DamagePrice", (int)(PlayerPrefs.GetInt("DamagePrice") * 1.3f));
                PlayerPrefs.SetFloat("DamageMultiplier", PlayerPrefs.GetFloat("DamageMultiplier") + 0.05f);
                PlayerPrefs.SetInt("DamagePercentage", PlayerPrefs.GetInt("DamagePercentage") + 5);
                PlayerPrefs.Save();
                print(5 * PlayerPrefs.GetFloat("DamageMultiplier"));
            }
        }
    }

    public void upgradeFireRate()
    {
        if (PlayerPrefs.GetInt("FireRatePercentage") < 50)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("FireRatePrice"))
            {
                money -= PlayerPrefs.GetInt("FireRatePrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("FireRatePrice", (int)(PlayerPrefs.GetInt("FireRatePrice") * 1.5f));
                PlayerPrefs.SetFloat("FireRateMultiplier", PlayerPrefs.GetFloat("FireRateMultiplier") - 0.05f);
                PlayerPrefs.SetInt("FireRatePercentage", PlayerPrefs.GetInt("FireRatePercentage") + 5);
                PlayerPrefs.Save();
                print(0.5f * PlayerPrefs.GetFloat("FireRateMultiplier"));
            }
        }
    }

    public void upgradeSpeed()
    {
        if (PlayerPrefs.GetInt("SpeedPercentage") < 50)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("SpeedPrice"))
            {
                money -= PlayerPrefs.GetInt("SpeedPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("SpeedPrice", (int)(PlayerPrefs.GetInt("SpeedPrice") * 1.6f));
                PlayerPrefs.SetFloat("SpeedMultiplier", PlayerPrefs.GetFloat("SpeedMultiplier") + 0.05f);
                PlayerPrefs.SetInt("SpeedPercentage", PlayerPrefs.GetInt("SpeedPercentage") + 5);
                PlayerPrefs.Save();
                print(6.5f * PlayerPrefs.GetFloat("SpeedMultiplier"));
            }
        }
    }

    public void upgradeHealth()
    {
        if (PlayerPrefs.GetInt("HealthPercentage") < 200)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("HealthPrice"))
            {
                money -= PlayerPrefs.GetInt("HealthPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HealthPrice", (int)(PlayerPrefs.GetInt("HealthPrice") * 1.35f));
                PlayerPrefs.SetFloat("HealthMultiplier", PlayerPrefs.GetFloat("HealthMultiplier") + 0.1f);
                PlayerPrefs.SetInt("HealthPercentage", PlayerPrefs.GetInt("HealthPercentage") + 10);
                PlayerPrefs.Save();
                print(30 * PlayerPrefs.GetFloat("HealthMultiplier"));
            }
        }
    }

    public void clickGraphicsQuality()
    {
        if (!graphicsQualityUI.enabled)
        {
            graphicsQualityUI.enabled = true;
            settingsUI.enabled = false;
        } else
        {
            graphicsQualityUI.enabled = false;
            settingsUI.enabled = true;
        }
    }

    public void clickSound()
    {
        if (!soundUI.enabled)
        {
            soundUI.enabled = true;
            settingsUI.enabled = false;
        } else
        {
            soundUI.enabled = false;
            settingsUI.enabled = true;
        }
    }

    public void changeFullscreen()
    {
        if (settingsUI.enabled) Screen.fullScreen = !Screen.fullScreen;
    }

    public void changeQualityLevel(int qualityLevel)
    {
        if (graphicsQualityUI.enabled && qualityLevel >= 0) QualitySettings.SetQualityLevel(qualityLevel, true);
    }

    IEnumerator loadScene(string scene)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(scene);
        while (!load.isDone)
        {
            loadingText.text = "Loading: " + Mathf.Floor(load.progress * 100) + "%";
            yield return null;
        }
        loading = false;
        mainMenu.enabled = true;
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