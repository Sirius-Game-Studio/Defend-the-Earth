using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Spaceship
{
    [Tooltip("Spaceship data key.")] public string key;
    [Tooltip("Spaceship price.")] public int price;
    [Tooltip("Spaceship info panel color.")] public Color32 panelColor;
    [Tooltip("Spaceship preview object.")] public GameObject preview;
    [Tooltip("Spaceship name.")] public string name;
    [Tooltip("Spaceship health in info panel.")] public long health;
    [Tooltip("Spaceship damage in info panel.")] public string damage;
    [Tooltip("Spaceship fire rate in info panel.")] public float fireRate;
}

[System.Serializable]
public struct Upgrade
{
    [Tooltip("Upgrade name.")] public string name;
    [Tooltip("Maximum upgrade value.")] public int maxValue;
    [Tooltip("Upgrade price.")] public int price;
    [Tooltip("Upgrade price multiplier.")] public float priceMultiplier;
    [Tooltip("Amount added to the upgrade multiplier.")] public float multiplierIncrease;
    [Tooltip("Amount added to the upgrade percentage visuals.")] public int percentageIncrease;
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [SerializeField] private Spaceship[] spaceships = new Spaceship[0];
    [SerializeField] private Upgrade[] upgrades = new Upgrade[0];

    [Header("Spaceships Menu")]
    [SerializeField] private Text spaceshipName = null;
    [SerializeField] private Text spaceshipPrice = null;
    [SerializeField] private Text spaceshipBuyText = null;
    [SerializeField] private Text spaceshipInfo = null;
    [SerializeField] private Image spaceshipPanel = null;
    [SerializeField] private GameObject leftButton = null;
    [SerializeField] private GameObject rightButton = null;
    [SerializeField] private Text controllerBuyText = null;

    [Header("Upgrades Menu")]
    [SerializeField] private Text damagePercentage = null;
    [SerializeField] private Text speedPercentage = null;
    [SerializeField] private Text healthPercentage = null;
    [SerializeField] private Text moneyPercentage = null;
    [SerializeField] private Text damagePrice = null;
    [SerializeField] private Text speedPrice = null;
    [SerializeField] private Text healthPrice = null;
    [SerializeField] private Text moneyPrice = null;
    [SerializeField] private Text damageButton = null;
    [SerializeField] private Text speedButton = null;
    [SerializeField] private Text healthButton = null;
    [SerializeField] private Text moneyButton = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip purchaseItem = null;
    [SerializeField] private AudioClip cannotAfford = null;

