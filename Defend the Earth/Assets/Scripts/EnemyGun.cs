using System.Collections;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Tooltip("Amount of damage dealt to the player.")] [SerializeField] private int damage = 1;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioClip fireSound;

    private AudioSource audioSource;
    private GameController gameController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<GameController>();
        StartCoroutine(shoot());
    }

    void Update()
    {
        if (damage < 1) damage = 1; //Checks if damage is below 1
    }

    IEnumerator shoot()
    {
        while (!gameController.gameOver)
        {
            yield return new WaitForSeconds(fireRate);
            if (!gameController.gameOver)
            {
                bool foundBulletSpawns = false;
                foreach (Transform bulletSpawn in transform)
                {
                    if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                    {
                        GameObject newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                        newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                        newBullet.GetComponent<EnemyBulletHit>().damage = damage;
                        foundBulletSpawns = true;
                    }
                }
                if (!foundBulletSpawns)
                {
                    GameObject newBullet = Instantiate(bullet, transform.position - new Vector3(0, 1, 0), transform.rotation);
                    newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                    if (newBullet.transform.rotation.x != 90) newBullet.transform.rotation = Quaternion.Euler(90, 0, 0);
                    newBullet.GetComponent<EnemyBulletHit>().damage = damage;
                    foundBulletSpawns = true;
                }
                if (foundBulletSpawns && audioSource && fireSound) audioSource.PlayOneShot(fireSound);
            }
        }
    }
}
