using System.Collections;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Tooltip("Amount of damage dealt to the player.")] [SerializeField] private long damage = 3;
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
            yield return new WaitForSeconds(Random.Range(1, 3));
            if (!gameController.gameOver)
            {
                GameObject newBullet = Instantiate(bullet, transform.position + new Vector3(0, -1, 0), transform.rotation);
                newBullet.GetComponent<EnemyBulletHit>().damage = damage;
                if (newBullet.transform.rotation.x != -90) newBullet.transform.rotation = Quaternion.Euler(-90, 0, 0);
                if (audioSource && fireSound) audioSource.PlayOneShot(fireSound);
            }
        }
    }
}
