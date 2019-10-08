using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Range(50, 250)] public long health = 100;
    [Range(5, 30)] [SerializeField] private long damage = 10;
    [Range(100, 300)] [SerializeField] private float RPM = 150;
    [SerializeField] private float speed = 8;

    [Header("Powerup Settings")]
    [Tooltip("Amount of health restored by Small Repairs.")] [Range(1, 15)] public long smallRepairHeal = 15;
    [Tooltip("Amount of health restored by Large Repairs.")] [Range(1, 25)] public long largeRepairHeal = 25;
    [Tooltip("Supercharge damage multiplier.")] [Range(1.05f, 2)] [SerializeField] private float superchargeMultiplier = 1.5f;
    [Tooltip("Supercharge powerup duration.")] [Range(5, 15)] [SerializeField] private float superchargeTime = 12;

    [Header("Boundary Settings")]
    [SerializeField] private float yMin = -6.75f;
    [SerializeField] private float yMax = 2;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private AudioSource lowHealthSound = null;
    [SerializeField] private GameObject textPopup = null;

    private new Renderer renderer;
    private AudioSource audioSource;
    private Controls input;
    private Slider healthBar;
    private Text healthText;
    private Text livesCount;
    private long maxHealth = 0;
    [HideInInspector] public long lives = 3;
    [HideInInspector] public bool invulnerable = false;
    private bool shooting = false;
    private Vector2 movement;
    private bool hasSupercharge = false;
    private float superchargeDuration = 0;
    private long oldDamage = 0;
    private float nextShot = 0;
    private bool shownSuperchargeText = false;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        //Gets all sliders and text objects tagged as HealthBar, then sets health bar and health text
        foreach (Slider slider in FindObjectsOfType<Slider>())
        {
            if (slider.CompareTag("HealthBar")) healthBar = slider;
        }
        foreach (Text text in FindObjectsOfType<Text>())
        {
            if (text.CompareTag("HealthBar")) healthText = text;
        }

        //Gets all text objects tagged as LivesCount, then sets lives count
        foreach (Text text in FindObjectsOfType<Text>())
        {
            if (text.CompareTag("LivesCount")) livesCount = text;
        }

        if (healthBar && healthBar.maxValue != health) healthBar.maxValue = health;
        if (GameController.instance.isCampaignLevel)
        {
            lives = 1;
        } else
        {
            lives = 3;
        }
        if (PlayerPrefs.HasKey("HealthMultiplier")) health = (long)(health * PlayerPrefs.GetFloat("HealthMultiplier"));
        if (PlayerPrefs.HasKey("DamageMultiplier")) damage = (long)(damage * PlayerPrefs.GetFloat("DamageMultiplier"));
        if (PlayerPrefs.HasKey("SpeedMultiplier")) speed *= PlayerPrefs.GetFloat("SpeedMultiplier");
        maxHealth = health;
        oldDamage = damage;
        if (oldDamage < 1) oldDamage = 1;
        superchargeDuration = superchargeTime;
        if (healthBar) healthBar.maxValue = maxHealth;
        if (healthText) healthText.text = health + " / " + maxHealth;
    }

    void Awake()
    {
        input = new Controls();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += context => move(context.ReadValue<Vector2>());
        input.Player.Fire.performed += context => fire(true);
        input.Player.Move.canceled += context => move(Vector2.zero);
        input.Player.Fire.canceled += context => fire(false);

        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        input.Debug.SmallRepair.performed += context => repair(smallRepairHeal);
        input.Debug.LargeRepair.performed += context => repair(largeRepairHeal);
        input.Debug.MaxHealth.performed += context => repair(maxHealth);
        input.Debug.Supercharge.performed += context => supercharge();
        #endif
    }

    void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= context => move(context.ReadValue<Vector2>());
        input.Player.Fire.performed -= context => fire(true);
        input.Player.Move.canceled -= context => move(Vector2.zero);
        input.Player.Fire.canceled -= context => fire(false);

        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        input.Debug.SmallRepair.performed -= context => repair(smallRepairHeal);
        input.Debug.LargeRepair.performed -= context => repair(largeRepairHeal);
        input.Debug.MaxHealth.performed -= context => repair(maxHealth);
        input.Debug.Supercharge.performed -= context => supercharge();
        #endif
    }

    void Update()
    {
        if (health < 0) //Checks if health is less than 0
        {
            health = 0;
        } else if (health > maxHealth) //Checks if health is more than the maximum
        {
            health = maxHealth;
        }
        if (lives < 0) lives = 0; //Checks if lives are less than 0
        if (healthBar) healthBar.value = health;
        if (healthText) healthText.text = health + " / " + maxHealth;
        if (livesCount)
        {
            livesCount.text = "Lives: " + lives;
            if (!GameController.instance.currentBoss)
            {
                livesCount.rectTransform.anchoredPosition = new Vector2(0, 10);
            } else
            {
                livesCount.rectTransform.anchoredPosition = new Vector2(0, 70);
            }
            if (GameController.instance.isCampaignLevel)
            {
                livesCount.enabled = false;
            } else
            {
                livesCount.enabled = true;
            }
        }
        if (health <= 0)
        {
            if (GameController.instance.isCampaignLevel)
            {
                lives = 0;
            } else
            {
                if (lives > 1)
                {
                    if (explosion) Instantiate(explosion, transform.position, transform.rotation);
                    --lives;
                    health = maxHealth;
                    startInvulnerability(3);
                } else
                {
                    lives = 0;
                }
            }
        }
        if (lives <= 0)
        {
            if (explosion) Instantiate(explosion, transform.position, transform.rotation);
            health = 0;
            if (!GameController.instance.gameOver && !GameController.instance.won)
            {
                GameController.instance.gameOver = true;
                GameController.instance.deathMessageToShow = "Your spaceship has been destroyed!";
            }
            Destroy(gameObject);
        }
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused)
        {
            transform.position += new Vector3(movement.x, movement.y, 0).normalized * speed * Time.deltaTime;
            if (shooting && Time.time >= nextShot)
            {
                bool foundBulletSpawns = false;
                nextShot = Time.time + 60 / RPM;
                foreach (Transform bulletSpawn in transform)
                {
                    if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                    {
                        GameObject newBullet = Instantiate(bullet, new Vector3(bulletSpawn.position.x, bulletSpawn.position.y, 0), bulletSpawn.rotation);
                        newBullet.GetComponent<Bullet>().damage = damage;
                        foundBulletSpawns = true;
                    }
                }
                if (!foundBulletSpawns)
                {
                    GameObject newBullet = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.Euler(-90, 0, 0));
                    if (newBullet.transform.rotation.y != 90) newBullet.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    newBullet.GetComponent<Bullet>().damage = damage;
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
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float width = GetComponent<Collider>().bounds.extents.x;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), Mathf.Clamp(transform.position.y, yMin, yMax), 0);
        if (lowHealthSound)
        {
            if (health > maxHealth * 0.25)
            {
                lowHealthSound.Stop();
            } else
            {
                if (!lowHealthSound.isPlaying) lowHealthSound.Play();
            }
        }
        if (hasSupercharge)
        {
            if (superchargeDuration > 0)
            {
                superchargeDuration -= Time.deltaTime;
                if (superchargeDuration <= 3 && !shownSuperchargeText)
                {
                    if (textPopup)
                    {
                        if (textPopup.GetComponent<TextMeshPro>())
                        {
                            shownSuperchargeText = true;
                            GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                            popup.GetComponent<TextMeshPro>().text = "Supercharge is wearing off!";
                            popup.GetComponent<TextMeshPro>().color = new Color32(0, 150, 150, 255);
                            popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 75, 75, 255);
                        } else
                        {
                            Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                        }
                    }
                }
            } else
            {
                hasSupercharge = false;
                damage = oldDamage;
                superchargeDuration = superchargeTime;
            }
        }
        if (damage < 1) damage = 1; //Checks if damage is less than 1
        if (speed < 0) speed = 0; //Checks if speed is less than 0
    }

    #region Input Functions
    void move(Vector2 direction)
    {
        movement = direction;
    }

    void fire(bool state)
    {
        shooting = state;
    }
    #endregion

    #region Main Functions
    public void takeDamage(long hitDamage)
    {
        if (!invulnerable)
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

    public void startInvulnerability(float duration)
    {
        if (!invulnerable && duration > 0)
        {
            invulnerable = true;
            StartCoroutine(invulnerabilityFadeEffect());
            Invoke("stopInvulnerability", duration);
        }
    }

    void stopInvulnerability()
    {
        invulnerable = false;
        if (renderer) renderer.enabled = true;
        foreach (Renderer child in GetComponentsInChildren<Renderer>())
        {
            if (child) child.enabled = true;
        }
    }

    IEnumerator invulnerabilityFadeEffect()
    {
        if (invulnerable)
        {
            while (invulnerable)
            {
                if (renderer) renderer.enabled = false;
                foreach (Renderer child in GetComponentsInChildren<Renderer>())
                {
                    if (child) child.enabled = false;
                }
                yield return new WaitForSeconds(0.1f);
                if (renderer) renderer.enabled = true;
                foreach (Renderer child in GetComponentsInChildren<Renderer>())
                {
                    if (child) child.enabled = true;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (renderer) renderer.enabled = true;
            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                if (child) child.enabled = true;
            }
        } else
        {
            if (renderer) renderer.enabled = true;
            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                if (child) child.enabled = true;
            }
            yield break;
        }
    }
    #endregion

    #region Powerup Functions
    public void repair(long heal)
    {
        if (heal != 0)
        {
            health += heal;
            if (textPopup)
            {
                if (textPopup.GetComponent<TextMeshPro>())
                {
                    GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                    if (heal > 0)
                    {
                        popup.GetComponent<TextMeshPro>().text = "+" + heal;
                        popup.GetComponent<TextMeshPro>().color = new Color32(0, 255, 0, 255);
                        popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 127, 0, 255);
                    } else
                    {
                        popup.GetComponent<TextMeshPro>().text = "-" + heal;
                        popup.GetComponent<TextMeshPro>().color = new Color32(255, 0, 0, 255);
                        popup.GetComponent<TextMeshPro>().outlineColor = new Color32(127, 0, 0, 255);
                    }
                } else
                {
                    Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                }
            }
        }
    }

    public void supercharge()
    {
        if (!hasSupercharge)
        {
            hasSupercharge = true;
            oldDamage = damage;
            if (oldDamage < 1) oldDamage = 1;
            damage = (long)(damage * superchargeMultiplier);
            superchargeDuration = superchargeTime;
            shownSuperchargeText = false;
        } else
        {
            superchargeDuration = superchargeTime;
            shownSuperchargeText = false;
        }
        if (textPopup)
        {
            if (textPopup.GetComponent<TextMeshPro>())
            {
                GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                popup.GetComponent<TextMeshPro>().text = "Supercharge!";
                popup.GetComponent<TextMeshPro>().color = new Color32(0, 255, 255, 255);
                popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 127, 127, 255);
            } else
            {
                Debug.LogError("TextPopup object does not have a TextMeshPro component!");
            }
        }
    }
    #endregion
}