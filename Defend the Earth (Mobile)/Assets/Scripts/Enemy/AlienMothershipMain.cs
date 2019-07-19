using System.Collections;
using UnityEngine;

public class AlienMothershipMain : MonoBehaviour
{
    [Header("Ability Settings")]

    [Header("Torpedo Barrage")]
    [Tooltip("The amount of shots to fire.")] [SerializeField] private long torpedoBarrageShots = 12;
    [SerializeField] private float torpedoBarrageFireRate = 0.3f;

    [Header("Ability Objects")]
    [Tooltip("Required for Busted Shot ability.")] [SerializeField] private GameObject bustedShotObject = null;

    [Header("Setup")]
    [SerializeField] private long bulletDamage = 18;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private Transform[] bulletSpawns = new Transform[0];

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            bulletDamage = (long)(bulletDamage * 1.4);
            bulletSpeed = 12;
            torpedoBarrageShots = 16;
            torpedoBarrageFireRate = 0.225f;
        }
        StartCoroutine(main());
    }

    IEnumerator main()
    {
        while (!GameController.instance.gameOver && !GameController.instance.won)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused)
            {
                yield return new WaitForSeconds(Random.Range(4.5f, 5.5f));
                if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused)
                {
                    float random = Random.value;
                    if (random <= 0.35f) //Busted Shot (35% chance)
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
                    } else if (random <= 0.65f) //Torpedo Barrage (65% chance)
                    {
                        StartCoroutine(torpedoBarrage());
                    } else
                    {
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        GameObject torpedo1 = Instantiate(bullet, bulletSpawns[1].position, Quaternion.Euler(90, 0, 0));
                        if (player) torpedo1.transform.LookAt(player.transform);
                        torpedo1.GetComponent<EnemyHit>().damage = bulletDamage;
                        torpedo1.GetComponent<Mover>().speed = bulletSpeed;
                        GameObject torpedo2 = Instantiate(bullet, bulletSpawns[2].position, Quaternion.Euler(90, 0, 0));
                        if (player) torpedo2.transform.LookAt(player.transform);
                        torpedo2.GetComponent<EnemyHit>().damage = bulletDamage;
                        torpedo2.GetComponent<Mover>().speed = bulletSpeed;
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
                }
            } else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator torpedoBarrage()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < torpedoBarrageShots; i++)
        {
            if (PlayerPrefs.GetInt("Difficulty") < 4)
            {
                GameObject torpedo = Instantiate(bullet, bulletSpawns[Random.Range(1, 2)].position, Quaternion.Euler(90, 0, 0));
                if (player) torpedo.transform.LookAt(player.transform);
                torpedo.GetComponent<EnemyHit>().damage = bulletDamage;
                torpedo.GetComponent<Mover>().speed = bulletSpeed;
            } else
            {
                GameObject torpedo1 = Instantiate(bullet, bulletSpawns[1].position, Quaternion.Euler(90, 0, 0));
                if (player) torpedo1.transform.LookAt(player.transform);
                torpedo1.GetComponent<EnemyHit>().damage = bulletDamage;
                torpedo1.GetComponent<Mover>().speed = bulletSpeed;
                GameObject torpedo2 = Instantiate(bullet, bulletSpawns[2].position, Quaternion.Euler(90, 0, 0));
                if (player) torpedo2.transform.LookAt(player.transform);
                torpedo2.GetComponent<EnemyHit>().damage = bulletDamage;
                torpedo2.GetComponent<Mover>().speed = bulletSpeed;
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
    }
}
