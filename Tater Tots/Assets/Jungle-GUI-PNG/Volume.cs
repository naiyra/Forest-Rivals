using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParameter = "MusicVolume"; // or "SFXVolume"
    [SerializeField] private string playerPrefsKey = "musicVolume";   // or "sfxVolume"

    void Start()
    {
        if (!PlayerPrefs.HasKey(playerPrefsKey))
        {
            PlayerPrefs.SetFloat(playerPrefsKey, 1f);
        }

        LoadVolume();
        ApplyVolume();

        volumeSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    public void OnSliderValueChanged()
    {
        ApplyVolume();
        SaveVolume();
    }

    private void ApplyVolume()
    {
        float sliderValue = volumeSlider.value;
        float volumeInDb = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
        audioMixer.SetFloat(exposedParameter, volumeInDb);
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(playerPrefsKey);
        volumeSlider.value = savedVolume;
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(playerPrefsKey, volumeSlider.value);
    }
}
