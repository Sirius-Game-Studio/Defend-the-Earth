using UnityEngine;

public class DataInitializer : MonoBehaviour
{
    private bool d = false;
    private bool f = false;
    private bool s = false;
    private bool h = false;
    private bool dm = false;
    private bool fm = false;
    private bool sm = false;
    private bool hm = false;

    void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("Spaceship"))
        {
            PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            PlayerPrefs.Save();
            print("Initialized currently used spaceship data.");
        }

        if (!PlayerPrefs.HasKey("HasSpaceFighter"))
        {
            PlayerPrefs.SetInt("HasSpaceFighter", 1);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("DamageMultiplier"))
        {
            d = true;
            PlayerPrefs.SetFloat("DamageMultiplier", 1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("FireRateMultiplier"))
        {
            f = true;
            PlayerPrefs.SetFloat("FireRateMultiplier", 1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("SpeedMultiplier"))
        {
            s = true;
            PlayerPrefs.SetFloat("SpeedMultiplier", 1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("HealthMultiplier"))
        {
            h = true;
            PlayerPrefs.SetFloat("HealthMultiplier", 1);
            PlayerPrefs.Save();
        }
        if (d && f && s && h) print("Initialized player upgrades.");

        if (!PlayerPrefs.HasKey("DamagePrice"))
        {
            dm = true;
            PlayerPrefs.SetInt("DamagePrice", 7);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("FireRatePrice"))
        {
            fm = true;
            PlayerPrefs.SetInt("FireRatePrice", 5);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("SpeedPrice"))
        {
            sm = true;
            PlayerPrefs.SetInt("SpeedPrice", 4);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("HealthPrice"))
        {
            hm = true;
            PlayerPrefs.SetInt("HealthPrice", 6);
            PlayerPrefs.Save();
        }
        if (dm && fm && sm && hm) print("Initialized player upgrade prices.");

        if (!PlayerPrefs.HasKey("Money"))
        {
            PlayerPrefs.SetString("Money", 0.ToString());
            PlayerPrefs.Save();
            print("Initialized player money.");
        }
        Destroy(gameObject);
    }
}
