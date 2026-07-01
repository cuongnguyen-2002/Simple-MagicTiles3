using System;
using SMT3.Game;
using UnityEngine;

namespace SMT3.Systems
{
    public enum JudgmentType
    {
        Perfect,
        Great,
        Good,
        Miss
    }
    
    public class JudgmentSystem : MonoBehaviour
    {
        [Header("Timing Windows (seconds)")] 
        [SerializeField] private double _perfectWindow = 0.1;
        [SerializeField] private double _greatWindow = 0.15;
        [SerializeField] private double _goodWindow = 0.20;
        [SerializeField] private double _missWindow = 0.3;

        private void OnEnable()
        {
            GameEvents.OnNoteHit += HandleNoteHit;
        }

        private void OnDisable()
        {
            GameEvents.OnNoteHit -= HandleNoteHit;
        }

        private void HandleNoteHit(NoteHitEvent hitEvent)
        {
            double noteTime = hitEvent.Note.HitTime;
            double delta = Math.Abs(noteTime - hitEvent.TabDps);
            
            if(delta > _missWindow) return;

            JudgmentType type;
            if(delta <= _perfectWindow)  type = JudgmentType.Perfect;
            else if(delta <= _greatWindow)  type = JudgmentType.Great;
            else type = JudgmentType.Good;
            
            Debug.Log(delta);
        }
    }
}
