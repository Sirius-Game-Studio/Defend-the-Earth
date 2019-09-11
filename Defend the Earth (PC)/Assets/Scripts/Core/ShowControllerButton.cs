using UnityEngine;
using UnityEngine.UI;

public class ShowControllerButton : MonoBehaviour
{
    [SerializeField] private Sprite xboxButton = null;
    [SerializeField] private Sprite PSButton = null;

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
            } else if (controllers[0].Length == 19)
            {
                image.sprite = PSButton;
            }
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
