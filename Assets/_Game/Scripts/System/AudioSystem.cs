using System;
using SMT3.Game;
using UnityEngine;

namespace SMT3.Systems
{
    public class AudioSystem : MonoBehaviour
    {
        private float _delay = 0.2f;
        private AudioSource _audioSource;
        private float _songStartDps;
        private float _songLength;
    
        public float SongStartDps => _songStartDps;
        public double SongDps =>  AudioSettings.dspTime;
        public double SongTime => SongDps - SongStartDps;
        public bool IsSongFinished => SongTime >= _songLength;
        public double Progress => Mathf.Clamp01((float)SongTime / _songLength);

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void InitSound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _songStartDps = (float)SongDps + _delay;
            _songLength = clip.length;
        }

        public void StartSong()
        {
            _audioSource.PlayScheduled(_songStartDps);
        }

        public void StopSong()
        {
            _audioSource.Stop();
        }
    }

}
