using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public long health = 100;
    [SerializeField] private long damage = 10;
    [SerializeField] private float RPM = 200;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float yMin = -6.75f, yMax = 2;

    [Header("Setup")]
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private AudioClip fireSound = null;

    private AudioSource audioSource;
    private Slider healthBar;
    private float nextShot = 0;
    private Vector3 screenBounds = Vector3.zero;
    private float width = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Slider[] healthbars = FindObjectsOfType<Slider>();
        foreach (Slider slider in healthbars)
        {
            if (slider.CompareTag("HealthBar")) healthBar = slider;
        }
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        width = GetComponent<Collider>().bounds.extents.x;
        if (PlayerPrefs.HasKey("HealthMultiplier")) health = (long)(health * PlayerPrefs.GetFloat("HealthMultiplier"));
        if (PlayerPrefs.HasKey("DamageMultiplier")) damage = (long)(damage * PlayerPrefs.GetFloat("DamageMultiplier"));
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
            if (!GameController.instance.gameOver && !GameController.instance.won)
            {
                GameController.instance.gameOver = true;
                GameController.instance.deathMessageToShow = "Your spaceship was destroyed!";
            }
            Destroy(gameObject);
        }
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Touch touch = Input.touches[0];
                moveHorizontal = touch.deltaPosition.x;
                moveVertical = touch.deltaPosition.y;
            }
            if ((moveHorizontal > 0 || moveVertical > 0) || (moveHorizontal < 0 || moveVertical < 0))
            {
                Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0).normalized * speed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;
                Vector3 cameraViewPoint = Camera.main.WorldToViewportPoint(newPosition);
                cameraViewPoint.x = Mathf.Clamp(cameraViewPoint.x, 0.04f, 0.96f);
                cameraViewPoint.y = Mathf.Clamp(cameraViewPoint.y, 0.07f, 0.93f);
                Vector3 finalPosition = Camera.main.ViewportToWorldPoint(cameraViewPoint);
                transform.position = finalPosition;
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), Mathf.Clamp(transform.position.y, yMin, yMax), 0);
            }
            if (Input.GetButton("Shoot") && Time.time >= nextShot)
            {
                bool foundBulletSpawns = false;
                nextShot = Time.time + 60 / RPM;
                foreach (Transform bulletSpawn in transform)
                {
                    if (bulletSpawn.CompareTag("BulletSpawn"))
                    {
                        GameObject newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                        newBullet.GetComponent<BulletHit>().damage = damage;
                        foundBulletSpawns = true;
                    }
                }
                if (!foundBulletSpawns)
                {
                    GameObject newBullet = Instantiate(bullet, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 0));
                    if (newBullet.transform.rotation.y != 90) newBullet.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    newBullet.GetComponent<BulletHit>().damage = damage;
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
        }
        if (damage < 0) damage = 1; //Checks if damage is below 1
        if (speed < 0) speed = 0; //Checks if speed is below 0
    }

    public void takeDamage(long hitDamage)
    {
        if (hitDamage > 0)
        {
            health -= hitDamage;
        } else
        {
            --health;
        }
    }
}
