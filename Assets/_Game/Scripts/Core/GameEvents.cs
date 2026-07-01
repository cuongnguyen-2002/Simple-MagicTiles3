using System;
using SMT3.Notes;

namespace SMT3.Game
{
    public struct NoteHitEvent
    {
        public NoteBase Note;
        public double TabDps;
    }
    
    public static class GameEvents
    {
        public static event Action<NoteHitEvent> OnNoteHit;
        
        public static void RaiseOnNoteHit(NoteHitEvent e) => OnNoteHit?.Invoke(e);
        
    }
}