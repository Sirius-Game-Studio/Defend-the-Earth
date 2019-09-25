using UnityEngine;
using UnityEngine.UI;

public class ShowControllerButton : MonoBehaviour
{
    [SerializeField] private Sprite xboxButton = null;
    [SerializeField] private Sprite PSButton = null;
    [SerializeField] private Sprite switchButton = null;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        string[] controllers = Input.GetJoystickNames();
        if (controllers.Length > 0)
        {
            gameObject.SetActive(true);
            if (controllers[0].ToLower().Contains("xbox"))
            {
                image.sprite = xboxButton;
            } else if (controllers[0].ToLower() == "wireless controller")
            {
                image.sprite = PSButton;
            } else if (controllers[0].ToLower() == "pro controller" || controllers[0].ToLower().Contains("joy-con"))
            {
                image.sprite = switchButton;
            }
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
