using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    public static AudioMixerManager Instance;

    public static float masterVolume = 1f;
    public static float musicVolume = 1f;
    public static float sfxVolume = 1f;

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private string masterVolumeParameter = "MasterVolume";
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;


    /**
     * @brief Updates all sliders in the scene to show the user modified value.
     */
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;

        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
    }

    private void SetVolume(string typeOfVolume, float volume)
    {
        audioMixer.SetFloat(typeOfVolume, Mathf.Log10(volume)*20f);
    }

    /**@brief Sliders event */
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        SetVolume(masterVolumeParameter, volume);
    }

    /**@brief Sliders event */
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        SetVolume(musicVolumeParameter, volume);
    }

    /**@brief Sliders event */
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        SetVolume(sfxVolumeParameter, volume);
    }


}
