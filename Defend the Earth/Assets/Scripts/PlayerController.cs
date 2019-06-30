using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private long health = 40;
    [SerializeField] private long damage = 5;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float speed = 8;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Setup")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private GameObject explosion;

    private AudioSource audioSource;
    private GameController gameController;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        healthBar.value = health;
        if (health <= 0)
        {
            if (explosion) Instantiate(explosion, transform.position, transform.rotation);
            gameController.gameOver = true;
            Destroy(gameObject);
        }
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
        movement = movement.normalized * speed * Time.deltaTime;
        transform.position += movement;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -15, 15), Mathf.Clamp(transform.position.y, -6.5f, 2), 0);
        if (Input.GetButton("Shoot") && nextShot >= fireRate)
        {
            nextShot = 0;
            foreach (Transform bulletSpawn in transform)
            {
                if (bulletSpawn.CompareTag("BulletSpawn"))
                {
                    GameObject newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                    newBullet.GetComponent<BulletHit>().damage = damage;
                }
            }
            if (audioSource && fireSound) audioSource.PlayOneShot(fireSound);
        }
        if (nextShot < fireRate) nextShot += Time.deltaTime;
        if (damage < 0) damage = 1; //Checks if damage is below 1
        if (fireRate < 0.175f) fireRate = 0.175f; //Checks if fire rate is below 0.175 seconds
        if (speed < 0) speed = 0; //Checks if speed is below 0
    }

    public void takeDamage(long damage)
    {
        if (damage > 0)
        {
            health -= damage;
        } else
        {
            --health;
        }
    }
}
