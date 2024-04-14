using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] AudioSource _sfxAudioSource;
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        _sfxAudioSource.pitch = pitch;
        _sfxAudioSource.clip = clip;
        _sfxAudioSource.Play();
    }
}
