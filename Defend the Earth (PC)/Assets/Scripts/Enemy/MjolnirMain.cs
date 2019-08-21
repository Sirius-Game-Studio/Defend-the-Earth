using System.Collections;
using UnityEngine;

public class MjolnirMain : MonoBehaviour
{
    [Header("Longshot Guns")]
    [SerializeField] private long longlaserDamage = 14;
    [SerializeField] private float longlaserSpeed = 15;
    [SerializeField] private float longshotGunsFireRate = 0.25f;
    [SerializeField] private int longshotGunsShotAmount = 8;

    [Header("Anti-Armor Missiles")]
    [SerializeField] private long shipkillerDamage = 31;
    [SerializeField] private float shipkillerSpeed = 17.5f;
    [SerializeField] private float aaMissilesFireRate = 0.2f;
    [SerializeField] private int aaMissilesShotAmount = 8;

    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 4);
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioClip longshotGunsSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject longlaser = null;
    [SerializeField] private GameObject shipkillerMissile = null;
    [SerializeField] private Transform[] bulletSpawns = new Transform[0];
    [SerializeField] private Transform[] chargeGlows = new Transform[0];

    private AudioSource audioSource;
    private bool usingAbility = false;

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
            longlaserDamage = (long)(longlaserDamage * 0.9);
            shipkillerDamage = (long)(shipkillerDamage * 0.9);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            longlaserDamage = (long)(longlaserDamage * 1.2);
            shipkillerDamage = (long)(shipkillerDamage * 1.2);
            longlaserSpeed *= 1.1f;
            shipkillerSpeed *= 1.1f;
            longshotGunsShotAmount = (int)(longshotGunsShotAmount * 1.5);
            aaMissilesShotAmount = (int)(aaMissilesShotAmount * 1.25);
            abilityTime -= new Vector2(0, -0.5f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            longlaserDamage = (long)(longlaserDamage * 1.4);
            shipkillerDamage = (long)(shipkillerDamage * 1.4);
            longlaserSpeed *= 1.2f;
            shipkillerSpeed *= 1.2f;
            longshotGunsShotAmount *= 2;
            aaMissilesShotAmount = (int)(aaMissilesShotAmount * 1.5);
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
                    if (random <= 0.5f) //Anti-Armor Missiles (% chance)
                    {
                        StartCoroutine(antiarmorMissiles());
                    } else //Longshot Guns (% chance)
                    {
                        StartCoroutine(longshotGuns());
                    }
                }
            } else
            {
                yield return null;
            }
        }
    }

    GameObject spawnProjectile(GameObject projectile, Vector3 spawnPosition, Vector3 spawnRotation, long damage, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z));
        if (turnToPlayer && GameObject.FindWithTag("Player")) bullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
        bullet.GetComponent<EnemyHit>().damage = damage;
        bullet.GetComponent<Mover>().speed = speed;
        return bullet;
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
    IEnumerator longshotGuns()
    {
        int point = 0;
        usingAbility = true;
        for (int i = 0; i < longshotGunsShotAmount; i++)
        {
            spawnProjectile(longlaser, bulletSpawns[point].position, new Vector3(90, 0, 0), longlaserDamage, longlaserSpeed, true);
            if (audioSource)
            {
                if (longshotGunsSound)
                {
                    audioSource.PlayOneShot(longshotGunsSound, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(longshotGunsFireRate);
            ++point;
            if (point > 1) point = 0;
        }
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            usingAbility = false;
        } else //Nightmare
        {
            float random = Random.value;
            if (random <= 0.5f) //Left
            {
                point = 0;
            } else //Right
            {
                point = 1;
            }
            foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
            while (chargeGlows[point].localScale.x < 0.0115f)
            {
                yield return new WaitForEndOfFrame();
                chargeGlows[point].localScale += new Vector3(0.00005f, 0.00005f, 0);
            }
            foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
            for (int i = 0; i < longshotGunsShotAmount; i++)
            {
                spawnProjectile(longlaser, bulletSpawns[point].position, new Vector3(90, 0, 0), (long)(longlaserDamage * 1.1), longlaserSpeed * 1.05f, true);
                if (audioSource)
                {
                    if (longshotGunsSound)
                    {
                        audioSource.PlayOneShot(longshotGunsSound, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
                yield return new WaitForSeconds(longshotGunsFireRate * 0.75f);
            }
            usingAbility = false;
        }
    }

    IEnumerator antiarmorMissiles()
    {
        usingAbility = true;
        for (int i = 0; i < 6; i++)
        {
            spawnProjectile(shipkillerMissile, bulletSpawns[Random.Range(2, 3)].position, new Vector3(90, 0, 0), shipkillerDamage, shipkillerSpeed, true);
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
            yield return new WaitForSeconds(aaMissilesFireRate);
        }
        usingAbility = false;
    }
}