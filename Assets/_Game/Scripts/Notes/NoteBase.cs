using System;
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
        Inactive
    }
    public abstract class NoteBase : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _noteVisual;
        protected NoteType _noteType;
        public NoteType NoteType => _noteType;
        
        protected float _speed;
        protected NoteData _data;
        protected double _songStartDSP;
        protected float _hitY;
        
        public Action<NoteBase> OnHit;
        private bool _isDie = false;

        public bool IsOutRange => transform.position.y < _hitY - 2f;

        public void Init(NoteData data,
            double songStartDSP,
            float speed,
            float hitY)
        {
            _data = data;
            _noteType = data.Type;
            _songStartDSP = songStartDSP;
            _hitY = hitY;
            _speed = speed;
            _isDie = false;
            OnInit();
        }
        
        public virtual void OnUpdate(double songTime)
        {
            if(_isDie) return;
            double remain = (float)(songTime - _songStartDSP);
            var pos = transform.position;
            pos.y = _hitY + (float)(_data.Time - remain) * _speed;
            transform.position = pos;
            if (IsOutRange)
            {
                OnHit?.Invoke(this);
                _isDie = true;
            }
        }
        
        protected virtual void OnInit()
        {}
        
    }
}
