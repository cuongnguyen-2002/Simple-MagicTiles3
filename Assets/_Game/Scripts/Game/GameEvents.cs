using System;
using SMT3.Data;
using SMT3.Notes;

namespace SMT3.Game
{
    public struct NoteHitEvent
    {
        public NoteBase Note;
        public double TabTime;
    }
    
    public static class GameEvents
    {
        public static event Action<NoteHitEvent> OnNoteHit;
        public static event Action<MetaData> OnMoodChanged;
        
        public static void RaiseOnNoteHit(NoteHitEvent e) => OnNoteHit?.Invoke(e);
        public static void RaiseOnMoodChanged(MetaData e) => OnMoodChanged?.Invoke(e);
        
    }
}