using UnityEngine;

public class TriTorpedo : MonoBehaviour
{
    [SerializeField] private GameObject[] bulletSpawns = new GameObject[0];

    private EnemyGun enemyGun;

    void Start()
    {
        enemyGun = GetComponent<EnemyGun>();
        if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            foreach (GameObject bulletSpawn in bulletSpawns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
            enemyGun.damage = (long)(enemyGun.damage * 1.1f);
        } else //Easy, Normal and Hard
        {
            foreach (GameObject bulletSpawn in bulletSpawns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(false);
            }
        }
        enabled = false;
    }
}
