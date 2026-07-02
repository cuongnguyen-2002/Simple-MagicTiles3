using SMT3.Game;
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
        protected override void OnInit()
        {
            base.OnInit();
            _longNoteState = LongNoteState.WaitingTab;
            _holdVisual.size = new Vector2(_holdVisual.size.x, 0);
            _holdTime = 0f;
            Resize();
        }

        public override void OnTabBegan(double songDps)
        {
            base.OnTabBegan(songDps);
            if(_longNoteState != LongNoteState.WaitingTab) return;
            _longNoteState = LongNoteState.Hold;
        }

        public override void OnHeld(double songDps)
        {
            base.OnHeld(songDps);
            UpdateHold(songDps - _songStartDSP);
        }

        private void UpdateHold(double songTime)
        {
            if (_longNoteState != LongNoteState.Hold) return;
           
            double remain = _endHitTime - songTime;
            _holdTime += Time.deltaTime;
            float holdRatio = Mathf.Clamp01((float)(remain / _data.Duration));
            float rawProcess = 1f - holdRatio;
            float fillHeight = Mathf.Clamp(rawProcess * _fullHeight * 2f, 0, _fullHeight);
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
        }

        private void Resize()
        {
            _fullHeight = _data.Duration * _speed;
            _noteVisual.size = new Vector2(_noteVisual.size.x, _fullHeight);
            _tail.size = new Vector2(_tail.size.x, _fullHeight / 2f + _tailOffet);
        }
    }
}
