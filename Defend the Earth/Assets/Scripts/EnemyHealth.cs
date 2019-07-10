using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Amount of health this enemy starts with.")] public long health = 0;
    [Tooltip("Amount of score this enemy gives to the player upon death.")] [SerializeField] private int score = 0;
    [Tooltip("Amount of money earned from killing this enemy.")] [SerializeField] private long money = 0;
    [Range(0, 1)] [SerializeField] private float powerupChance = 1;
    [SerializeField] private GameObject[] powerups;
    [Tooltip("The kills key to set (leave blank to not count towards kills).")] [SerializeField] private string killsKey = "";

    [Header("Setup")]
    [SerializeField] private GameObject explosion;

    private GameController gameController;

    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()   
    {
        if (health <= 0)
        {
            gameController.addScore(score);
            if (explosion) Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            if (powerups.Length > 0)
            {
                float random = Random.value;
                if (random <= powerupChance) Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, Quaternion.Euler(0, -90, 0)); print("Allahu");
            }
            if (money > 0)
            {
                long cash = long.Parse(PlayerPrefs.GetString("Money"));
                cash += money;
                PlayerPrefs.SetString("Money", cash.ToString());
            }
            /*
            if (killsKey != "")
            {
                int kill = PlayerPrefs.GetInt(killsKey);
                if (kill <= 0)
                {
                    kill = 1;
                } else
                {
                    ++kill;
                }
                PlayerPrefs.SetInt(killsKey, kill);
                PlayerPrefs.Save();
            }
            */
            gameObject.SetActive(false); //Makes the enemy inactive, also ensuring it doesn't shoot out of nowhere
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