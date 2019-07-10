using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int health = 30;
    [SerializeField] private int damage = 5;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float speed = 6.5f;

    [Header("Setup")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip fireSound;

    private AudioSource audioSource;
    private Slider healthBar;
    private GameController gameController;
    private float nextShot = 0;
    private Vector3 screenBounds = Vector3.zero;
    private float width = 0;
    private Slider[] sliders;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.CompareTag("HealthBar")) healthBar = slider;
        }
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        width = GetComponent<Collider>().bounds.extents.x;
        gameController = FindObjectOfType<GameController>();
        if (PlayerPrefs.HasKey("HealthMultiplier")) health = (int)(health * PlayerPrefs.GetFloat("HealthMultiplier"));
        if (PlayerPrefs.HasKey("DamageMultiplier")) damage = (int)(damage * PlayerPrefs.GetFloat("DamageMultiplier"));
        if (PlayerPrefs.HasKey("FireRateMultiplier")) fireRate *= PlayerPrefs.GetFloat("FireRateMultiplier");
        if (PlayerPrefs.HasKey("SpeedMultiplier")) speed *= PlayerPrefs.GetFloat("SpeedMultiplier");
        if (healthBar) healthBar.maxValue = health;
    }

    void Update()
    {
        if (health > healthBar.maxValue) health = (int)healthBar.maxValue;
        if (healthBar) healthBar.value = health;
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
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), Mathf.Clamp(transform.position.y, -7, 2), 0);
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

    public void takeDamage(int damage)
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