    private AudioSource audioSource;
    private Controls input;
    [HideInInspector] public int page = 1;
    [HideInInspector] public bool open = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
        input = new Controls();
        page = 1;
        open = false;
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (!PlayerPrefs.HasKey(upgrades[i].name + "Multiplier")) PlayerPrefs.SetFloat(upgrades[i].name + "Multiplier", 1);
            if (!PlayerPrefs.HasKey(upgrades[i].name + "Price")) PlayerPrefs.SetInt(upgrades[i].name + "Price", upgrades[i].price);
        }
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        input.Enable();
        input.Menu.BuySpaceship.performed += context => buySpaceship(false);
        input.Menu.UpgradeDamage.performed += context => buyUpgrade(0);
        input.Menu.UpgradeSpeed.performed += context => buyUpgrade(1);
        input.Menu.UpgradeHealth.performed += context => buyUpgrade(2);
        input.Menu.UpgradeMoney.performed += context => buyUpgrade(3);
        input.Menu.SpaceshipsLeft.performed += context => changeSpaceshipForController(false);
        input.Menu.SpaceshipsRight.performed += context => changeSpaceshipForController(true);
    }

    void OnDisable()
    {
        input.Disable();
        input.Menu.BuySpaceship.performed -= context => buySpaceship(false);
        input.Menu.UpgradeDamage.performed -= context => buyUpgrade(0);
        input.Menu.UpgradeSpeed.performed -= context => buyUpgrade(1);
        input.Menu.UpgradeHealth.performed -= context => buyUpgrade(2);
        input.Menu.UpgradeMoney.performed -= context => buyUpgrade(3);
        input.Menu.SpaceshipsLeft.performed -= context => changeSpaceshipForController(false);
        input.Menu.SpaceshipsRight.performed -= context => changeSpaceshipForController(true);
    }

    void Update()
    {
        Spaceship spaceship = spaceships[page - 1];
        if (page < 1)
        {
            page = 1;
        } else if (page > spaceships.Length)
        {
            page = spaceships.Length;
        }
        spaceshipName.text = spaceship.name;
        if (PlayerPrefs.GetInt("Has" + spaceship.key) >= 1)
        {
            spaceshipBuyText.rectTransform.offsetMin = new Vector2(0, 0);
            spaceshipBuyText.rectTransform.offsetMax = new Vector2(0, 0);
            spaceshipBuyText.alignment = TextAnchor.MiddleCenter;
            if (PlayerPrefs.GetString("Spaceship") == spaceship.key)
            {
                spaceshipBuyText.text = "Using";
            } else
            {
                spaceshipBuyText.text = "Use";
            }
            spaceshipPrice.text = "";
            if (controllerBuyText) controllerBuyText.text = "Use";
        } else
        {
            spaceshipBuyText.rectTransform.offsetMin = new Vector2(10, 0);
            spaceshipBuyText.rectTransform.offsetMax = new Vector2(0, 0);
            spaceshipBuyText.alignment = TextAnchor.MiddleLeft;
            spaceshipBuyText.text = "Buy";
            if (spaceship.price > 0)
            {
                spaceshipPrice.text = "$" + spaceship.price;
            } else
            {
                spaceshipPrice.text = "Free";
            }
            if (controllerBuyText) controllerBuyText.text = "Buy";
        }
        spaceshipInfo.text = "Health: " + spaceship.health + "\nDamage: " + spaceship.damage + "\nFire Rate: " + spaceship.fireRate;
        spaceshipPanel.color = new Color32(spaceship.panelColor.r, spaceship.panelColor.g, spaceship.panelColor.b, 150);
        if (page <= 1)
        {
            leftButton.SetActive(false);
            rightButton.SetActive(true);
        } else if (page >= spaceships.Length)
        {
            leftButton.SetActive(true);
            rightButton.SetActive(false);
        } else
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);
        }
        foreach (GameObject shipPreview in GameObject.FindGameObjectsWithTag("Player")) shipPreview.SetActive(false);
        if (spaceship.preview) spaceship.preview.SetActive(true);
        
        //Updates upgrade percentage visuals
        damagePercentage.text = "+" + PlayerPrefs.GetInt("DamagePercentage") + "% Damage";
        speedPercentage.text = "+" + PlayerPrefs.GetInt("SpeedPercentage") + "% Speed";
        healthPercentage.text = "+" + PlayerPrefs.GetInt("HealthPercentage") + "% Health";
        moneyPercentage.text = "+" + PlayerPrefs.GetInt("MoneyPercentage") + "% Money";

        //Sets upgrade text states
        upgradeState(damagePrice, damageButton, "DamagePercentage", "DamagePrice", 50);
        upgradeState(speedPrice, speedButton, "SpeedPercentage", "SpeedPrice", 20);
        upgradeState(healthPrice, healthButton, "HealthPercentage", "HealthPrice", 100);
        upgradeState(moneyPrice, moneyButton, "MoneyPercentage", "MoneyPrice", 200);
    }

    #region Main Functions
    public void changeSpaceship(bool next)
    {
        if (open)
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
            if (next && page < spaceships.Length)
            {
                ++page;
            } else if (!next && page > 1)
            {
                --page;
            }
        }
    }

    void changeSpaceshipForController(bool next)
    {
        if (open)
        {
            if (next && page < spaceships.Length)
            {
                ++page;
            } else if (!next && page > 1)
            {
                --page;
            }
        }
    }

    void upgradeState(Text main, Text button, string statKey, string priceKey, int max)
    {
        if (statKey != "")
        {
            if (PlayerPrefs.GetInt(statKey) < max)
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
                button.rectTransform.sizeDelta = new Vector2(100, 41);
                button.text = "Upgrade";
            } else
            {
                main.text = "Maxed out!";
                main.color = new Color32(255, 215, 0, 255);
                main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                button.rectTransform.sizeDelta = Vector2.zero;
                button.text = "";
            }
        }
    }
    #endregion

    #region Purchase Functions
    public void buySpaceship(bool wasClicked)
    {
        if (open)
        {
            if (PlayerPrefs.GetInt("Has" + spaceships[page - 1].key) <= 0)
            {
                if (spaceships[page - 1].price > 0)
                {
                    long money = long.Parse(PlayerPrefs.GetString("Money"));
                    if (money >= spaceships[page - 1].price)
                    {
                        if (audioSource)
                        {
                            if (purchaseItem)
                            {
                                audioSource.PlayOneShot(purchaseItem);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                        money -= spaceships[page - 1].price;
                        PlayerPrefs.SetString("Money", money.ToString());
                        PlayerPrefs.SetInt("Has" + spaceships[page - 1].key, 1);
                        PlayerPrefs.SetString("Spaceship", spaceships[page - 1].key);
                    } else
                    {
                        if (audioSource)
                        {
                            if (cannotAfford)
                            {
                                audioSource.PlayOneShot(cannotAfford);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                    }
                } else
                {
                    PlayerPrefs.SetInt("Has" + spaceships[page - 1].key, 1);
                }
                PlayerPrefs.Save();
            } else
            {
                if (audioSource && wasClicked)
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick);
                    } else
                    {
                        audioSource.Play();
                    }
                }
                PlayerPrefs.SetString("Spaceship", spaceships[page - 1].key);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyUpgrade(int index)
    {
        if (open)
        {
            int maxValue = upgrades[index].maxValue;
            int c = (int)(PlayerPrefs.GetInt(upgrades[index].name + "Price") * upgrades[index].priceMultiplier);
            float m = PlayerPrefs.GetFloat(upgrades[index].name + "Multiplier") + upgrades[index].multiplierIncrease;
            int p = PlayerPrefs.GetInt(upgrades[index].name + "Percentage") + upgrades[index].percentageIncrease;
            if (PlayerPrefs.GetInt(upgrades[index].name + "Percentage") < maxValue)
            {
                if (upgrades[index].price > 0)
                {
                    long money = long.Parse(PlayerPrefs.GetString("Money"));
                    if (money >= PlayerPrefs.GetInt(upgrades[index].name + "Price"))
                    {
                        if (audioSource)
                        {
                            if (purchaseItem)
                            {
                                audioSource.PlayOneShot(purchaseItem);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                        money -= PlayerPrefs.GetInt(upgrades[index].name + "Price");
                        PlayerPrefs.SetString("Money", money.ToString());
                        PlayerPrefs.SetInt(upgrades[index].name + "Price", c);
                        PlayerPrefs.SetFloat(upgrades[index].name + "Multiplier", m);
                        PlayerPrefs.SetInt(upgrades[index].name + "Percentage", p);
                    } else
                    {
                        if (audioSource)
                        {
                            if (cannotAfford)
                            {
                                audioSource.PlayOneShot(cannotAfford);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                    }
                } else
                {
                    if (audioSource)
                    {
                        if (purchaseItem)
                        {
                            audioSource.PlayOneShot(purchaseItem);
                        } else
                        {
                            audioSource.Play();
                        }
                    }
                    PlayerPrefs.SetInt(upgrades[index].name + "Price", c);
                    PlayerPrefs.SetFloat(upgrades[index].name + "Multiplier", m);
                    PlayerPrefs.SetInt(upgrades[index].name + "Percentage", p);
                }
                PlayerPrefs.Save();
            }
        }
    }
    #endregion
}