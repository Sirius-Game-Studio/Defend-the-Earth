using UnityEngine;

public class ControllerMusicAdjuster : MonoBehaviour
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
        input.Sound.LowerMusic.performed += context => setVolume.controllerAdjust(false, true);
        input.Sound.IncreaseMusic.performed += context => setVolume.controllerAdjust(true, false);
        input.Sound.LowerMusic.canceled += context => setVolume.controllerCancel();
        input.Sound.IncreaseMusic.canceled += context => setVolume.controllerCancel();
    }

    void OnDisable()
    {
        input.Disable();
        input.Sound.LowerMusic.performed -= context => setVolume.controllerAdjust(false, true);
        input.Sound.IncreaseMusic.performed -= context => setVolume.controllerAdjust(true, false);
        input.Sound.LowerMusic.canceled -= context => setVolume.controllerCancel();
        input.Sound.IncreaseMusic.canceled -= context => setVolume.controllerCancel();
    }
}
