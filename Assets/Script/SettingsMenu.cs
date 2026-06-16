using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private Resolution[] resolutions;
    private bool loading = true;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }

        resolutionDropdown.AddOptions(options);

        int savedResolution = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        resolutionDropdown.SetValueWithoutNotify(savedResolution);
        fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);

        masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", 1f));
        musicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 1f));
        sfxVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 1f));

        loading = false;

        ApplyAllSettings();
    }

    public void SetResolution(int index)
    {
        if (loading) return;

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();

        ApplyAllSettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (loading) return;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        ApplyAllSettings();
    }

    public void SetMasterVolume(float volume)
    {
        if (loading) return;

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        if (loading) return;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (loading) return;

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void ApplyAllSettings()
    {
        int index = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (index >= 0 && index < resolutions.Length)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
        }

        Screen.fullScreen = fullscreen;
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    public void ApplySettings()
    {
        ApplyAllSettings();
        PlayerPrefs.Save();
        Debug.Log("Settings saved");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("HomeScene");
    }
}