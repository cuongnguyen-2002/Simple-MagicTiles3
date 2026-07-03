using System;
using System.Collections.Generic;
using SMT3.InputSystem;
using SMT3.Systems;
using UnityEngine;
using SMT3.Notes;

namespace SMT3.Systems
{
    public class HitDetectSystem : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private NotesSpawnSystem _notesSpawnSystem;

        [SerializeField] private InputReader _inputSystem;
        [SerializeField] private AudioSystem _audioSystem;

        private List<NoteBase> _activeNotes => _notesSpawnSystem.ActiveNotes;
        private Dictionary<int, NoteBase> _fingerNotesMap = new();
        private float _maxHitDetection = 0.5f;

        private void OnEnable()
        {
            _inputSystem.OnTabBegan += HandleTabBegan;
            _inputSystem.OnTabEnded += HandleTabEnded;
            _inputSystem.OnHeldTab += HandleTabHeld;
        }

        private void OnDisable()
        {
            _inputSystem.OnTabBegan -= HandleTabBegan;
            _inputSystem.OnTabEnded -= HandleTabEnded;
            _inputSystem.OnHeldTab -= HandleTabHeld;
        }

        private void HandleTabBegan(TouchInfo touchInfo)
        {
            NoteBase bestNote = FindBestNote(touchInfo);
            if (bestNote == null) return;
            _fingerNotesMap[touchInfo.FingerId] = bestNote;
            bestNote.OnTabBegan(touchInfo.DpsTime);
        }

        private void HandleTabEnded(TouchInfo touchInfo)
        {
            if (!_fingerNotesMap.TryGetValue(touchInfo.FingerId, out NoteBase fingerNote)) return;
            fingerNote.OnTabEnded(touchInfo.DpsTime);
            _fingerNotesMap.Remove(touchInfo.FingerId);
        }

        private void HandleTabHeld(TouchInfo touchInfo)
        {
            if (!_fingerNotesMap.TryGetValue(touchInfo.FingerId, out NoteBase fingerNote)) return;
            fingerNote.OnHeld(touchInfo.DpsTime);
        }

        private NoteBase FindBestNote(TouchInfo touchInfo)
        {
            double songTime = _audioSystem.SongTime;
            double bestDelta = double.MaxValue;
            NoteBase bestNote = null;

            for (int i = 0; i < _activeNotes.Count; i++)
            {
                var note = _activeNotes[i];
                if (note.IsOutRange || !note.IsActive) continue;
                if (note.Lane != touchInfo.Lane) continue;

                double delta = Math.Abs(note.HitTime - songTime);
                if (delta > _maxHitDetection) continue;
                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    bestNote = note;
                }
            }

            return bestNote;
        }


    }

}