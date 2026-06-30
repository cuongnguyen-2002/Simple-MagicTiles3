using System.Collections.Generic;
using SMT3.Notes;
using UnityEngine;

namespace SMT3.Data
{
    [System.Serializable]
    public class NotePoolConfigData
    {
        [SerializeField] private NoteType _noteType;
        [SerializeField] private NoteBase _notePrefabs;
        [SerializeField] private int _defaultPoolSize;
        [SerializeField] private int _maxPoolSize;
        
        public NoteType NoteType => _noteType;
        public NoteBase NotePrefabs => _notePrefabs;
        public int DefaultPoolSize => _defaultPoolSize;
        public int MaxPoolSize => _maxPoolSize;
    }
    
    [CreateAssetMenu(fileName = "NotesPoolConfig", menuName = "Data/NotesPoolConfig")]
    public class NotesPoolConfig : ScriptableObject
    {
        [SerializeField] private List<NotePoolConfigData> _notePoolConfigs = new();
        public List<NotePoolConfigData> NotePoolConfigs => _notePoolConfigs;
    }
}