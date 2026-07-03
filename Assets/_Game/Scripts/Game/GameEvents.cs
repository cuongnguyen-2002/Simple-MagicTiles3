using System;
using SMT3.Data;
using SMT3.Notes;
using SMT3.Systems;
using UnityEngine;

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
        public static event Action<JudgmentType> OnJudged;
        public static event Action<VFXType, Vector2> SpawnVFX;
        
        public static event Action OnGameStarted;
        public static event Action OnGameOver;
        public static event Action OnGameReset;
        
        public static void RaiseOnNoteHit(NoteHitEvent e) => OnNoteHit?.Invoke(e);
        public static void RaiseOnMoodChanged(MetaData e) => OnMoodChanged?.Invoke(e);
        public static void RaiseOnJudged(JudgmentType judge) => OnJudged?.Invoke(judge);
        public static void RaiseSpawnVFX(VFXType vfx, Vector2 position)  => SpawnVFX?.Invoke(vfx, position);
        public static void RaiseOnGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseOnGameOver() => OnGameOver?.Invoke();
        public static void RaiseOnGameReset() => OnGameReset?.Invoke();
    }
}