using UnityEngine;

public class ControllerSoundAdjuster : MonoBehaviour
{
    private SetVolume setVolume;
    private Controls input;

    void Awake()
    {
        setVolume = GetComponent<SetVolume>();
        input = new Controls();
    }

    void OnEnable()
    {
        input.Enable();
        input.Sound.LowerSound.performed += context => setVolume.controllerAdjust(false, true);
        input.Sound.IncreaseSound.performed += context => setVolume.controllerAdjust(true, false);
        input.Sound.LowerSound.canceled += context => setVolume.controllerCancel();
        input.Sound.IncreaseSound.canceled += context => setVolume.controllerCancel();
    }

    void OnDisable()
    {
        input.Disable();
        input.Sound.LowerSound.performed -= context => setVolume.controllerAdjust(false, true);
        input.Sound.IncreaseSound.performed -= context => setVolume.controllerAdjust(true, false);
        input.Sound.LowerSound.canceled -= context => setVolume.controllerCancel();
        input.Sound.IncreaseSound.canceled -= context => setVolume.controllerCancel();
    }
}
