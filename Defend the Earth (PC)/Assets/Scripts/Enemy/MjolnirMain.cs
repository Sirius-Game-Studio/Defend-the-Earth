using System.Collections;
using UnityEngine;

public class MjolnirMain : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 4);
    [Tooltip("The Y position this enemy stops at.")] [SerializeField] private float yPosition = 4;
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Longshot Guns")]
    [SerializeField] private long longlaserDamage = 14;
    [SerializeField] private float longlaserSpeed = 15;
    [SerializeField] private float longshotGunsFireRate = 0.25f;
    [SerializeField] private int longshotGunsShots = 8;

    [Header("Blind Spray")]
    [SerializeField] private long scatterlaserDamage = 12;
    [SerializeField] private float scatterlaserSpeed = 16.5f;
    [SerializeField] private float scatterlaserSpread = 4.5f;
    [SerializeField] private int blindSprayShots = 6;

    [Header("Anti-Armor Missiles")]
    [SerializeField] private long shipkillerDamage = 31;
    [SerializeField] private float shipkillerSpeed = 17.5f;
    [SerializeField] private float AAMissilesFireRate = 0.2f;
    [SerializeField] private int AAMissilesShots = 8;

    [Header("Chaos Orb")]
    [SerializeField] private long chaosOrbDamage = 20;

    [Header("Protective Shield")]
    [SerializeField] private float protectiveShieldDuration = 8;
    [SerializeField] private Vector2 protectiveShieldUseTime = new Vector2(20, 30);

    [Header("Sound Effects")]
    [SerializeField] private AudioClip longshotGunsFireSound = null;
    [SerializeField] private AudioClip blindSprayFireSound = null;
    [SerializeField] private AudioClip AAMissilesFireSound = null;
    [SerializeField] private AudioClip chaosOrbFireSound = null;

    [Header("Setup")]
    [SerializeField] private Transform shield = null;
    [SerializeField] private GameObject longlaser = null;
    [SerializeField] private GameObject scatterlaser = null;
    [SerializeField] private GameObject shipkillerMissile = null;
    [SerializeField] private GameObject chaosOrbObject = null;
    [SerializeField] private Transform[] longlaserGuns = new Transform[0];
    [SerializeField] private Transform[] antiArmorMissilesGuns = new Transform[0];
    [SerializeField] private Transform[] chaosOrbGuns = new Transform[0];
    [SerializeField] private Transform[] chargeGlows = new Transform[0];

    private AudioSource audioSource;
    private EnemyHealth enemyHealth;
    private bool usingAbility = false;
    private bool shielded = false;
    private bool animatingCharge = false;
    private float timeTillShieldUse = 15;

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
            scatterlaserDamage = (long)(scatterlaserDamage * 0.9);
            shipkillerDamage = (long)(shipkillerDamage * 0.9);
            chaosOrbDamage = (long)(chaosOrbDamage * 0.9);
            protectiveShieldDuration *= 0.75f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            longlaserDamage = (long)(longlaserDamage * 1.2);
            scatterlaserDamage = (long)(scatterlaserDamage * 1.2);
            shipkillerDamage = (long)(shipkillerDamage * 1.2);
            chaosOrbDamage = (long)(chaosOrbDamage * 1.2);
            longlaserSpeed *= 1.1f;
            scatterlaserSpeed *= 1.1f;
            shipkillerSpeed *= 1.1f;
            scatterlaserSpread *= 1.15f;
            longshotGunsShots = (int)(longshotGunsShots * 1.5);
            blindSprayShots = (int)(blindSprayShots * 1.25);
            AAMissilesShots = (int)(AAMissilesShots * 1.25);
            protectiveShieldUseTime *= 0.9f;
            abilityTime -= new Vector2(0, 0.25f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            longlaserDamage = (long)(longlaserDamage * 1.4);
            scatterlaserDamage = (long)(scatterlaserDamage * 1.4);
            shipkillerDamage = (long)(shipkillerDamage * 1.4);
            chaosOrbDamage = (long)(chaosOrbDamage * 1.4);
            longlaserSpeed *= 1.2f;
            scatterlaserSpeed *= 1.2f;
            shipkillerSpeed *= 1.2f;
            scatterlaserSpread *= 1.3f;
            longshotGunsShots *= 2;
            blindSprayShots = (int)(blindSprayShots * 1.5);
            AAMissilesShots = (int)(AAMissilesShots * 1.5);
            protectiveShieldDuration *= 1.25f;
            protectiveShieldUseTime *= 0.8f;
            abilityTime -= new Vector2(0.25f, 0.25f);
        }
        shield.gameObject.SetActive(false);
        shield.localScale = Vector3.zero;
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        timeTillShieldUse = Random.Range(protectiveShieldUseTime.x, protectiveShieldUseTime.y);
        StartCoroutine(main());
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won && !shielded)
        {
            if (timeTillShieldUse > 0)
            {
                timeTillShieldUse -= Time.deltaTime;
            } else
            {
                StartCoroutine(protectiveShield());
            }
        }
    }

    #region Main Functions
    IEnumerator main()
    {
        transform.position = new Vector3(0, GameController.instance.bossInitialYPosition, 0);
        while (transform.position.y > yPosition)
        {
            GetComponent<EnemyHealth>().invulnerable = true;
            GetComponent<Mover>().enabled = true;
            if (GetComponent<HorizontalOnlyMover>()) GetComponent<HorizontalOnlyMover>().enabled = false;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<EnemyHealth>().invulnerable = false;
        GetComponent<Mover>().enabled = false;
        if (GetComponent<HorizontalOnlyMover>()) GetComponent<HorizontalOnlyMover>().enabled = true;
        while (true)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !usingAbility)
            {
                yield return new WaitForSeconds(Random.Range(abilityTime.x, abilityTime.y));
                if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && !usingAbility)
                {
                    float random = Random.value;
                    if (random <= 0.15f) //Chaos Orb (15% chance)
                    {
                        chaosOrb();
                    } else if (random <= 0.35f) //Anti-Armor Missiles (20% chance)
                    {
                        StartCoroutine(antiarmorMissiles());
                    } else if (random <= 0.6f) //Blind Spray (25% chance)
                    {
                        StartCoroutine(blindSpray());
                    } else //Longshot Guns (40% Chance)
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

    GameObject spawnProjectile(GameObject projectile, Vector3 spawnPosition, Vector3 spawnRotation, float spreadDegree, long damage, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z));
        if (turnToPlayer && GameObject.FindWithTag("Player")) bullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
        if (spreadDegree != 0) bullet.transform.Rotate(Random.Range(-spreadDegree, spreadDegree), 0, 0);
        bullet.GetComponent<EnemyHit>().damage = damage;
        bullet.GetComponent<Mover>().speed = speed;
        return bullet;
    }

    IEnumerator animateChargeGlow(Transform charge, float speed, float maxSize, bool forward)
    {
        animatingCharge = true;
        if (forward)
        {
            while (charge.localScale.x < maxSize)
            {
                charge.localScale += new Vector3(speed, speed, 0);
                if (charge.localScale.x > maxSize) charge.localScale = new Vector3(maxSize, maxSize, 0);
                yield return new WaitForSeconds(speed);
            }
        } else
        {
            while (charge.localScale.x > maxSize)
            {
                charge.localScale -= new Vector3(speed, speed, 0);
                if (charge.localScale.x < maxSize) charge.localScale = new Vector3(maxSize, maxSize, 0);
                yield return new WaitForSeconds(speed);
            }
        }
        animatingCharge = false;
        yield break;
    }
    #endregion

    #region Main Functions
    IEnumerator longshotGuns()
    {
        int point = 0;
        usingAbility = true;
        for (int i = 0; i < longshotGunsShots; i++)
        {
            spawnProjectile(longlaser, longlaserGuns[point].position, new Vector3(90, 0, 0), 0, longlaserDamage, longlaserSpeed, true);
            if (audioSource)
            {
                if (longshotGunsFireSound)
                {
                    audioSource.PlayOneShot(longshotGunsFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(longshotGunsFireRate);
            ++point;
            if (point > 1) point = 0;
        }
        float random = Random.value;
        if (random <= 0.5f) //Left
        {
            point = 0;
        } else //Right
        {
            point = 1;
        }
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        float chargeSpeed;
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            chargeSpeed = 0.0001f;
        } else //Nightmare
        {
            chargeSpeed = 0.0002f;
        }
        StartCoroutine(animateChargeGlow(chargeGlows[point], chargeSpeed, 0.01f, true));
        while (animatingCharge) yield return null;
        StartCoroutine(animateChargeGlow(chargeGlows[point], 0.0003f, 0, false));
        for (int i = 0; i < longshotGunsShots; i++)
        {
            spawnProjectile(longlaser, longlaserGuns[point].position, new Vector3(90, 0, 0), 0, (long)(longlaserDamage * 1.1), longlaserSpeed * 1.05f, true);
            if (audioSource)
            {
                if (longshotGunsFireSound) 
                {
                    audioSource.PlayOneShot(longshotGunsFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(longshotGunsFireRate * 0.75f);
        }
        usingAbility = false;
    }

    IEnumerator blindSpray()
    {
        usingAbility = true;
        for (int i = 0; i < blindSprayShots; i++)
        {
            for (int s = 0; s < 6; s++) spawnProjectile(scatterlaser, longlaserGuns[Random.Range(0, longlaserGuns.Length)].position, new Vector3(90, 90, -90), scatterlaserSpread, scatterlaserDamage, scatterlaserSpeed, true);
            if (audioSource)
            {
                if (blindSprayFireSound)
                {
                    audioSource.PlayOneShot(blindSprayFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        usingAbility = false;
    }

    IEnumerator antiarmorMissiles()
    {
        usingAbility = true;
        for (int i = 0; i < AAMissilesShots; i++)
        {
            spawnProjectile(shipkillerMissile, antiArmorMissilesGuns[Random.Range(0, antiArmorMissilesGuns.Length)].position, new Vector3(90, 0, 0), 1.5f, shipkillerDamage, shipkillerSpeed, true);
            if (audioSource)
            {
                if (AAMissilesFireSound)
                {
                    audioSource.PlayOneShot(AAMissilesFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(AAMissilesFireRate);
        }
        usingAbility = false;
    }

    void chaosOrb()
    {
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            Instantiate(chaosOrbObject, chaosOrbGuns[Random.Range(0, chaosOrbGuns.Length)].position, Quaternion.Euler(0, 0, 0));
        } else //Nightmare
        {
            foreach (Transform gun in chaosOrbGuns) Instantiate(chaosOrbObject, gun.position, Quaternion.Euler(0, 0, 0));
        }
        if (audioSource)
        {
            if (chaosOrbFireSound)
            {
                audioSource.PlayOneShot(chaosOrbFireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }

    IEnumerator protectiveShield()
    {
        if (!shielded)
        {
            shielded = true;
            shield.gameObject.SetActive(true);
            shield.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.08f, 0.08f, 0.08f), 5);
            enemyHealth.invulnerable = true;
            yield return new WaitForSeconds(protectiveShieldDuration);
            shielded = false;
            timeTillShieldUse = Random.Range(15, 20);
            shield.gameObject.SetActive(false);
            shield.localScale = Vector3.Lerp(new Vector3(0.08f, 0.08f, 0.08f), Vector3.zero, 5);
            enemyHealth.invulnerable = false;
        } else
        {
            yield break;
        }
    }
    #endregion
}