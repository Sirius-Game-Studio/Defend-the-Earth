using UnityEngine;
using UnityEngine.SceneManagement;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevels = 20;
    [SerializeField] private bool setLevel = true;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Tips")) PlayerPrefs.SetInt("Tips", 1);
        if (!PlayerPrefs.HasKey("Spaceship")) PlayerPrefs.SetString("Spaceship", "SpaceFighter");

        //Set up owned spaceship data
        if (!PlayerPrefs.HasKey("HasSpaceFighter")) PlayerPrefs.SetInt("HasSpaceFighter", 1);
        if (!PlayerPrefs.HasKey("HasAlienMower")) PlayerPrefs.SetInt("HasAlienMower", 0);
        if (!PlayerPrefs.HasKey("HasBlazingRocket")) PlayerPrefs.SetInt("HasBlazingRocket", 0);
        if (!PlayerPrefs.HasKey("HasQuadShooter")) PlayerPrefs.SetInt("HasQuadShooter", 0);
        if (!PlayerPrefs.HasKey("HasPointVoidBreaker")) PlayerPrefs.SetInt("HasPointVoidBreaker", 0);
        if (!PlayerPrefs.HasKey("HasAnnihilator")) PlayerPrefs.SetInt("HasAnnihilator", 0);

        //Gets the level the player previously played if there's a level key
        int previousLevel = 1;
        if (PlayerPrefs.HasKey("Level")) previousLevel = PlayerPrefs.GetInt("Level");

        //Set up level data
        string sceneName = SceneManager.GetActiveScene().name;
        if (setLevel)
        {
            if (!PlayerPrefs.HasKey("Level"))
            {
                if (sceneName.ToLower().Contains("level"))
                {
                    PlayerPrefs.SetInt("IngameLevel", level);
                    if (!PlayerPrefs.HasKey("Restarted")) PlayerPrefs.SetInt("Level", level);
                } else
                {
                    PlayerPrefs.SetInt("Level", 1);
                    PlayerPrefs.SetInt("IngameLevel", 1);
                }
            } else
            {
                if (sceneName.ToLower().Contains("level"))
                {
                    PlayerPrefs.SetInt("IngameLevel", level);
                    if (!PlayerPrefs.HasKey("Restarted")) PlayerPrefs.SetInt("Level", level);
                }
            }
        }
        if (maxLevels > 0)
        {
            PlayerPrefs.SetInt("MaxLevels", maxLevels);
        } else
        {
            PlayerPrefs.SetInt("MaxLevels", 1);
        }
        if (PlayerPrefs.GetInt("Level") != previousLevel && PlayerPrefs.HasKey("WatchedAd")) PlayerPrefs.DeleteKey("WatchedAd");
        if (!PlayerPrefs.HasKey("Autofire")) PlayerPrefs.SetInt("Autofire", 1);
        if (!PlayerPrefs.HasKey("Money")) PlayerPrefs.SetString("Money", "0");
        PlayerPrefs.Save();
        Destroy(gameObject);
    }
}