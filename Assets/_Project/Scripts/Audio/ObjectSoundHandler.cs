using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundHandler : MonoBehaviour
{
    [SerializeField]
    AudioSources _audioOutput;
    [SerializeField]
    AudioSource _audioSource;
    [SerializeField]
    List<AudioClip> _objectSound;
    [SerializeField]
    float _audibleRadious=1;
    [SerializeField]
    LayerMask _characterLayer;
    [SerializeField]
    public bool isLoop;//If the sound should be played as long as the player has it in sight, maybe combine this with the Cull

    void Start()
    {
        InitializeAudioSource();
        InitializeSound();
    }

    private void InitializeSound()
    {
        Collider[] characterColliders = Physics.OverlapSphere(transform.position, _audibleRadious, _characterLayer);
        if (characterColliders != null)
        {
            foreach (Collider character in characterColliders)
            {
                if (character.tag == "Player")
                {
                    StartSound();
                }
            }
        }
    }

    private void InitializeAudioSource()
    {
        switch (_audioOutput)
        {
            case AudioSources.EnvironmentAudio:
                _audioSource.outputAudioMixerGroup = MasterAudioManager._instance.EnvironmentSoundsMixer;
                break;
            case AudioSources.MusicAudio:
                _audioSource.outputAudioMixerGroup = MasterAudioManager._instance.MusicSoundsMixer;
                break;
            case AudioSources.VoicesAudio:
                _audioSource.outputAudioMixerGroup = MasterAudioManager._instance.VoicesMixer;
                break;
            case AudioSources.SFXAudio:
                _audioSource.outputAudioMixerGroup = MasterAudioManager._instance.SFXMixer;
                break;
            default:
                break;
        }

        _audioSource.maxDistance = _audibleRadious*5;
        _audioSource.minDistance = _audibleRadious;

        _audioSource.loop = isLoop;

        if (_objectSound.Count > 0)
        {
            _audioSource.clip = _objectSound[Random.Range(0,_objectSound.Count-1)];
        }
    }

    public void StartSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _audibleRadious);
    }
}
