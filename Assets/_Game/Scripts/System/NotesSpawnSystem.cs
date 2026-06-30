using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using SMT3.Data;
using SMT3.Notes;

namespace SMT3.Systems
{
    public class NotesSpawnSystem : MonoBehaviour
    {
        [SerializeField] private NotesFactory _notesFactory;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform _hitY;
        [SerializeField] private TextAsset _notesFile;
        [SerializeField] private AudioClip _bgmClip;
        
        [SerializeField] private AudioSystem _audioSystem;
        
        private List<NoteData> _notesData = new();
        private int _noteIndex = 0;
        private float _topY;
        private float _visualSpeed;
        private float _travelTime = 0;
        
        private bool _playing = false;
        
        private List<NoteBase> _activeNotes = new();
        
        private void Awake()
        {
            _notesFactory.Init();
            RootNoteData root = JsonConvert.DeserializeObject<RootNoteData>(_notesFile.text);
            _notesData = root.Notes;
            _notesData.Sort((a, b) => a.Time.CompareTo(b.Time));
            
            _topY = Camera.main.orthographicSize + 2f;
            _visualSpeed = root.SongMeta.VisualSpeed * 2;
            _travelTime = (_topY - _hitY.transform.position.y) / _visualSpeed;
            _noteIndex = 1;
        }

        private void Start()
        {
            // SpawnNote(_notesData[_noteIndex]);
            // _noteIndex++;
        }


        public void Init(List<NoteData> notesData)
        {
            _notesData = notesData;
            _noteIndex = 1;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            _playing = true;
            
            _audioSystem.InitSound(_bgmClip);
            _audioSystem.StartSong();
        }

        private void Update()
        {
            if (!_playing && Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }
            
            if (!_playing) return;
            
            double songTime = _audioSystem.SongTime;
            double dps = _audioSystem.SongDps;
            SpawnPendingNote(songTime);
            TickActiveNotes(dps);
        }

        private void SpawnPendingNote(double songTime)
        {
            while (_noteIndex < _notesData.Count)
            {
                double expectedSpawnTime = _notesData[_noteIndex].Time - _travelTime;
                if (expectedSpawnTime <= songTime)
                {
                    var note = SpawnNote(_notesData[_noteIndex]);
                    _noteIndex++;
                    
                    if(note == null) continue;
                    _activeNotes.Add(note);
                }
                else break;
            }
        }

        private void TickActiveNotes(double songTime)
        {
            for (int i = _activeNotes.Count - 1; i >= 0; i--)
            {
                var note = _activeNotes[i];
                if (note.IsOutRange)
                {
                    _activeNotes.RemoveAt(i);
                    continue;
                }
                note.OnUpdate(songTime);
            }
        }
        
        private NoteBase SpawnNote(NoteData noteData)
        {
            NoteBase note = _notesFactory.GetNote(noteData.Type);
            if (note == null)
            {
                Debug.Log("SpawnNote Failed");
                return null;
            }
            note.Init(noteData, _audioSystem.SongStartDps , _visualSpeed, _hitY.position.y);
            
            float currentTime = (float) _audioSystem.SongTime;
            float remain = ((float)noteData.Time - currentTime);
            float posy = _hitY.position.y + (remain * _visualSpeed);
            float posx = _spawnPoints[noteData.Lane - 1].position.x;
            
            note.transform.position = new Vector3(posx, posy, 0);

            return note;
        }
    }
}
