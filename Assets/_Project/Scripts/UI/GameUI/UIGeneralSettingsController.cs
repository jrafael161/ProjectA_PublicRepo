using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGeneralSettingsController : MonoBehaviour
{
    public Slider MasterVolumeSlider;

    private void Start()
    {
        InitializeSoundsUI();
    }

    private void InitializeSoundsUI()
    {
        MasterVolumeSlider.value = PlayerPrefs.GetInt("MasterVolume");
    }

    public void ChangeMasterVolume(float newVolume)
    {
        MasterAudioManager._instance.ChangeVolume(AudioSources.MasterAudio, newVolume);
        GameController._instance.gameSettings.masterVolume = Mathf.FloorToInt(newVolume);
        GameController._instance.settingsChanged = true;
    }

    public void ChangeMasterVolume()
    {
        MasterAudioManager._instance.ChangeVolume(AudioSources.MasterAudio, MasterVolumeSlider.value);
        GameController._instance.gameSettings.masterVolume = Mathf.FloorToInt(MasterVolumeSlider.value);
        GameController._instance.settingsChanged = true;
    }
}
