using System.Collections;
using UnityEngine;

public class ChaosOrb : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Chaos Orb movement speed.")] [SerializeField] private float orbSpeed = 1;
    [Tooltip("How long Chaos Orb lasts for.")] [SerializeField] private float time = 3;
    [Tooltip("Projectile damage.")] public long damage = 22;
    [Tooltip("Projectile speed.")] public float speed = 17.5f;

    [Header("Setup")]
    [SerializeField] private GameObject sound = null;
    [SerializeField] private GameObject orb = null;
    [SerializeField] private GameObject split = null;

    void Start()
    {
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            time *= 1.1f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            orbSpeed *= 0.95f;
        } else if (PlayerPrefs.GetInt("Diffiiculty") >= 4) //Nightmare
        {
            orbSpeed *= 0.9f;
        }
        StartCoroutine(spreadingLight());
    }

    void Update()
    {
        if (time > 0)
        {
            transform.position += Vector3.up * orbSpeed * Time.deltaTime;
            time -= Time.deltaTime;
        } else
        {
            GameObject newSplit = Instantiate(split, transform.position, Quaternion.Euler(0, 0, 0));
            foreach (Transform projectile in newSplit.transform)
            {
                if (projectile.CompareTag("Projectile") && projectile.GetComponent<EnemyHit>() && projectile.GetComponent<Mover>())
                {
                    projectile.GetComponent<EnemyHit>().damage = damage;
                    projectile.GetComponent<Mover>().speed = speed;
                }
            }
            if (sound) Instantiate(sound, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    IEnumerator spreadingLight()
    {
        while (true)
        {
            if (PlayerPrefs.GetInt("Difficulty") <= 2) //Easy and Normal
            {
                yield return new WaitForSeconds(0.25f);
            } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
            {
                yield return new WaitForSeconds(0.2f);
            } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
            {
                yield return new WaitForSeconds(0.15f);
            }
            Instantiate(orb, transform.position, Quaternion.Euler(Random.Range(0, 180), 90, -90));
        }
    }
}
