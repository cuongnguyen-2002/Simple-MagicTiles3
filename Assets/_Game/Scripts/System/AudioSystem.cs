using System;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    private float _delay = 0.2f;
    private AudioSource _audioSource;
    private float _songStartDps;
    
    public float SongStartDps => _songStartDps;
    public double SongDps =>  AudioSettings.dspTime;
    public double SongTime => SongDps - SongStartDps;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void InitSound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _songStartDps = (float)SongDps + _delay;
    }

    public void StartSong()
    {
        _audioSource.PlayScheduled(_songStartDps);
    }
}
