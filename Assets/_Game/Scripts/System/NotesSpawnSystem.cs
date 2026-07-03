using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SMT3.Core;
using UnityEngine;
using SMT3.Data;
using SMT3.Notes;

namespace SMT3.Systems
{
    public class NotesSpawnSystem : MonoBehaviour, ITickable
    {
        [Header("References")]
        [SerializeField] private NotesFactory _notesFactory;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform _hitY;
        [SerializeField] private AudioSystem _audioSystem;
        
        [Header("Config")]
        [SerializeField] private float _visualOffsetMultiplier  = 1f;
        
        private List<NoteData> _notesData = new();
        private List<NoteBase> _activeNotes = new();
        private int _currentNoteIndex = 0;
        private float _spawnY;
        private float _visualSpeed;
        private float _travelTime = 0;
        
        public bool AllNoteSpawned => _currentNoteIndex >= _activeNotes.Count;
        public bool HasNoActiveNote => _activeNotes.Count == 0;
        
        public List<NoteBase> ActiveNotes => _activeNotes;
        
        
        private void Awake()
        {
            _notesFactory.Init();
        }
        
        public void Init(List<NoteData> notesData, float visualSpeed, float spawnY)
        {
            _notesData = notesData;
            _spawnY = spawnY;
            _visualSpeed = visualSpeed * _visualOffsetMultiplier;
            _travelTime = (_spawnY - _hitY.transform.position.y) / _visualSpeed;
            //skip note start
            _currentNoteIndex = 1;
        }

        public void OnTick(double songTime)
        {
            SpawnPendingNote(songTime);
            TickActiveNotes(songTime);
        }

        private void SpawnPendingNote(double songTime)
        {
            while (_currentNoteIndex < _notesData.Count)
            {
                double expectedSpawnTime = _notesData[_currentNoteIndex].Time - _travelTime;
                if (expectedSpawnTime <= songTime)
                {
                    var note = SpawnNote(_notesData[_currentNoteIndex], songTime);
                    _currentNoteIndex++;
                    
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
                if (note.IsOutRange || !note.IsActive)
                {
                    _activeNotes.RemoveAt(i);
                    continue;
                }
                note.OnTick(songTime);
            }
        }
        
        private NoteBase SpawnNote(NoteData noteData, double currentSongTime)
        {
            NoteBase note = _notesFactory.GetNote(noteData.Type);
            if (note == null)
            {
                Debug.Log("SpawnNote Failed");
                return null;
            }
            note.Init(noteData, _audioSystem.SongStartDps , _visualSpeed, _hitY.position.y);
            
            float remain = (float)(noteData.Time - currentSongTime);
            float posy = _hitY.position.y + (remain * _visualSpeed);
            float posx = _spawnPoints[noteData.Lane - 1].position.x;
            
            note.transform.position = new Vector3(posx, posy, 0);

            return note;
        }

        public void Restart()
        {
            _currentNoteIndex = 1;
            _notesFactory.ReturnAllNotes(_activeNotes);
            _activeNotes.Clear();
        }
    }
}
