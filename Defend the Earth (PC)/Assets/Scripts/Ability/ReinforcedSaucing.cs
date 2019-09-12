using UnityEngine;

public class ReinforcedSaucing : MonoBehaviour
{
    [SerializeField] private GameObject[] bulletSpawns = new GameObject[0];

    private EnemyHealth enemyHealth;
    private Mover mover;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        mover = GetComponent<Mover>();
        if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            foreach (GameObject bulletSpawn in bulletSpawns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
            bulletSpawns[0].SetActive(false);
            enemyHealth.defense *= 1.05f;
            mover.speed *= 1.1f;
        } else //Easy, Normal and Hard
        {
            bulletSpawns[1].SetActive(false);
            bulletSpawns[2].SetActive(false);
        }
        enabled = false;
    }
}