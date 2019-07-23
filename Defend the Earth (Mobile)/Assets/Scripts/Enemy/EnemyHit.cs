using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [Tooltip("Amount of damage dealt to players.")] public long damage = 1;
    [SerializeField] private GameObject explosion = null;

    private bool hit = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("Difficulty") <= 1)
        {
            damage = (long)(damage * 0.75);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            damage = (long)(damage * 1.15);
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4)
        {
            damage = (long)(damage * 1.3);
        }
    }

    void Update()
    {
        if (damage < 1) damage = 1; //Checks if damage is below 1
    }

    void OnTriggerStay(Collider other)
    {
        if (!hit && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.takeDamage(damage);
                if (explosion)
                {
                    GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
                    if (newExplosion.GetComponent<AudioSource>()) newExplosion.GetComponent<AudioSource>().volume = getVolumeData(true);
                }
                hit = true;
                Destroy(gameObject);
            }
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