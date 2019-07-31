using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private long damage = 1;
    [SerializeField] private float RPM = 50;

    [Header("Default Skin")]
    [Tooltip("Used only if SkinPicker is in this GameObject.")] [SerializeField] private Texture defaultAlbedo = null;
    [Tooltip("Used only if SkinPicker is in this GameObject.")] [SerializeField] private Texture greenAlbedo = null;
    [Tooltip("Used only if SkinPicker is in this GameObject.")] [SerializeField] private Texture whiteAlbedo = null;

    [Header("Setup")]
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private AudioClip fireSound = null;

    private AudioSource audioSource;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nextShot = Time.time + 60 / RPM;
        if (PlayerPrefs.GetInt("Difficulty") <= 1)
        {
            damage = (long)(damage * 0.8);
            RPM *= 0.85f;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            damage = (long)(damage * 1.15);
            RPM *= 1.05f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            damage = (long)(damage * 1.3);
            RPM *= 1.1f;
        }
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && Time.time >= nextShot)
        {
            SkinPicker skinPicker = GetComponent<SkinPicker>();
            bool foundBulletSpawns = false;
            nextShot = Time.time + 60 / RPM;
            foreach (Transform bulletSpawn in transform)
            {
                if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                {
                    GameObject newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                    newBullet.transform.position = new Vector3(newBullet.transform.position.x, newBullet.transform.position.y, 0);
                    if (skinPicker)
                    {
                        if (skinPicker.skin <= 1) //Default
                        {
                            if (defaultAlbedo) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", defaultAlbedo);
                        } else if (skinPicker.skin == 2) //Green
                        {
                            if (greenAlbedo) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", greenAlbedo);
                        } else if (skinPicker.skin >= 3) //White
                        {
                            if (whiteAlbedo) newBullet.GetComponent<Renderer>().material.SetTexture("_MainTex", whiteAlbedo);
                        }
                    }
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
                    audioSource.PlayOneShot(fireSound, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
        }
        if (damage < 1) damage = 1; //Checks if damage is below 1
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
