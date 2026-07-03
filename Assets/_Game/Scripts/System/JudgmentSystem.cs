using System;
using SMT3.Game;
using SMT3.Notes;
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
        [SerializeField] private double _perfectWindow = 0.1d;
        [SerializeField] private double _greatWindow = 0.15d;
        [SerializeField] private double _goodWindow = 0.2d;
        [SerializeField] private double _missWindow = 0.46153846153d;

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
            double delta = Math.Abs(noteTime - hitEvent.TabTime);
            if (delta > _missWindow)
            {
                Debug.Log(hitEvent.Note.HitTime + " " + hitEvent.Note.NoteType);
                return;
            }
            JudgmentType type = EvaluateJudge(delta);
            GameEvents.RaiseOnJudged(type);
            hitEvent.Note.CompleteHandle();
        }

        private JudgmentType EvaluateJudge(double delta)
        {
            if(delta <= _perfectWindow)  return JudgmentType.Perfect;
            if(delta <= _greatWindow)  return JudgmentType.Great;
            return JudgmentType.Good;
        }
    }
}
