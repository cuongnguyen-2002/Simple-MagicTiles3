using System;
using Newtonsoft.Json;
using SMT3.Data;
using SMT3.Systems;
using SMT3.UI;
using UnityEngine;

namespace SMT3.Game
{
    public class GameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSystem _audioSystem;
        [SerializeField] private NotesSpawnSystem _notesSpawnSystem;
        [SerializeField] private ScoreSystem _scoreSystem;
        [SerializeField] private UIManager _uiManager;
        
        private GamePlayScreen _gamePlayScreen;
        
        [Header("Level")]
        [SerializeField] private TextAsset _notesFile;
        [SerializeField] private AudioClip _bgmClip;
        
        
        [SerializeField] private float _spawnOffset = 2f;
        private bool _isPlaying = false;
        private float _currentProgress;

        private void Awake()
        {
            RootNoteData root = JsonConvert.DeserializeObject<RootNoteData>(_notesFile.text);
            float spawnY = Camera.main.orthographicSize + _spawnOffset;
            _notesSpawnSystem.Init(root.Notes, root.SongMeta.VisualSpeed, spawnY);
        }

        private void Start()
        {
            _uiManager = UIManager.Instance;
            _gamePlayScreen = _uiManager.GetUI<GamePlayScreen>();
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += GameStarted;
            GameEvents.OnGameOver += GameLose;
            GameEvents.OnGameReset += GameStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= GameStarted;
            GameEvents.OnGameOver -= GameLose;
            GameEvents.OnGameReset -= GameStarted;
        }
        
        public void GameStarted()
        {
            _isPlaying = true;
            ResetProgress();
            SetUpSong();
            _notesSpawnSystem.Restart();
            _uiManager.ShowUI<GamePlayScreen>();
        }

        private void ResetProgress()
        {
            _currentProgress = 0;
        }

        private void SetUpSong()
        {
            _audioSystem.InitSound(_bgmClip);
            _audioSystem.StartSong();
        }

        private void GameLose()
        {
            _isPlaying = false;
            _audioSystem.StopSong();
            GameOverScreen ui = _uiManager.GetUI<GameOverScreen>();
            ui.SetScore(_scoreSystem.CurrentScore);
            ui.Show();
        }

        private void Update()
        {
            if (!_isPlaying) return;
            double songTime = _audioSystem.SongTime;
            _currentProgress = Math.Max(_currentProgress, (float)_audioSystem.Progress);
            _notesSpawnSystem.OnTick(songTime);
            _gamePlayScreen.SetProgress(_currentProgress);

            if (_audioSystem.IsSongFinished && 
                _notesSpawnSystem.HasNoActiveNote && 
                _notesSpawnSystem.AllNoteSpawned)
            {
                StartNewLoop();
            }
        }

        private void StartNewLoop()
        {
            SetUpSong();
            _notesSpawnSystem.Restart();
        }
    }

    public static class GameContext
    {
        private static float SongStartDps;
    }
}
