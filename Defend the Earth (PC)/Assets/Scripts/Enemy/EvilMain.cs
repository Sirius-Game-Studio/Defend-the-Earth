using System.Collections;
using UnityEngine;

public class EvilMain : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3, 3.5f);
    [Tooltip("The Y position this enemy stops at.")] [SerializeField] private float yPosition = 3;
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Perforating Cannons")]
    [SerializeField] private long longlaserDamage = 17;
    [SerializeField] private float longlaserSpeed = 14;
    [SerializeField] private float perforatingCannonsFireRate = 0.2f;
    [SerializeField] private int perforatingCannonsShots = 8;

    [Header("Dying Craft")]
    [SerializeField] private long shipkillerDamage = 31;
    [SerializeField] private float shipkillerSpeed = 17.5f;
    [SerializeField] private float shipkillerSpread = 4;
    [SerializeField] private float dyingCraftFireRate = 0.25f;
    [SerializeField] private int dyingCraftShots = 10;

    [Header("Bombastic Spray")]
    [SerializeField] private long scatterlaserDamage = 12;
    [SerializeField] private float scatterlaserSpeed = 17;
    [SerializeField] private float scatterlaserSpread = 8.5f;
    [SerializeField] private int scatterlaserShots = 12;
    [SerializeField] private float bombasticSprayFireRate = 0.3f;
    [SerializeField] private int bombasticSprayShots = 4;

    [Header("Spreading Rays")]
    [SerializeField] private float longlaserSpread = 8;
    [SerializeField] private float spreadingRaysFireRate = 0.25f;
    [SerializeField] private int spreadingRaysShots = 12;

    [Header("Battering Charge")]
    [SerializeField] private long superlaserDamage = 32;
    [SerializeField] private float superlaserSpeed = 15;
    [SerializeField] private float batteringChargeFireRate = 1;
    [SerializeField] private int batteringChargeShots = 3;

    [Header("Sphericling Demon")]
    [SerializeField] private long orbDamage = 16;
    [SerializeField] private float orbSpeed = 16;
    [Tooltip("Nightmare only.")] [SerializeField] private float orbLifesteal = 0.2f;
    [SerializeField] private float sphericlingDemonFireRate = 0.2f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip perforatingCannonsFireSound = null;
    [SerializeField] private AudioClip dyingCraftFireSound = null;
    [SerializeField] private AudioClip bombasticSprayFireSound = null;
    [SerializeField] private AudioClip spreadingRaysFireSound = null;
    [SerializeField] private AudioClip batteringChargeFireSound = null;
    [SerializeField] private AudioClip sphericlingDemonFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject longlaser = null;
    [SerializeField] private GameObject scatterlaser = null;
    [SerializeField] private GameObject laser = null;
    [SerializeField] private GameObject superlaser = null;
    [SerializeField] private GameObject shipkillerMissile = null;
    [SerializeField] private GameObject miniOrb = null;
    [SerializeField] private Transform[] perforatingCannonsGuns = new Transform[0];
    [SerializeField] private Transform dyingCraftGun = null;
    [SerializeField] private Transform[] chargeGlows = new Transform[0];

    private AudioSource audioSource;
    private EnemyHealth enemyHealth;
    private bool usingAbility = false;
    private bool animatingCharge = false;

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
            superlaserDamage = (long)(superlaserDamage * 0.9);
            shipkillerDamage = (long)(shipkillerDamage * 0.9);
            orbDamage = (long)(orbDamage * 0.9);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            longlaserDamage = (long)(longlaserDamage * 1.2);
            longlaserSpeed *= 1.05f;
            longlaserSpread *= 1.1f;
            scatterlaserDamage = (long)(scatterlaserDamage * 1.2);
            scatterlaserSpeed *= 1.05f;
            scatterlaserSpread *= 1.1f;
            superlaserDamage = (long)(superlaserDamage * 1.2);
            superlaserSpeed *= 1.05f;
            shipkillerDamage = (long)(shipkillerDamage * 1.2);
            shipkillerSpeed *= 1.05f;
            shipkillerSpread *= 1.25f;
            orbDamage = (long)(orbDamage * 1.2);
            orbSpeed *= 1.05f;
            scatterlaserShots = (int)(scatterlaserShots * 1.25);
            bombasticSprayShots = (int)(bombasticSprayShots * 1.25);
            spreadingRaysFireRate *= 0.95f;
            spreadingRaysShots = (int)(spreadingRaysShots * 1.25);
            batteringChargeFireRate *= 0.85f;
            ++batteringChargeShots;
            sphericlingDemonFireRate *= 0.95f;
            abilityTime -= new Vector2(0, 0.25f);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            longlaserDamage = (long)(longlaserDamage * 1.4);
            longlaserSpeed *= 1.1f;
            longlaserSpread *= 1.2f;
            scatterlaserDamage = (long)(scatterlaserDamage * 1.4);
            scatterlaserSpeed *= 1.1f;
            scatterlaserSpread *= 1.2f;
            superlaserDamage = (long)(superlaserDamage * 1.4);
            superlaserSpeed *= 1.1f;
            shipkillerDamage = (long)(shipkillerDamage * 1.4);
            shipkillerSpeed *= 1.1f;
            shipkillerSpread *= 1.5f;
            orbDamage = (long)(orbDamage * 1.4);
            orbSpeed *= 1.1f;
            perforatingCannonsShots = (int)(perforatingCannonsShots * 1.25);
            dyingCraftShots = (int)(dyingCraftShots * 1.5);
            scatterlaserShots = (int)(scatterlaserShots * 1.25);
            bombasticSprayShots = (int)(bombasticSprayShots * 1.5);
            spreadingRaysFireRate *= 0.9f;
            spreadingRaysShots = (int)(spreadingRaysShots * 1.5);
            batteringChargeFireRate *= 0.7f;
            batteringChargeShots += 2;
            sphericlingDemonFireRate *= 0.9f;
            abilityTime -= new Vector2(0.25f, 0.25f);
        }
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        StartCoroutine(main());
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
                    if (random <= 0.15f) //Sphericling Demon (15% chance)
                    {
                        StartCoroutine(sphericlingDemon());
                    } else if (random <= 0.3f) //Battering Charge (15% chance)
                    {
                        StartCoroutine(batteringCharge());
                    } else if (random <= 0.45f) //Spreading Rays (15% chance)
                    {
                        StartCoroutine(spreadingRays());
                    } else if (random <= 0.6f) //Bombastic Spray (15% chance)
                    {
                        StartCoroutine(bombasticSpray());
                    } else if (random <= 0.8f) //Dying Craft (20% chance)
                    {
                        StartCoroutine(dyingCraft());
                    } else //Perforating Cannons (20% chance)
                    {
                        StartCoroutine(perforatingCannons());
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

    #region Ability Functions
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

    IEnumerator dyingCraft()
    {
        usingAbility = true;
        for (int i = 0; i < dyingCraftShots; i++)
        {
            spawnProjectile(shipkillerMissile, dyingCraftGun.position, new Vector3(90, 0, 0), shipkillerSpread, shipkillerDamage, shipkillerSpeed, true);
            if (audioSource)
            {
                if (dyingCraftFireSound)
                {
                    audioSource.PlayOneShot(dyingCraftFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(dyingCraftFireRate);
        }
        usingAbility = false;
    }

    IEnumerator bombasticSpray()
    {
        usingAbility = true;
        for (int i = 0; i < bombasticSprayShots; i++)
        {
            for (int s = 0; s < scatterlaserShots; s++) spawnProjectile(scatterlaser, dyingCraftGun.position, new Vector3(90, 0, 0), scatterlaserSpread, scatterlaserDamage, scatterlaserSpeed, true);
            if (audioSource)
            {
                if (bombasticSprayFireSound)
                {
                    audioSource.PlayOneShot(bombasticSprayFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(bombasticSprayFireRate);
        }
        usingAbility = false;
    }

    IEnumerator spreadingRays()
    {
        usingAbility = true;
        for (int i = 0; i < spreadingRaysShots; i++)
        {
            spawnProjectile(longlaser, dyingCraftGun.position, new Vector3(90, 0, 0), longlaserSpread, (long)(longlaserDamage * 0.85), longlaserSpeed * 1.1f, true);
            if (audioSource)
            {
                if (spreadingRaysFireSound)
                {
                    audioSource.PlayOneShot(spreadingRaysFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(spreadingRaysFireRate);
        }
        float chargeSpeed;
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            chargeSpeed = 0.001f;
        } else //Nightmare
        {
            chargeSpeed = 0.002f;
        }
        StartCoroutine(animateChargeGlow(chargeGlows[2], chargeSpeed, 0.06f, true));
        while (animatingCharge) yield return null;
        StartCoroutine(animateChargeGlow(chargeGlows[2], 0.005f, 0, false));
        float fireRate = 0.15f;
        if (PlayerPrefs.GetFloat("Difficulty") == 3) //Hard
        {
            fireRate *= 0.95f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            fireRate *= 0.9f;
        }
        float angle = 0;
        for (int i = 0; i < 37; i++)
        {
            spawnProjectile(longlaser, dyingCraftGun.position, new Vector3(angle, 90, -90), 0, (long)(longlaserDamage * 0.85), longlaserSpeed * 1.1f, false);
            angle += 20;
            if (audioSource)
            {
                if (spreadingRaysFireSound)
                {
                    audioSource.PlayOneShot(spreadingRaysFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(fireRate);
        }
        usingAbility = false;
    }

    IEnumerator batteringCharge()
    {
        usingAbility = true;
        float chargeSpeed;
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            chargeSpeed = 0.001f;
        } else //Nightmare
        {
            chargeSpeed = 0.002f;
        }
        StartCoroutine(animateChargeGlow(chargeGlows[3], chargeSpeed, 0.1f, true));
        while (animatingCharge) yield return null;
        StartCoroutine(animateChargeGlow(chargeGlows[3], 0.005f, 0, false));
        for (int i = 0; i < batteringChargeShots; i++)
        {
            float angle = Random.Range(-180, 180);
            for (int s = 0; s < 12; s++)
            {
                spawnProjectile(superlaser, new Vector3(chargeGlows[3].position.x, chargeGlows[3].position.y, 0), new Vector3(angle, 90, -90), 0, superlaserDamage, superlaserSpeed, false);
                angle += 30;
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
            yield return new WaitForSeconds(batteringChargeFireRate);
        }
        usingAbility = false;
    }

    IEnumerator sphericlingDemon()
    {
        usingAbility = true;
        int shots;
        float chargeSpeed;
        int point;
        float random = Random.value;
        if (random <= 0.5f)
        {
            point = 0;
        } else
        {
            point = 1;
        }
        foreach (Transform chargeGlow in chargeGlows) chargeGlow.localScale = Vector3.zero;
        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
        {
            shots = 55;
            chargeSpeed = 0.001f;
        } else //Nightmare
        {
            shots = 91;
            chargeSpeed = 0.002f;
        }
        StartCoroutine(animateChargeGlow(chargeGlows[point], chargeSpeed, 0.1f, true));
        while (animatingCharge) yield return null;
        StartCoroutine(animateChargeGlow(chargeGlows[point], 0.005f, 0, false));
        float angle = 0;
        for (int i = 0; i < shots; i++)
        {
            GameObject orb = spawnProjectile(miniOrb, new Vector3(chargeGlows[point].position.x, chargeGlows[point].position.y, 0), new Vector3(angle, 90, -90), 0, orbDamage, orbSpeed, false);
            if (PlayerPrefs.GetInt("Difficulty") >= 4)
            {
                orb.GetComponent<EnemyHit>().enemyHealth = enemyHealth;
                if (orbLifesteal > 0) orb.GetComponent<EnemyHit>().lifesteal = orbLifesteal;
            }
            angle += 10;
            if (audioSource)
            {
                if (sphericlingDemonFireSound)
                {
                    audioSource.PlayOneShot(sphericlingDemonFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(sphericlingDemonFireRate);
        }
        usingAbility = false;
    }
    #endregion
}