using System.Collections;
using UnityEngine;

public class KoutakriMain : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Laser Machine")]
    [SerializeField] private long laserDamage = 15;
    [SerializeField] private float laserSpeed = 15;
    [SerializeField] private float laserMachineFireRate = 0.15f;
    [SerializeField] private int laserMachineShots = 30;

    [Header("Scattered Laser Shot")]
    [SerializeField] private long scatterlaserDamage = 12;
    [SerializeField] private float scatterlaserSpeed = 17;
    [SerializeField] private int scatterlaserShots = 6;

    [Header("Scorching Beam")]
    [SerializeField] private float scorchingBeamTime = 5;

    [Header("Ability Objects")]
    [SerializeField] private GameObject beam = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioClip circleBombSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject laser = null;
    [SerializeField] private GameObject scatterlaser = null;
    [SerializeField] private Transform[] bulletSpawns = new Transform[0];
    [SerializeField] private Transform scorchingBeamSpawn = null;

    private AudioSource audioSource;
    private bool usingAbility = false;
    private bool usingScorchingBeam = false;
    private GameObject beamObject;

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
            laserDamage = (long)(laserDamage * 0.9);
            scatterlaserDamage = (long)(scatterlaserDamage * 0.9);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            laserDamage = (long)(laserDamage * 1.2);
            laserSpeed *= 1.1f;
            scatterlaserDamage = (long)(scatterlaserDamage * 1.2);
            scatterlaserSpeed *= 1.05f;
            laserMachineFireRate *= 0.95f;
            laserMachineShots = (int)(laserMachineShots * 1.25);
            scorchingBeamTime *= 1.25f;
            abilityTime -= new Vector2(0, -0.5f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            laserDamage = (long)(laserDamage * 1.4);
            laserSpeed *= 1.2f;
            scatterlaserDamage = (long)(scatterlaserDamage * 1.4);
            scatterlaserSpeed *= 1.1f;
            laserMachineFireRate *= 0.9f;
            laserMachineShots = (int)(laserMachineShots * 1.5);
            scatterlaserShots = (int)(scatterlaserShots * 1.5);
            scorchingBeamTime *= 1.5f;
            abilityTime -= new Vector2(-0.5f, -0.5f);
        }
        StartCoroutine(main());
    }

    #region Main Functions
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
                    if (!usingScorchingBeam)
                    {
                        if (random <= 0.25f) //Scorching Beam (25% chance)
                        {
                            StartCoroutine(scorchingBeam());
                        } else if (random <= 0.5f) //Scattered Laser Shot (25% chance)
                        {
                            scatteredLaserShot();
                        } else if (random <= 0.75f) //Laser Machine (25% chance)
                        {
                            StartCoroutine(laserMachine());
                        } else //Circle Bomb (25% chance)
                        {
                            circleBomb();
                        }
                    } else
                    {
                        if (random <= 0.33f) //Scattered Laser Shot (33% chance)
                        {
                            scatteredLaserShot();
                        } else if (random <= 0.33f) //Laser Machine (33% chance)
                        {
                            StartCoroutine(laserMachine());
                        } else if (random <= 0.75f) //Circle Bomb (34% chance)
                        {
                            circleBomb();
                        }
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
    #endregion

    #region Ability Functions
    IEnumerator laserMachine()
    {
        usingAbility = true;
        for (int i = 0; i < laserMachineShots; i++)
        {
            spawnProjectile(laser, bulletSpawns[0].position, new Vector3(90, 90, -90), 0, laserDamage, laserSpeed, true);
            spawnProjectile(laser, bulletSpawns[1].position, new Vector3(90, 90, -90), 0, laserDamage, laserSpeed, true);
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
            yield return new WaitForSeconds(laserMachineFireRate);
        }
        usingAbility = false;
    }

    void scatteredLaserShot()
    {
        for (int i = 0; i < scatterlaserShots; i++)
        {
            float x = 90;
            if (PlayerPrefs.GetInt("Difficulty") < 4) //Nightmare
            {
                x = Random.Range(45, 135);
            } else
            {
                x = Random.Range(60, 120);
            }
            spawnProjectile(scatterlaser, bulletSpawns[0].position, new Vector3(x, 90, -90), 0, scatterlaserDamage, scatterlaserSpeed, false);
            spawnProjectile(scatterlaser, bulletSpawns[1].position, new Vector3(x, 90, -90), 0, scatterlaserDamage, scatterlaserSpeed, false);
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

    void circleBomb()
    {
        if (PlayerPrefs.GetInt("Difficulty") < 4)
        {
            int spawn = Random.Range(0, 1);
            float x = 0;
            for (int i = 0; i < 10; i++)
            {
                spawnProjectile(laser, bulletSpawns[spawn].position, new Vector3(x + Random.Range(-7.5f, 7.5f), 90, -90), 0, laserDamage, laserSpeed * 0.7f, false);
                x += 20;
            }
        } else
        {
            float x1 = 0;
            float x2 = 0;
            for (int i = 0; i < 10; i++)
            {
                spawnProjectile(laser, bulletSpawns[0].position, new Vector3(x1 + Random.Range(-15, 15), 90, -90), 0, laserDamage, laserSpeed * 0.7f, false);
                x1 += 20;
            }
            for (int i = 0; i < 10; i++)
            {
                spawnProjectile(laser, bulletSpawns[1].position, new Vector3(x2 + Random.Range(-15, 15), 90, -90), 0, laserDamage, laserSpeed * 0.7f, false);
                x2 += 20;
            }
        }
        if (audioSource)
        {
            if (circleBombSound)
            {
                audioSource.PlayOneShot(circleBombSound);
            } else
            {
                audioSource.Play();
            }
        }
    }

    IEnumerator scorchingBeam()
    {
        if (!beamObject)
        {
            GameObject newBeam = Instantiate(beam, scorchingBeamSpawn.position, Quaternion.Euler(0, 0, 0));
            newBeam.GetComponent<ScorchingBeam>().origin = scorchingBeamSpawn;
            beamObject = newBeam;
        } else
        {
            Destroy(beamObject);
            GameObject newBeam = Instantiate(beam, scorchingBeamSpawn.position, Quaternion.Euler(0, 0, 0));
            newBeam.GetComponent<ScorchingBeam>().origin = scorchingBeamSpawn;
            beamObject = newBeam;
        }
        usingScorchingBeam = true;
        yield return new WaitForSeconds(scorchingBeamTime);
        if (beamObject) Destroy(beamObject);
        usingScorchingBeam = false;
    }
    #endregion
}