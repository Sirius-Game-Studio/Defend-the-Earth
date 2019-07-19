using System.Collections;
using UnityEngine;

public class AlienMothershipMain : MonoBehaviour
{
    public static AlienMothershipMain instance;

    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(6, 7);
    [SerializeField] private AudioClip music = null;

    [Header("Torpedo Barrage Settings")]
    [Tooltip("The amount of shots to fire.")] [SerializeField] private long torpedoBarrageShots = 12;
    [SerializeField] private float torpedoBarrageFireRate = 0.3f;

    [Header("UFO Deployment Settings")]
    [Tooltip("The maximum amount of UFOs that can be deployed.")] [SerializeField] private int maxUFOs = 2;
    [SerializeField] private float UFODeploymentTime = 15;

    [Header("Ability Objects")]
    [Tooltip("Required for Busted Shot ability.")] [SerializeField] private GameObject bustedShotObject = null;
    [Tooltip("Required for UFO Deployment ability.")] [SerializeField] private GameObject UFO = null;

    [Header("Setup")]
    [SerializeField] private long bulletDamage = 18;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private AudioClip fireSound = null;
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
        if (PlayerPrefs.GetInt("Difficulty") <= 1)
        {
            bulletDamage = (long)(bulletDamage * 0.9);
            bulletSpeed = 8.5f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            bulletDamage = (long)(bulletDamage * 1.2);
            bulletSpeed = 11;
            torpedoBarrageShots = 14;
            torpedoBarrageFireRate = 0.25f;
            UFODeploymentTime -= 2;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            bulletDamage = (long)(bulletDamage * 1.4);
            bulletSpeed = 12;
            torpedoBarrageShots = 16;
            torpedoBarrageFireRate = 0.225f;
            UFODeploymentTime -= 4;
        }
        StartCoroutine(main());
    }

    void Update()
    {
        if (audioSource && PlayerPrefs.HasKey("MusicVolume")) audioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        if (deployedUFOs < maxUFOs)
        {
            if (timeTillUFODeployment < UFODeploymentTime)
            {
                timeTillUFODeployment += Time.deltaTime;
            } else
            {
                ufoDeployment();
                timeTillUFODeployment = 0;
            }
        }
        int amount = 0;
        foreach (GameObject deployedUFO in FindObjectsOfType<GameObject>())
        {
            if (deployedUFO.name.ToLower() == UFO.name.ToLower()) ++amount;
        }
        deployedUFOs = amount;
    }

    IEnumerator main()
    {
        while (!GameController.instance.gameOver && !GameController.instance.won)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !usingAbility)
            {
                yield return new WaitForSeconds(Random.Range(abilityTime.x, abilityTime.y));
                if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && !usingAbility)
                {
                    float random = Random.value;
                    if (random <= 0.3f) //Busted Shot (30% chance)
                    {
                        bustedShot();
                    } else if (random <= 0.75f) //Torpedo Barrage (45% chance)
                    {
                        StartCoroutine(torpedoBarrage());
                    } else //Double Shot (25% chance)
                    {
                        doubleShot();
                    }
                }
            } else
            {
                yield return new WaitForEndOfFrame();
            }
        }
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

    void spawnProjectile(GameObject projectile, Vector3 spawnPosition, long damage, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(90, 0, 0));
        if (turnToPlayer && GameObject.FindWithTag("Player")) bullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
        if (damage > 0) bullet.GetComponent<EnemyHit>().damage = damage;
        if (speed > 0) bullet.GetComponent<Mover>().speed = speed;
    }

    //Ability Functions
    void doubleShot()
    {
        if (PlayerPrefs.GetInt("Difficulty") < 4)
        {
            spawnProjectile(bioTorpedo, bulletSpawns[1].position, bulletDamage, bulletSpeed, true);
            spawnProjectile(bioTorpedo, bulletSpawns[2].position, bulletDamage, bulletSpeed, true);
        } else
        {
            spawnProjectile(alienMissile, bulletSpawns[1].position, bulletDamage + 1, bulletSpeed + 0.5f, true);
            spawnProjectile(alienMissile, bulletSpawns[2].position, bulletDamage + 1, bulletSpeed + 0.5f, true);
        }
        if (audioSource)
        {
            if (fireSound)
            {
                audioSource.PlayOneShot(fireSound, PlayerPrefs.GetFloat("SoundVolume"));
            } else
            {
                audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
                audioSource.Play();
            }
        }
    }

    void bustedShot()
    {
        GameObject ability = Instantiate(bustedShotObject, bulletSpawns[0].position, Quaternion.Euler(0, 0, 0));
        foreach (Transform torpedo in ability.transform)
        {
            if (torpedo.GetComponent<EnemyHit>() && torpedo.GetComponent<Mover>())
            {
                torpedo.GetComponent<EnemyHit>().damage = bulletDamage;
                torpedo.GetComponent<Mover>().speed = bulletSpeed;
            }
        }
        if (fireSound)
        {
            audioSource.PlayOneShot(fireSound, PlayerPrefs.GetFloat("SoundVolume"));
        } else
        {
            audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
            audioSource.Play();
        }
    }

    IEnumerator torpedoBarrage()
    {
        usingAbility = true;
        for (int i = 0; i < torpedoBarrageShots; i++)
        {
            if (PlayerPrefs.GetInt("Difficulty") < 4)
            {
                float random = Random.value;
                if (random <= 0.5f)
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[1].position, bulletDamage, bulletSpeed, true);
                } else
                {
                    spawnProjectile(bioTorpedo, bulletSpawns[2].position, bulletDamage, bulletSpeed, true);
                }
            } else
            {
                spawnProjectile(alienMissile, bulletSpawns[1].position, bulletDamage + 1, bulletSpeed + 0.5f, true);
                spawnProjectile(alienMissile, bulletSpawns[2].position, bulletDamage + 1, bulletSpeed + 0.5f, true);
            }
            if (audioSource)
            {
                if (fireSound)
                {
                    audioSource.PlayOneShot(fireSound, PlayerPrefs.GetFloat("SoundVolume"));
                } else
                {
                    audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(torpedoBarrageFireRate);
        }
        usingAbility = false;
    }

    void ufoDeployment()
    {
        if (deployedUFOs < maxUFOs)
        {
            GameObject newUFO = Instantiate(UFO, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(90, 180, 0));
            newUFO.name = UFO.name;
            newUFO.GetComponent<UFODeployMotion>().y += getFinalUFOPosition();
        }
    }
}