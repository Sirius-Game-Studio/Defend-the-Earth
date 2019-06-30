using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Canvas settingsUI;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void clickQuitGame()
    {
        Application.Quit();
    }
}
