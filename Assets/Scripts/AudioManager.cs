using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";


    void Start()
    {
        // Lataa tallennetut arvot
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Aseta mixer arvot heti
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        // Lis‰t‰‰n kuuntelijat
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MyExposedParam", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MyExposedParam 1", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("MyExposedParam 2", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, value);
        PlayerPrefs.Save();
    }
}
