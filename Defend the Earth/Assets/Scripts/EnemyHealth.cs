using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Amount of health this enemy starts with.")] public long health = 0;
    [Tooltip("Amount of score this enemy gives to the player upon death.")] [SerializeField] private int score = 0;
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
            if (explosion)
            {
                GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
                if (newExplosion.GetComponent<AudioSource>()) newExplosion.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume");
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