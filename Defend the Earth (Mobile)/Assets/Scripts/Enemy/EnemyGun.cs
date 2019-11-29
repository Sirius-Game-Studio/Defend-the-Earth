using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Easy, Normal and Hard only.")] public long damage = 10;
    [Tooltip("Overrides bullet damage if nightmareBullet is set (Nightmare only).")] [SerializeField] private long nightmareDamage = 20;
    [SerializeField] private float spreadDegree = 0;
    [SerializeField] private int shots = 1;
    [SerializeField] private bool turnToPlayer = false;
    public float RPM = 50;

    [Header("Projectile Texture")]
    [SerializeField] private bool useOnNightmare = false;
    [SerializeField] private Texture[] textures = new Texture[0];

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;

    [Header("Setup")]
    [Tooltip("Easy, Normal and Hard only.")] [SerializeField] private GameObject bullet = null;
    [Tooltip("Overrides bullet to fire if set to a GameObject (Nightmare only).")] [SerializeField] private GameObject nightmareBullet = null;

    private AudioSource audioSource;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nextShot = Time.time + 60 / RPM;
        if (GameController.instance.gamemode == GameController.Gamemodes.Campaign)
        {
            if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
            {
                damage = (long)(damage * 0.8);
                RPM *= 0.85f;
            } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
            {
                damage = (long)(damage * 1.15);
            } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
            {
                if (nightmareBullet) damage = nightmareDamage;
                damage = (long)(damage * 1.3);
                RPM *= 1.05f;
            }
        } else if (GameController.instance.gamemode == GameController.Gamemodes.Endless)
        {
            if (GameController.instance.wavesCleared > 0)
            {
                float multiplier = 1;
                for (long i = 0; i < GameController.instance.wavesCleared; i++) multiplier += 0.05f;
                if (multiplier > 1.5f) multiplier = 1.5f;
                damage = (long)(damage * multiplier);
            }
        }
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && Time.time >= nextShot)
        {
            SkinPicker skinPicker = GetComponent<SkinPicker>();
            bool foundBulletSpawns = false;
            nextShot = Time.time + 60 / RPM;
            for (int i = 0; i < shots; i++)
            {
                foreach (Transform bulletSpawn in transform)
                {
                    if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                    {
                        GameObject newBullet;
                        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal, Hard
                        {
                            newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                        } else
                        {
                            if (nightmareBullet)
                            {
                                newBullet = Instantiate(nightmareBullet, bulletSpawn.position, bulletSpawn.rotation);
                            } else
                            {
                                newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                            }
                        }
                        newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                        if (turnToPlayer && GameObject.FindWithTag("Player")) newBullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
                        if (spreadDegree != 0) newBullet.transform.Rotate(0, Random.Range(-spreadDegree, spreadDegree), 0);
                        newBullet.GetComponent<EnemyHit>().damage = damage;
                        if (PlayerPrefs.GetInt("Difficulty") < 4) //Easy, Normal and Hard
                        {
                            if (skinPicker && textures.Length > 0) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[skinPicker.texture]);
                        } else //Nightmare
                        {
                            if (nightmareBullet)
                            {
                                if (useOnNightmare && skinPicker && textures.Length > 0) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[skinPicker.texture]);
                            } else
                            {
                                if (skinPicker && textures.Length > 0) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[skinPicker.texture]);
                            }
                        }
                        foundBulletSpawns = true;
                    }
                }
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
        if (damage < 1) damage = 1; //Checks if damage is less than 1
        if (shots < 1) shots = 1; //Checks if amount of shots fired is less than 1
    }
}