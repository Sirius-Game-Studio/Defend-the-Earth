using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public long health = 1;
    [Tooltip("Values below 1 reduce damage and values above 1 increase damage.")] public float defense = 1;
    [Tooltip("Amount of money earned from killing this enemy.")] [SerializeField] private long money = 0;
    [Range(0, 1)] [SerializeField] private float powerupChance = 1;
    [SerializeField] private GameObject[] powerups = new GameObject[0];
    [SerializeField] private bool countsTowardsKillGoal = true;
    [Tooltip("The kill tracker to update (leave blank to not update).")] [SerializeField] private string killTracker = "";

    [Header("Setup")]
    [SerializeField] private GameObject explosion = null;

    void Start()
    {
        if (PlayerPrefs.GetInt("Difficulty") <= 1)
        {
            health = (long)(health * 0.75);
        }
        else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            if (gameObject.layer != 9)
            {
                health = (long)(health * 1.1);
            } else
            {
                health = (long)(health * 1.25);
                defense -= 0.05f;
            }
        }
        else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            if (gameObject.layer != 9)
            {
                health = (long)(health * 1.2);
            } else
            {
                health = (long)(health * 1.5);
                defense -= 0.1f;
            }
        }
        if (PlayerPrefs.HasKey("MoneyMultiplier")) money = (long)(money * PlayerPrefs.GetFloat("MoneyMultiplier"));
    }

    void Update()   
    {
        if (health <= 0)
        {
            if (killTracker != "")
            {
                if (!PlayerPrefs.HasKey(killTracker))
                {
                    PlayerPrefs.SetString(killTracker, "1");
                } else
                {
                    long plus = long.Parse(PlayerPrefs.GetString(killTracker));
                    ++plus;
                    PlayerPrefs.SetString(killTracker, plus.ToString());
                }
                PlayerPrefs.Save();
            }
            if (explosion) Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            if (powerups.Length > 0)
            {
                float random = Random.value;
                if (random <= powerupChance) Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, Quaternion.Euler(0, -90, 0));
            }
            if (money > 0)
            {
                long cash = long.Parse(PlayerPrefs.GetString("Money"));
                cash += money;
                PlayerPrefs.SetString("Money", cash.ToString());
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
    }

    public void takeDamage(long damage)
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