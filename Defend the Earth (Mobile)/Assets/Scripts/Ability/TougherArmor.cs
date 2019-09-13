using UnityEngine;

public class TougherArmor : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private EnemyHit enemyHit;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHit = GetComponent<EnemyHit>();
        if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            enemyHealth.health = (long)(enemyHealth.health * 1.25);
            enemyHealth.defense *= 1.1f;
            enemyHit.damage = (long)(enemyHit.damage * 1.2);
        }
        enabled = false;
    }
}
