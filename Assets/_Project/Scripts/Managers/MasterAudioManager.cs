using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioSources
{
    MasterAudio,
    EnvironmentAudio,
    MusicAudio,
    VoicesAudio,
    SFXAudio
}

public class MasterAudioManager : MonoBehaviour
{
    public static MasterAudioManager _instance;
    [SerializeField]
    private AudioMixerGroup _masterMixer;
    public AudioMixerGroup EnvironmentSoundsMixer { get { return _environmentSoundsMixer;} private set { } } [SerializeField] private AudioMixerGroup _environmentSoundsMixer;
    public AudioMixerGroup MusicSoundsMixer { get { return _musicSoundsMixer; } private set { } } [SerializeField] private AudioMixerGroup _musicSoundsMixer;
    public AudioMixerGroup VoicesMixer { get { return _voicesMixer; } private set { } } [SerializeField] private AudioMixerGroup _voicesMixer;
    public AudioMixerGroup SFXMixer { get { return _sFxMixer; } private set { } } [SerializeField] private AudioMixerGroup _sFxMixer;

    [SerializeField]
    private AudioSource _environmentSoundsSource;
    [SerializeField]
    private AudioSource _musicSoundsSource;
    [SerializeField]
    private AudioSource _voicesSource;
    [SerializeField]
    private AudioSource _SFXSource;

    float environmentVolumeBeforeFadeOut;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        InitializeAudioSettings();
    }

    private void InitializeAudioSettings()
    {
        _masterMixer.audioMixer.SetFloat("MasterVolume", ConvertValueToDecibels(PlayerPrefs.GetInt("MasterVolume")));
        _environmentSoundsMixer.audioMixer.SetFloat("EnvironmentVolume", ConvertValueToDecibels(PlayerPrefs.GetInt("EnvironmentVolume")));
        environmentVolumeBeforeFadeOut = PlayerPrefs.GetInt("EnvironmentVolume");
        _musicSoundsMixer.audioMixer.SetFloat("MusicVolume", ConvertValueToDecibels(PlayerPrefs.GetInt("MusicVolume")));
        _voicesMixer.audioMixer.SetFloat("VoicesVolume", ConvertValueToDecibels(PlayerPrefs.GetInt("VoicesVolume")));
        _sFxMixer.audioMixer.SetFloat("SFXVolume", ConvertValueToDecibels(PlayerPrefs.GetInt("SFXVolume")));
    }

    public void ChangeVolume(AudioSources audioSource, float newVolumeLevel)
    {
        switch (audioSource)
        {
            case AudioSources.MasterAudio:
                _masterMixer.audioMixer.SetFloat("MasterVolume", ConvertValueToDecibels(newVolumeLevel));
                break;
            case AudioSources.EnvironmentAudio:
                _environmentSoundsMixer.audioMixer.SetFloat("EnvironmentVolume", ConvertValueToDecibels(newVolumeLevel));
                break;
            case AudioSources.MusicAudio:
                _musicSoundsMixer.audioMixer.SetFloat("MusicVolume", ConvertValueToDecibels(newVolumeLevel));
                break;
            case AudioSources.VoicesAudio:
                _voicesMixer.audioMixer.SetFloat("VoicesVolume", ConvertValueToDecibels(newVolumeLevel));
                break;
            case AudioSources.SFXAudio:
                _sFxMixer.audioMixer.SetFloat("SFXVolume", ConvertValueToDecibels(newVolumeLevel));
                break;
            default:
                break;
        }
    }

    public void PlaySound(AudioSources audioSource, AudioClip audioClip, bool isPermanent=false)
    {
        switch (audioSource)
        {
            case AudioSources.MasterAudio:
                break;
            case AudioSources.EnvironmentAudio:
                if (audioClip != _environmentSoundsSource.clip)
                {
                    _environmentSoundsSource.clip = audioClip;
                    _environmentSoundsSource.Play();
                }
                break;
            case AudioSources.MusicAudio:
                break;
            case AudioSources.VoicesAudio:
                break;
            case AudioSources.SFXAudio:
                break;
            default:
                break;
        }
    }

    private float ConvertValueToDecibels(float volumeLevel)
    {
        //Debug.Log(volumeLevel);
        return (Mathf.Log(volumeLevel/100) * 20);
    }

    public void TransitionateBetweenEnvironmentSounds(AudioClip newEnvironmentSound)
    {
        if(_environmentSoundsSource.clip != newEnvironmentSound)
        {
            StopAllCoroutines();
            float tempValue = 0;
            _environmentSoundsMixer.audioMixer.GetFloat("EnvironmentVolume", out tempValue);
            if (tempValue > environmentVolumeBeforeFadeOut)//If its more than the environment volume it means the previous environment volume was taken from in between fade in and out
            {
                environmentVolumeBeforeFadeOut = tempValue;
            }
            StartCoroutine("FadeOutEnvironmentSound", newEnvironmentSound);
        }
    }

    IEnumerator FadeOutEnvironmentSound(AudioClip audioClip)
    {
        float rateOfFade = 10;//waiting .5 seconds it will take 5 seconds to totally fade out
        for (float i = 0; i <= rateOfFade; i++)
        {
            float volume = Mathf.Lerp(environmentVolumeBeforeFadeOut, .1f, i / rateOfFade);
            _environmentSoundsMixer.audioMixer.SetFloat("EnvironmentVolume", ConvertValueToDecibels(volume));
            yield return new WaitForSeconds(.2f);
        }
        _environmentSoundsSource.clip = audioClip;
        _environmentSoundsSource.Play();
        StartCoroutine("FadeInEnvironmentSound");
    }

    IEnumerator FadeInEnvironmentSound()
    {
        float rateOfFade = 10;//waiting .5 seconds it will take 5 seconds to totally fade out
        for (float i = 0; i <= rateOfFade; i++)
        {
            float volume = Mathf.Lerp(.1f, environmentVolumeBeforeFadeOut, i / rateOfFade);
            _environmentSoundsMixer.audioMixer.SetFloat("EnvironmentVolume", ConvertValueToDecibels(volume));
            yield return new WaitForSeconds(.2f);
        }
    }
}
