using System.Collections;
using UnityEngine;

public class ChaosOrb : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Chaos Orb fire rate.")] [SerializeField] private float orbFireRate = 0.25f;
    [Tooltip("Chaos Orb movement speed.")] [SerializeField] private float orbSpeed = 1;
    [Tooltip("Chaos Orb lifetime.")] [SerializeField] private float lifetime = 3;
    [Tooltip("Projectile damage.")] public long damage = 20;
    [Tooltip("Projectile speed.")] public float speed = 17.5f;

    [Header("Setup")]
    [SerializeField] private GameObject sound = null;
    [SerializeField] private GameObject orb = null;
    [SerializeField] private GameObject miniOrb = null;

    void Start()
    {
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            lifetime *= 1.1f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            orbFireRate *= 0.8f;
            orbSpeed *= 0.95f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            orbFireRate *= 0.6f;
            orbSpeed *= 0.9f;
        }
        StartCoroutine(spreadingLight());
    }

    void Update()
    {
        if (lifetime > 0)
        {
            transform.position += Vector3.up * orbSpeed * Time.deltaTime;
            lifetime -= Time.deltaTime;
        } else
        {
            float angle = 0;
            for (int i = 0; i < 24; i++)
            {
                GameObject newOrb = Instantiate(orb, transform.position, Quaternion.Euler(angle, 90, -90));
                newOrb.GetComponent<EnemyHit>().damage = damage;
                newOrb.GetComponent<Mover>().speed = speed;
                angle += 15;
            }
            if (sound) Instantiate(sound, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    IEnumerator spreadingLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(orbFireRate);
            Instantiate(miniOrb, transform.position, Quaternion.Euler(Random.Range(0, 180), 90, -90));
        }
    }
}