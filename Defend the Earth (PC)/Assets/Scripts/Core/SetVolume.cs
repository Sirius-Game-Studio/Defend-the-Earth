using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private string volume = "";

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat(volume);
    }

    public void setVolume()
    {
        audioMixer.SetFloat(volume, Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat(volume, slider.value);
        PlayerPrefs.Save();
    }
}
