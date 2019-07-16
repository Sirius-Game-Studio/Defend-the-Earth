using System.Collections;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [SerializeField] private long damage = 1;
    [SerializeField] private float RPM = 50;
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private AudioClip fireSound = null;

    private AudioSource audioSource;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("Difficulty") <= 1)
        {
            damage = (long)(damage * 0.75);
            RPM *= 0.9f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            damage = (long)(damage * 1.15);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            damage = (long)(damage * 1.3);
            RPM *= 1.05f;
        }
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && Time.time >= nextShot)
        {
            bool foundBulletSpawns = false;
            nextShot = Time.time + 60 / RPM;
            foreach (Transform bulletSpawn in transform)
            {
                if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                {
                    GameObject newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                    newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                    newBullet.GetComponent<EnemyHit>().damage = damage;
                    foundBulletSpawns = true;
                }
            }
            if (!foundBulletSpawns)
            {
                GameObject newBullet = Instantiate(bullet, transform.position - new Vector3(0, 1, 0), transform.rotation);
                newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                if (newBullet.transform.rotation.x != 90) newBullet.transform.rotation = Quaternion.Euler(90, 0, 0);
                newBullet.GetComponent<EnemyHit>().damage = damage;
                foundBulletSpawns = true;
            }
            if (audioSource && foundBulletSpawns)
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
        if (damage < 1) damage = 1; //Checks if damage is below 1
    }
}
