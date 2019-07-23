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
    private Text healthText;
    private long maxHealth = 0;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //Gets sliders and texts tagged as HealthBar, then sets health bar and health text
        foreach (Slider slider in FindObjectsOfType<Slider>())
        {
            if (slider.CompareTag("HealthBar")) healthBar = slider;
        }
        foreach (Text text in FindObjectsOfType<Text>())
        {
            if (text.CompareTag("HealthBar")) healthText = text;
        }

        if (healthBar && healthBar.maxValue != health) healthBar.maxValue = health;
        if (PlayerPrefs.HasKey("HealthMultiplier")) health = (long)(health * PlayerPrefs.GetFloat("HealthMultiplier"));
        if (PlayerPrefs.HasKey("DamageMultiplier")) damage = (long)(damage * PlayerPrefs.GetFloat("DamageMultiplier"));
        if (PlayerPrefs.HasKey("SpeedMultiplier")) speed *= PlayerPrefs.GetFloat("SpeedMultiplier");
        maxHealth = health;
        if (healthBar) healthBar.maxValue = maxHealth;
        if (healthText) healthText.text = health + " / " + maxHealth;
    }

    void Update()
    {
        if (health < 0) //Checks if health is below 0
        {
            health = 0;
        } else if (health > maxHealth) //Checks if health is above the maximum
        {
            health = maxHealth;
        }
        if (healthBar) healthBar.value = health;
        if (healthText) healthText.text = health + " / " + maxHealth;
        if (health <= 0)
        {
            if (explosion)
            {
                GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
                if (newExplosion.GetComponent<AudioSource>()) newExplosion.GetComponent<AudioSource>().volume = getVolumeData(true);
            }
            if (!GameController.instance.gameOver && !GameController.instance.won)
            {
                GameController.instance.gameOver = true;
                GameController.instance.deathMessageToShow = "Your spaceship has been destroyed!";
            }
            Destroy(gameObject);
        }
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused)
        {
            Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            float width = GetComponent<Collider>().bounds.extents.x;
            transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized * speed * Time.deltaTime;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), Mathf.Clamp(transform.position.y, yMin, yMax), 0);
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
                        audioSource.PlayOneShot(fireSound, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
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

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }
}
