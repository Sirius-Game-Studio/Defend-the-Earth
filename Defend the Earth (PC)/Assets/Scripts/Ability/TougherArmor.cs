using UnityEngine;

public class TougherArmor : MonoBehaviour
{
    [SerializeField] private GameObject[] bulletSpawns = new GameObject[0];

    private EnemyHealth enemyHealth;
    private EnemyHit enemyHit;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHit = GetComponent<EnemyHit>();
        if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            foreach (GameObject bulletSpawn in bulletSpawns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
            enemyHealth.health = (long)(enemyHealth.health * 1.25);
            enemyHealth.defense *= 0.9f;
            enemyHit.damage = (long)(enemyHit.damage * 1.2);
        } else
        {
            foreach (GameObject bulletSpawn in bulletSpawns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(false);
            }
        }
        enabled = false;
    }
}
