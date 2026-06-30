using System;
using System.Collections.Generic;
using SMT3.Data;
using SMT3.Notes;
using UnityEngine;
using UnityEngine.Pool;

namespace SMT3.Systems
{
    public class NotesFactory : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private NotesPoolConfig _notesPoolConfig;
        
        private Dictionary<NoteType, ObjectPool<NoteBase>> _notesPool = new();
        
        public void Init()
        {
            List<NotePoolConfigData> notePoolConfigs = _notesPoolConfig.NotePoolConfigs;
            for (int i = 0; i < notePoolConfigs.Count; i++)
            {
                var notePoolConfig = notePoolConfigs[i];
                var pool = new ObjectPool<NoteBase>(
                    createFunc: () => CreatePool(notePoolConfig),
                    actionOnGet: (note) => note.gameObject.SetActive(true),
                    actionOnRelease: (note) => note.gameObject.SetActive(false),
                    actionOnDestroy: (note) => DestroyPool(note),
                    collectionCheck: false,
                    defaultCapacity: notePoolConfig.DefaultPoolSize,
                    maxSize: notePoolConfig.MaxPoolSize);
                _notesPool[notePoolConfig.NoteType] = pool;
            }
        }

        private NoteBase CreatePool(NotePoolConfigData notePoolConfig)
        {
            var note = Instantiate(notePoolConfig.NotePrefabs);
            note.OnHit += ReturnNote;
            return note;
        }

        private void DestroyPool(NoteBase note)
        {
            note.OnHit -= ReturnNote;
            Destroy(note.gameObject);
        }

        public NoteBase GetNote(NoteType noteType)
        {
            if (!_notesPool.TryGetValue(noteType, out var pool)) return null;
            
            var note = pool.Get();
            return note;
        }

        public void ReturnNote(NoteBase note)
        {
            if (!_notesPool.TryGetValue(note.NoteType, out var pool))
            {
                Debug.LogError($"{note.NoteType} not found");
                return;
            }
            pool.Release(note);
        }

        public void ReturnAllNotes(List<NoteBase> notes)
        {
            foreach (var note in notes) ReturnNote(note);
        }
    }
}