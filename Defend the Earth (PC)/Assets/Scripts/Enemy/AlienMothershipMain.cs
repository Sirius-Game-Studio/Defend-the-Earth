using System.Collections;
using UnityEngine;

public class AlienMothershipMain : MonoBehaviour
{
    public static AlienMothershipMain instance;

    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(6, 7);
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
    [SerializeField] private long bulletDamage = 16;
    [SerializeField] private float bulletSpeed = 11.25f;
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
            bulletDamage = 16;
            bulletDamage = (long)(bulletDamage * 0.9);
            bulletSpeed *= 0.8f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            bulletDamage = 16;
            bulletDamage = (long)(bulletDamage * 1.2);
            bulletSpeed *= 1.05f;
            abilityTime -= new Vector2(0, -0.5f);
            torpedoBarrageShots = 17;
            torpedoBarrageFireRate = 0.25f;
            UFODeploymentTime -= 2.5f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            bulletDamage = 18;
            bulletDamage = (long)(bulletDamage * 1.4);
            bulletSpeed = 12.5f;
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
    }

    //Main Functions
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
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void spawnProjectile(GameObject projectile, Vector3 spawnPosition, long damage, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(90, 0, 0));
        if (turnToPlayer && GameObject.FindWithTag("Player")) bullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
        bullet.GetComponent<EnemyHit>().damage = damage;
        bullet.GetComponent<Mover>().speed = speed;
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

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }

    //Ability Functions
    void doubleShot()
    {
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            spawnProjectile(bioTorpedo, bulletSpawns[1].position, (long)(bulletDamage * 1.5), bulletSpeed * 1.25f, true);
            spawnProjectile(bioTorpedo, bulletSpawns[2].position, (long)(bulletDamage * 1.5), bulletSpeed * 1.25f, true);
        } else
        {
            spawnProjectile(alienMissile, bulletSpawns[1].position, (long)(bulletDamage * 1.5), bulletSpeed * 1.5f, true);
            spawnProjectile(alienMissile, bulletSpawns[2].position, (long)(bulletDamage * 1.5), bulletSpeed * 1.5f, true);
        }
        if (audioSource)
        {
            if (fireSound)
            {
                audioSource.PlayOneShot(fireSound, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
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
        foreach (Transform torpedo in ability.transform)
        {
            EnemyHit enemyHit = torpedo.GetComponent<EnemyHit>();
            Mover mover = torpedo.GetComponent<Mover>();
            if (enemyHit && mover)
            {
                if (PlayerPrefs.GetInt("Difficulty") <= 3)
                {
                    enemyHit.damage = bulletDamage;
                } else
                {
                    enemyHit.damage = bulletDamage;
                }
                torpedo.GetComponent<Mover>().speed = bulletSpeed;
            }
        }
        if (audioSource)
        {
            if (bustedShotFireSound)
            {
                audioSource.PlayOneShot(bustedShotFireSound, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
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
                spawnProjectile(alienMissile, bulletSpawns[1].position, bulletDamage, bulletSpeed + 0.5f, true);
                spawnProjectile(alienMissile, bulletSpawns[2].position, bulletDamage, bulletSpeed + 0.5f, true);
            }
            if (audioSource)
            {
                if (fireSound)
                {
                    audioSource.PlayOneShot(fireSound, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
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
            newUFO.GetComponent<UFODeployMotion>().y += getFinalUFOPosition();
        }
    }
}