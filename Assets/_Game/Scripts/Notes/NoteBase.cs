using System;
using DG.Tweening;
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
        Judged,
        Returned
    }
    
    
    public abstract class NoteBase : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _noteVisual;
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
        
        public virtual void OnUpdate(double songTime)
        {
            if(_noteState  != NoteState.Active) return;
            
            Vector2 currentPosition = transform.position;
            currentPosition.y = _hitY + (float)(_data.Time - songTime) * _speed;
            transform.position = currentPosition;
            if (IsOutRange)
            {
                Completed();
            }
        }

        protected virtual void OnInit()
        {
            _noteVisualTween?.Kill();
            _noteVisualTween = _noteVisual.DOFade(1, 0);
            _noteState = NoteState.Active;
        }

        public virtual void OnTabBegan(double songDps){}
        public virtual void OnTabEnded(double songDps) {}
        public virtual void OnHeld(double songDps) {}

        private void ReturnToPool()
        {
            _noteVisualTween?.Kill();
            _noteVisualTween = _noteVisual.DOFade(0, 0.5f).OnComplete(() => OnHit?.Invoke(this));
        }

        protected void Completed()
        {
            ReturnToPool();
            _noteState = NoteState.Returned;
        }

    }
}
