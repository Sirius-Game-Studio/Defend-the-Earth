using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [Header("Settings")]
    public long damage = 30;
    [Tooltip("Only works when enemyHealth is set to a GameObject with the EnemyHealth component.")] public float lifesteal = 0;
    [SerializeField] private bool instakill = false;

    [Header("Setup")]
    [SerializeField] private GameObject explosion = null;
    [HideInInspector] public EnemyHealth enemyHealth;

    private bool hit = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            damage = (long)(damage * 0.75);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            damage = (long)(damage * 1.15);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            damage = (long)(damage * 1.3);
        }
    }

    void Update()
    {
        if (damage < 1) damage = 1; //Checks if damage is less than 1 
        if (lifesteal < 0) lifesteal = 0; //Checks if lifesteal percentage is less than 0
    }

    void OnTriggerStay(Collider other)
    {
        if (!hit && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController && !playerController.invulnerable)
            {
                if (!instakill)
                {
                    playerController.takeDamage(damage);
                } else
                {
                    playerController.health = 0;
                    playerController.lives = 0;
                }
                if (enemyHealth) enemyHealth.health += (long)(damage * lifesteal);
                if (explosion) Instantiate(explosion, transform.position, transform.rotation);
                hit = true;
                Destroy(gameObject);
            }
        }
    }
}