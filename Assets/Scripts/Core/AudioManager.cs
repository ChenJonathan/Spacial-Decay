using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Audio settings
    private float volumeEffects = 0.5f;
    public float VolumeEffects
    {
        get { return volumeEffects; }
        set { volumeEffects = value; }
    }
    private float volumeMusic = 0.25f;
    public float VolumeMusic
    {
        get { return volumeMusic; }
        set { audioSource.volume = volumeMusic = value; }
    }

    [SerializeField]
    private AudioClip[] clips;

    private AudioSource audioSource;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volumeMusic;
    }

    public void Restart(int song)
    {
        audioSource.clip = clips[song];
        audioSource.Play();
    }
}