using UnityEngine;
using UnityEngine.SceneManagement;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevels = 3;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Spaceship")) PlayerPrefs.SetString("Spaceship", "SpaceFighter");
        if (!PlayerPrefs.HasKey("HasSpaceFighter")) PlayerPrefs.SetInt("HasSpaceFighter", 1);

        //Set up level data
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", level);
        } else
        {
            if (SceneManager.GetActiveScene().name != "Main Menu") PlayerPrefs.SetInt("Level", level);
        }
        PlayerPrefs.SetInt("MaxLevels", maxLevels);

        //Set up player upgrade data
        if (!PlayerPrefs.HasKey("DamageMultiplier")) PlayerPrefs.SetFloat("DamageMultiplier", 1);
        if (!PlayerPrefs.HasKey("SpeedMultiplier")) PlayerPrefs.SetFloat("SpeedMultiplier", 1);
        if (!PlayerPrefs.HasKey("HealthMultiplier")) PlayerPrefs.SetFloat("HealthMultiplier", 1);
        if (!PlayerPrefs.HasKey("MoneyMultiplier")) PlayerPrefs.SetFloat("MoneyMultiplier", 1);

        //Set up player upgrade price data
        if (!PlayerPrefs.HasKey("DamagePrice")) PlayerPrefs.SetInt("DamagePrice", 8);
        if (!PlayerPrefs.HasKey("SpeedPrice")) PlayerPrefs.SetInt("SpeedPrice", 5);
        if (!PlayerPrefs.HasKey("HealthPrice")) PlayerPrefs.SetInt("HealthPrice", 7);
        if (!PlayerPrefs.HasKey("MoneyPrice")) PlayerPrefs.SetInt("MoneyPrice", 3);

        //Set up money data
        if (!PlayerPrefs.HasKey("Money")) PlayerPrefs.SetString("Money", "10000");

        PlayerPrefs.Save();
        Destroy(gameObject);
    }
}
