using System.Collections;
using UnityEngine;

public class EvilMain : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip n = null;

    [Header("Setup")]
    [SerializeField] private bool Unity = false;

    private AudioSource audioSource;
    private EnemyHealth enemyHealth;
    private bool usingAbility = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemyHealth = GetComponent<EnemyHealth>();
        if (Camera.main.GetComponent<AudioSource>() && music)
        {
            Camera.main.GetComponent<AudioSource>().clip = music;
            Camera.main.GetComponent<AudioSource>().Stop();
            Camera.main.GetComponent<AudioSource>().Play();
        }
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {

        }
        else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            abilityTime -= new Vector2(0, -0.5f);
        }
        else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            abilityTime -= new Vector2(-0.5f, -0.5f);
        }
        StartCoroutine(main());
    }

    //Main Functions
    IEnumerator main()
    {
        while (true)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !usingAbility)
            {
                yield return new WaitForSeconds(Random.Range(abilityTime.x, abilityTime.y));
                if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && !usingAbility)
                {
                    float random = Random.value;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    GameObject spawnProjectile(GameObject projectile, Vector3 spawnPosition, Vector3 spawnRotation, float spreadDegree, long damage, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z));
        if (turnToPlayer && GameObject.FindWithTag("Player")) bullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
        if (spreadDegree != 0) bullet.transform.Rotate(Random.Range(-spreadDegree, spreadDegree), 0, 0);
        bullet.GetComponent<EnemyHit>().damage = damage;
        bullet.GetComponent<Mover>().speed = speed;
        return bullet;
    }

    //Ability Functions
}