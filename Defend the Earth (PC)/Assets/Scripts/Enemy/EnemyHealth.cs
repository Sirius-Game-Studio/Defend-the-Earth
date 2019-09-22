using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public long health = 20;
    [Tooltip("Values below 1 reduce damage and values above 1 increase damage.")] public float defense = 1;
    [Tooltip("Amount of money earned from killing this enemy (Campaign only).")] [SerializeField] private long money = 1;
    [Tooltip("Amount of score earned from killing this enemy (Endless only).")] [SerializeField] private long score = 5;
    [Range(0, 1)] [SerializeField] private float powerupChance = 0.37f;
    [SerializeField] private GameObject[] powerups = new GameObject[0];
    [SerializeField] private bool countsTowardsKillGoal = true;
    [Tooltip("The kill tracker to update (leave blank to not update).")] [SerializeField] private string killTracker = "";

    [Header("Miscellaneous")]
    public long maxHealth = 0;
    public bool invulnerable = false;

    [Header("Setup")]
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private GameObject textPopup = null;

    private long mh = 0;

    void Start()
    {
        if (GameController.instance.isCampaignLevel)
        {
            if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
            {
                if (gameObject.layer != 9) //If this enemy isn't a boss
                {
                    health = (long)(health * 0.85);
                    powerupChance += 0.03f;
                }
                if (money > 0)
                {
                    money = (long)(money * 0.5f);
                    if (money <= 0) money = 1;
                }
            } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
            {
                if (gameObject.layer != 9) //If this enemy isn't a boss
                {
                    health = (long)(health * 1.1);
                    defense -= 0.03f;
                    powerupChance -= 0.02f;
                } else //If this enemy is a boss
                {
                    health = (long)(health * 1.5);
                    defense -= 0.06f;
                }
            } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
            {
                if (gameObject.layer != 9) //If this enemy isn't a boss
                {
                    health = (long)(health * 1.2);
                    defense -= 0.06f;
                    powerupChance -= 0.04f;
                } else //If this enemy is a boss
                {
                    health *= 2;
                    defense -= 0.12f;
                }
            }
        } else
        {
            if (GameController.instance.wavesCleared > 0)
            {
                float multiplier = 1;
                for (long i = 0; i < GameController.instance.wavesCleared; i++) multiplier += 0.04f;
                if (multiplier > 1.4f) multiplier = 1.4f;
                health = (long)(health * multiplier);
            }
            money = 0;
        }
        maxHealth = health;
        mh = maxHealth;
        if (PlayerPrefs.HasKey("MoneyMultiplier")) money = (long)(money * PlayerPrefs.GetFloat("MoneyMultiplier"));
    }

    void Update()
    {
        if (maxHealth != mh) maxHealth = mh;
        if (health > maxHealth)
        {
            health = maxHealth;
        } else if (health <= 0)
        {
            if (killTracker != "")
            {
                if (!PlayerPrefs.HasKey(killTracker))
                {
                    PlayerPrefs.SetString(killTracker, "1");
                } else
                {
                    long newKill = long.Parse(PlayerPrefs.GetString(killTracker));
                    ++newKill;
                    PlayerPrefs.SetString(killTracker, newKill.ToString());
                }
                PlayerPrefs.Save();
            }
            if (explosion) Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            if (powerups.Length > 0)
            {
                float random = Random.value;
                if (random <= powerupChance) Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, Quaternion.Euler(0, 0, 0));
            }
            if (GameController.instance.isCampaignLevel && money > 0)
            {
                if (PlayerPrefs.GetString("Money") != "")
                {
                    long cash = long.Parse(PlayerPrefs.GetString("Money"));
                    cash += money;
                    PlayerPrefs.SetString("Money", cash.ToString());
                } else
                {
                    PlayerPrefs.SetString("Money", money.ToString());
                }
                if (textPopup)
                {
                    if (textPopup.GetComponent<TextMeshPro>())
                    {
                        GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                        popup.GetComponent<TextMeshPro>().text = "$" + money;
                        popup.GetComponent<TextMeshPro>().color = new Color32(255, 215, 0, 255);
                        popup.GetComponent<TextMeshPro>().outlineColor = new Color32(127, 107, 0, 255);
                    } else
                    {
                        Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                    }
                }
            }
            if (!GameController.instance.isCampaignLevel && !GameController.instance.gameOver && score > 0)
            {
                GameController.instance.addScore(score);
                if (textPopup)
                {
                    if (textPopup.GetComponent<TextMeshPro>())
                    {
                        GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                        popup.GetComponent<TextMeshPro>().text = "+" + score;
                        popup.GetComponent<TextMeshPro>().color = new Color32(0, 170, 255, 255);
                        popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 85, 127, 255);
                    } else
                    {
                        Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                    }
                }
            }
            if (countsTowardsKillGoal && GameController.instance.enemiesLeft > 0)
            {
                if (!GameController.instance.currentBoss && gameObject.layer != 9)
                {
                    --GameController.instance.enemiesLeft;
                } else if (GameController.instance.currentBoss && gameObject.layer == 9)
                {
                    GameController.instance.enemiesLeft = 0;
                }
            }
            Destroy(gameObject);
        }
        if (score < 0) score = 0; //Checks if score is less than 0
    }

    public void takeDamage(long damage)
    {
        if (!invulnerable)
        {
            if (damage > 0)
            {
                health -= damage;
            } else
            {
                --health;
            }
        }
    }
}