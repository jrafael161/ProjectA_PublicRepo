using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentAudioHandler : MonoBehaviour
{
    [SerializeField]
    AudioClip _audioClip;
    [SerializeField]
    BoxCollider _areaBounds;
    [SerializeField]
    LayerMask playerMask;

    void Start()
    {
        StartSound();
    }

    public void StartSound()
    {
        if (Physics.CheckBox(transform.position, new Vector3(_areaBounds.size.x / 2, _areaBounds.size.y / 2, _areaBounds.size.z / 2), Quaternion.identity, ~playerMask))
        {
            MasterAudioManager._instance.PlaySound(AudioSources.EnvironmentAudio, _audioClip);
            PlayAllSoundsInsideArea();
        }
    }

    private void PlayAllSoundsInsideArea()
    {
        ObjectSoundHandler[] objectsSoundHandlers = GetComponentsInChildren<ObjectSoundHandler>();
        foreach (ObjectSoundHandler soundHandler in objectsSoundHandlers)
        {
            if (soundHandler.isLoop)
            {
                soundHandler.StartSound();
            }
        }
    }

    public void UpdateEnvironmentSound(AudioClip newBackgroundSound)
    {
        MasterAudioManager._instance.PlaySound(AudioSources.EnvironmentAudio, newBackgroundSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MasterAudioManager._instance.TransitionateBetweenEnvironmentSounds(_audioClip);
        }
    }
}
