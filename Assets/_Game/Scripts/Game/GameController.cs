using System;
using Newtonsoft.Json;
using SMT3.Data;
using SMT3.Systems;
using UnityEngine;

namespace SMT3.Game
{
    public class GameController : MonoBehaviour
    {
        private bool _isPlaying = false;
        [SerializeField] private AudioSystem _audioSystem;
        [SerializeField] private NotesSpawnSystem _notesSpawnSystem;
        [SerializeField] private TextAsset _notesFile;
        [SerializeField] private AudioClip _bgmClip;
        
        [SerializeField] private float _spawnOffset = 2f;

        private void Awake()
        {
            RootNoteData root = JsonConvert.DeserializeObject<RootNoteData>(_notesFile.text);
            float spawnY = Camera.main.orthographicSize + _spawnOffset;
            _notesSpawnSystem.Init(root.Notes, root.SongMeta.VisualSpeed, spawnY);
        }
        
        [ContextMenu("Play")]
        public void Play()
        {
            _isPlaying = true;
            _audioSystem.InitSound(_bgmClip);
            _audioSystem.StartSong();
        }

        private void Update()
        {
            if (!_isPlaying && Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }
            if (!_isPlaying) return;
            double songTime = _audioSystem.SongTime;
            _notesSpawnSystem.OnTick(songTime);
        }
    }

    public static class GameContext
    {
        private static float SongStartDps;
    }
}
