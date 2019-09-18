using System.Collections;
using UnityEngine;

public class AlienMothershipMain : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3.5f, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Torpedo Barrage Settings")]
    [Tooltip("The amount of shots to fire.")] [SerializeField] private long torpedoBarrageShots = 14;
    [SerializeField] private float torpedoBarrageFireRate = 0.3f;

    [Header("UFO Deployment Settings")]
    [Tooltip("The maximum amount of UFOs that can be deployed.")] [SerializeField] private int maxUFOs = 2;
    [SerializeField] private float UFODeploymentTime = 15;

    [Header("Ability Objects")]
    [Tooltip("Required for Busted Shot (Easy, Normal and Hard) ability.")] [SerializeField] private GameObject weakBustedShot = null;
    [Tooltip("Required for Busted Shot (Nightmare) ability.")] [SerializeField] private GameObject strongBustedShot = null;
    [Tooltip("Required for UFO Deployment ability.")] [SerializeField] private GameObject UFO = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioClip bustedShotFireSound = null;

    [Header("Setup")]
    [SerializeField] private long damage = 17;
    [SerializeField] private float bulletSpeed = 12;
    [SerializeField] private GameObject bioTorpedo = null;
    [SerializeField] private GameObject alienMissile = null;
    [SerializeField] private Transform[] bulletSpawns = new Transform[0];

    private AudioSource audioSource;
    private bool usingAbility = false;
    private float timeTillUFODeployment = 0;
    private int deployedUFOs = 0; //Amount of UFOs deployed

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (Camera.main.GetComponent<AudioSource>() && music)
        {
            Camera.main.GetComponent<AudioSource>().clip = music;
            Camera.main.GetComponent<AudioSource>().Stop();
            Camera.main.GetComponent<AudioSource>().Play();
        }
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            damage = 17;
            damage = (long)(damage * 0.9);
            bulletSpeed = 12;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            damage = 17;
            damage = (long)(damage * 1.2);
            bulletSpeed = 12;
            bulletSpeed *= 1.05f;
            abilityTime -= new Vector2(0, -0.5f);
            torpedoBarrageShots = 17;
            torpedoBarrageFireRate = 0.25f;
            UFODeploymentTime -= 2.5f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            damage = 19;
            damage = (long)(damage * 1.4);
            bulletSpeed = 12.75f;
            bulletSpeed *= 1.1f;
            abilityTime -= new Vector2(-0.5f, -0.5f);
            torpedoBarrageShots = 20;
            torpedoBarrageFireRate = 0.225f;
            UFODeploymentTime -= 5;
        }
        StartCoroutine(main());
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && deployedUFOs < maxUFOs)
        {
            if (timeTillUFODeployment < UFODeploymentTime)
            {
                timeTillUFODeployment += Time.deltaTime;
            } else
            {
                UFODeployment();
                timeTillUFODeployment = 0;
            }
        }

        //Looks for enemies with the same name as the UFO to deploy, then updates the amount
        int amount = 0;
        foreach (GameObject deployedUFO in FindObjectsOfType<GameObject>())
        {
            if (deployedUFO.name.ToLower() == UFO.name.ToLower()) ++amount;
        }
        deployedUFOs = amount;

        if (damage < 1) damage = 1; //Checks if damage is less than 1
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
                    if (random <= 0.3f) //Torpedo Barrage (30% chance)
                    {
                        StartCoroutine(torpedoBarrage());
                    } else if (random <= 0.75f) //Busted Shot (45% chance)
                    {
                        bustedShot();
                    } else //Double Shot (25% chance)
                    {
                        doubleShot();
                    }
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

    float getFinalUFOPosition()
    {
        float newY = 0;
        int amount = 0;
        foreach (GameObject deployedUFO in FindObjectsOfType<GameObject>())
        {
            if (deployedUFO.name.ToLower() == UFO.name.ToLower())
            {
                ++amount;
                if (amount > 1) newY += 2f;
            }
        }
        if (amount > 1)
        {
            return newY;
        } else
        {
            return 0;
        }
    }

    //Ability Functions
    void doubleShot()
    {
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            spawnProjectile(bioTorpedo, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, (long)(damage * 1.5), bulletSpeed * 1.25f, true);
            spawnProjectile(bioTorpedo, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, (long)(damage * 1.5), bulletSpeed * 1.25f, true);
        } else
        {
            spawnProjectile(alienMissile, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, (long)(damage * 1.5), bulletSpeed * 1.5f, true);
            spawnProjectile(alienMissile, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, (long)(damage * 1.5), bulletSpeed * 1.5f, true);
        }
        if (audioSource)
        {
            if (fireSound)
            {
                audioSource.PlayOneShot(fireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }

    void bustedShot()
    {
        GameObject ability;
        if (PlayerPrefs.GetInt("Difficulty") <= 3)
        {
            ability = Instantiate(weakBustedShot, bulletSpawns[0].position, Quaternion.Euler(0, 0, 0));
        } else
        {
            ability = Instantiate(strongBustedShot, bulletSpawns[0].position, Quaternion.Euler(0, 0, 0));
        }
        foreach (Transform projectile in ability.transform)
        {
            if (projectile.CompareTag("Projectile"))
            {
                EnemyHit enemyHit = projectile.GetComponent<EnemyHit>();
                Mover mover = projectile.GetComponent<Mover>();
                if (enemyHit && mover)
                {
                    if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
                    {
                        enemyHit.damage = damage;
                    } else //Nightmare
                    {
                        enemyHit.damage = (long)(damage * 1.05);
                    }
                    mover.speed = bulletSpeed;
                }
            }
        }
        if (audioSource)
        {
            if (bustedShotFireSound)
            {
                audioSource.PlayOneShot(bustedShotFireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }

    IEnumerator torpedoBarrage()
    {
        usingAbility = true;
        for (int i = 0; i < torpedoBarrageShots; i++)
        {
            if (PlayerPrefs.GetInt("Difficulty") < 4) //Nightmare
            {
                float random = Random.value;
                if (random <= 0.5f)
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, damage, bulletSpeed * 1.05f, true);
                } else
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, damage, bulletSpeed * 1.05f, true);
                }
            } else //Easy, Normal and Hard
            {
                spawnProjectile(alienMissile, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, damage, bulletSpeed * 1.1f, true);
                spawnProjectile(alienMissile, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, damage, bulletSpeed * 1.1f, true);
            }
            if (audioSource)
            {
                if (fireSound)
                {
                    audioSource.PlayOneShot(fireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(torpedoBarrageFireRate);
        }
        usingAbility = false;
    }

    void UFODeployment()
    {
        if (deployedUFOs < maxUFOs)
        {
            GameObject newUFO = Instantiate(UFO, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(90, 180, 0));
            newUFO.name = UFO.name;
            if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
            {
                newUFO.GetComponent<EnemyHealth>().health = (long)(newUFO.GetComponent<EnemyHealth>().health * 1.2f);
                newUFO.GetComponent<EnemyGun>().damage = (long)(UFO.GetComponent<EnemyGun>().damage * 1.3f);
                newUFO.GetComponent<EnemyGun>().RPM *= 1.1f;
            }
            newUFO.GetComponent<UFODeployMotion>().y += getFinalUFOPosition();
        }
    }
}