using System.Collections;
using UnityEngine;

public class EvilMain : MonoBehaviour
{
    [Header("Perforating Cannons")]
    [SerializeField] private float perforatingCannonsFireRate = 0.2f;
    [SerializeField] private int perforatingCannonsShots = 8;
    [SerializeField] private long longlaserDamage = 17;
    [SerializeField] private float longlaserSpeed = 14;

    [Header("Battering Charge")]
    [SerializeField] private long superlaserDamage = 32;
    [SerializeField] private float superlaserSpeed = 15;

    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip perforatingCannonsFireSound = null;
    [SerializeField] private AudioClip batteringChargeFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject longlaser = null;
    [SerializeField] private GameObject batteringChargeObject = null;
    [SerializeField] private Transform[] perforatingCannonsGuns = new Transform[0];
    [SerializeField] private Transform batteringChargeSpawn = null;

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
            longlaserDamage = (long)(longlaserDamage * 0.9);
            superlaserDamage = (long)(superlaserDamage * 0.9);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            longlaserDamage = (long)(longlaserDamage * 1.2);
            longlaserSpeed *= 1.05f;
            superlaserDamage = (long)(superlaserDamage * 1.2);
            superlaserSpeed *= 1.05f;
            abilityTime -= new Vector2(0, -0.5f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            perforatingCannonsShots = (int)(perforatingCannonsShots * 1.25);
            longlaserDamage = (long)(longlaserDamage * 1.4);
            superlaserDamage = (long)(superlaserDamage * 1.4);
            longlaserSpeed *= 1.1f;
            superlaserSpeed *= 1.1f;
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
                    StartCoroutine(perforatingCannons());
                    batteringCharge();
                }
            } else
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
    IEnumerator perforatingCannons()
    {
        usingAbility = true;
        for (int i = 0; i < perforatingCannonsShots; i++)
        {
            foreach (Transform bulletSpawn in perforatingCannonsGuns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn"))
                {
                    if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
                    {
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(90, 0, 0), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(60, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(120, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(60, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(120, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                    } else //Nightmare
                    {
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(90, 0, 0), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(60, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(75, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(105, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(120, 90, 90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(60, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(75, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(105, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                        spawnProjectile(longlaser, bulletSpawn.position, new Vector3(120, -90, -90), 0, longlaserDamage, longlaserSpeed, false);
                    }
                }
            }
            if (audioSource)
            {
                if (perforatingCannonsFireSound)
                {
                    audioSource.PlayOneShot(perforatingCannonsFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(perforatingCannonsFireRate);
        }
        usingAbility = false;
    }

    void batteringCharge()
    {
        GameObject ability = Instantiate(batteringChargeObject, batteringChargeSpawn.position, Quaternion.Euler(0, 0, 0));
        foreach (Transform projectile in ability.transform)
        {
            if (projectile.CompareTag("Projectile"))
            {
                EnemyHit enemyHit = projectile.GetComponent<EnemyHit>();
                Mover mover = projectile.GetComponent<Mover>();
                if (enemyHit && mover)
                {
                    enemyHit.damage = superlaserDamage;
                    projectile.GetComponent<Mover>().speed = superlaserSpeed;
                }
            }
        }
        if (audioSource)
        {
            if (batteringChargeFireSound)
            {
                audioSource.PlayOneShot(batteringChargeFireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }
}