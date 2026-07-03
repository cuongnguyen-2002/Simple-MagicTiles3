using System;
using SMT3.Game;
using SMT3.Systems;
using UnityEngine;

namespace SMT3.Notes
{
    public enum LongNoteState
    {
        WaitingTab,
        Hold,
        Released
    }
    public class LongNote : NoteBase
    {
        [SerializeField] private SpriteRenderer _tail;
        [SerializeField] private SpriteRenderer _holdVisual;
        private LongNoteState _longNoteState;
        private float _tailOffet = 0.4f;
        private float _fullHeight;
        private float _holdTime = 0f;
        private double _endHitTime => _data.Time + _data.Duration;
        private double _holdStartTime;
        protected override void OnInit()
        {
            base.OnInit();
            _longNoteState = LongNoteState.WaitingTab;
            _holdVisual.size = new Vector2(_holdVisual.size.x, 0);
            _holdTime = 0f;
            _holdStartTime = 0;
            Resize();
        }

        public override void OnTabBegan(double songDps)
        {
            base.OnTabBegan(songDps);
            if(_longNoteState != LongNoteState.WaitingTab) return;
            _longNoteState = LongNoteState.Hold;
            _holdStartTime = songDps - _songStartDSP;
        }

        public override void OnHeld(double songDps)
        {
            base.OnHeld(songDps);
            UpdateHold(songDps - _songStartDSP);
        }

        private void UpdateHold(double songTime)
        {
            if (_longNoteState != LongNoteState.Hold) return;
            _holdTime += Time.deltaTime;

            double startWindows = Math.Min(_holdStartTime, _data.Time);
            double remain = _endHitTime - startWindows;
            float holdRatio = Mathf.Clamp01((float)(remain / _data.Duration));

            float fillHeight = Mathf.Clamp(holdRatio * _fullHeight, 0, _fullHeight);
            _holdVisual.size = new Vector2(_holdVisual.size.x, fillHeight);
        }

        public override void OnTabEnded(double songDps)
        {
            base.OnTabEnded(songDps);
            if(_longNoteState != LongNoteState.Hold) return;
            _longNoteState = LongNoteState.Released;
            NoteHitEvent hitEvent = new NoteHitEvent()
            {
                Note =  this,
                TabTime =  songDps - _songStartDSP - _holdTime
            };
            GameEvents.RaiseOnNoteHit(hitEvent);
            GameEvents.RaiseSpawnVFX(VFXType.Hit, this.transform.position + new Vector3(0,1,0));
        }

        private void Resize()
        {
            _fullHeight = _data.Duration * _speed;
            _noteVisual.size = new Vector2(_noteVisual.size.x, _fullHeight);
            _tail.size = new Vector2(_tail.size.x, _fullHeight / 2f + _tailOffet);
        }
    }
}
