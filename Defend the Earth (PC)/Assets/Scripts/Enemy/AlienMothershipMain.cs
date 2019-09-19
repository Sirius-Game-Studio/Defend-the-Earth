using System.Collections;
using UnityEngine;

public class AlienMothershipMain : MonoBehaviour
{
    [Header("Torpedo Barrage")]
    [Tooltip("The amount of shots to fire.")] [SerializeField] private long torpedoBarrageShots = 12;
    [SerializeField] private float torpedoBarrageFireRate = 0.3f;

    [Header("UFO Deployment")]
    [SerializeField] private int maxUFOs = 2;
    [SerializeField] private float UFODeploymentTime = 15;

    [Header("Settings")]
    [SerializeField] private long bioTorpedoDamage = 17;
    [SerializeField] private long missileDamage = 19;
    [SerializeField] private float bioTorpedoSpeed = 12;
    [SerializeField] private float missileSpeed = 13;
    [SerializeField] private Vector2 abilityTime = new Vector2(3.5f, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioClip bustedShotFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject bioTorpedo = null;
    [SerializeField] private GameObject alienMissile = null;
    [Tooltip("Easy, Normal and Hard only.")] [SerializeField] private GameObject weakBustedShot = null;
    [Tooltip("Nightmare only.")] [SerializeField] private GameObject strongBustedShot = null;
    [Tooltip("Required for UFO Deployment ability.")] [SerializeField] private GameObject UFO = null;
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
            bioTorpedoDamage = (long)(bioTorpedoDamage * 0.9);
            missileDamage = (long)(missileDamage * 0.9);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            bioTorpedoDamage = (long)(bioTorpedoDamage * 1.2);
            missileDamage = (long)(missileDamage * 1.2);
            bioTorpedoSpeed *= 1.05f;
            missileSpeed *= 1.05f;
            torpedoBarrageShots = (int)(torpedoBarrageShots * 1.25);
            torpedoBarrageFireRate *= 0.9f;
            UFODeploymentTime -= 2.5f;
            abilityTime -= new Vector2(0, -0.5f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            bioTorpedoDamage = (long)(bioTorpedoDamage * 1.4);
            missileDamage = (long)(missileDamage * 1.4);
            bioTorpedoSpeed *= 1.1f;
            missileSpeed *= 1.1f;
            torpedoBarrageShots = (int)(torpedoBarrageShots * 1.5);
            torpedoBarrageFireRate *= 0.8f;
            UFODeploymentTime -= 5;
            abilityTime -= new Vector2(0, -0.5f);
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
                if (amount > 1) newY += 2;
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
            spawnProjectile(bioTorpedo, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, (long)(bioTorpedoDamage * 1.5), bioTorpedoSpeed * 1.25f, true);
            spawnProjectile(bioTorpedo, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, (long)(bioTorpedoDamage * 1.5), bioTorpedoSpeed * 1.25f, true);
        } else
        {
            spawnProjectile(alienMissile, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, (long)(missileDamage * 1.5), missileSpeed * 1.5f, true);
            spawnProjectile(alienMissile, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, (long)(missileDamage * 1.5), missileSpeed * 1.5f, true);
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
        long damage;
        float speed;
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            ability = Instantiate(weakBustedShot, bulletSpawns[0].position, Quaternion.Euler(0, 0, 0));
            damage = bioTorpedoDamage;
            speed = bioTorpedoSpeed;
        } else //Nightmare
        {
            ability = Instantiate(strongBustedShot, bulletSpawns[0].position, Quaternion.Euler(0, 0, 0));
            damage = missileDamage;
            speed = missileSpeed;
        }
        foreach (Transform projectile in ability.transform)
        {
            if (projectile.CompareTag("Projectile"))
            {
                EnemyHit enemyHit = projectile.GetComponent<EnemyHit>();
                Mover mover = projectile.GetComponent<Mover>();
                if (enemyHit && mover)
                {
                    enemyHit.damage = damage;
                    mover.speed = speed;
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
            if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
            {
                float random = Random.value;
                if (random <= 0.5f)
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, bioTorpedoDamage, bioTorpedoSpeed, true);
                } else
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, bioTorpedoDamage, bioTorpedoSpeed, true);
                }
            } else //Nightmare
            {
                spawnProjectile(alienMissile, bulletSpawns[1].position, new Vector3(90, 0, 0), 0, missileDamage, missileSpeed, true);
                spawnProjectile(alienMissile, bulletSpawns[2].position, new Vector3(90, 0, 0), 0, missileDamage, missileSpeed, true);
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
                newUFO.GetComponent<EnemyHealth>().health = (long)(newUFO.GetComponent<EnemyHealth>().health * 1.2);
                newUFO.GetComponent<EnemyGun>().damage = (long)(UFO.GetComponent<EnemyGun>().damage * 1.3);
                newUFO.GetComponent<EnemyGun>().RPM *= 1.1f;
            }
            newUFO.GetComponent<UFODeployMotion>().y += getFinalUFOPosition();
        }
    }
}