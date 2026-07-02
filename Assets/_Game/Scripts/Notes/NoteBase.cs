using System;
using DG.Tweening;
using SMT3.Core;
using SMT3.Data;
using UnityEngine;

namespace SMT3.Notes
{
    public enum NoteType
    {
        Short,
        Long,
        ZigZag,
        Mood
    }

    public enum NoteState
    {
        Idle,
        Active,
        Returned
    }
    
    
    public abstract class NoteBase : MonoBehaviour, ITickable
    {
        [SerializeField] protected SpriteRenderer _noteVisual;
        [SerializeField] private SpriteRenderer[]  _noteVisuals;
        protected NoteData _data;
        protected NoteType _noteType;
        protected NoteState _noteState = NoteState.Idle;
        protected float _speed;
        protected float _hitY;
        protected float _hitOffset = 4f;
        protected Tween _noteVisualTween;
        protected double _songStartDSP;
        
        public Action<NoteBase> OnHit;
        public NoteType NoteType => _noteType;
        public bool IsOutRange => transform.position.y < _hitY - _hitOffset;
        public bool IsActive => _noteState == NoteState.Active;
        public int Lane => _data.Lane;
        public double HitTime => _data.Time;

        public void Init(NoteData data,
            double songStartDSP,
            float speed,
            float hitY)
        {
            _data = data;
            _noteType = data.Type;
            _hitY = hitY;
            _speed = speed;
            _songStartDSP = songStartDSP;
            OnInit();
        }
        
        public virtual void OnTick(double songTime)
        {
            if(_noteState  != NoteState.Active) return;
            
            Vector2 currentPosition = transform.position;
            currentPosition.y = _hitY + (float)(_data.Time - songTime) * _speed;
            transform.position = currentPosition;
            if (IsOutRange) HandleMissTimeout();
        }

        protected virtual void OnInit()
        {
            _noteVisualTween?.Kill();
            _noteVisualTween = FadeAllVisual(1, 0);
            _noteState = NoteState.Active;
        }

        public virtual void OnTabBegan(double songDps){}
        public virtual void OnTabEnded(double songDps) {}
        public virtual void OnHeld(double songDps) {}

        private void ReturnToPool()
        {
            _noteVisualTween?.Kill();
            _noteVisualTween = FadeAllVisual(0, 0.5f).OnComplete(() => OnHit?.Invoke(this));
        }

        public virtual void CompleteHandle()
        {
            ReturnToPool();
            _noteState = NoteState.Returned;
        }

        private void HandleMissTimeout()
        {
            CompleteHandle();
        }

        private Tween FadeAllVisual(float alpha, float duration)
        {
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < _noteVisuals.Length; i++)
            {
                seq.Join(_noteVisuals[i].DOFade(alpha, duration));
            }
            return seq;
        }

    }
}
