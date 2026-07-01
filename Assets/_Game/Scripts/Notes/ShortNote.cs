using System;
using SMT3.Game;
using UnityEngine;

namespace SMT3.Notes
{
    public class ShortNote : NoteBase
    {
        private bool _hasBeenHit = false;
        private double _perfectWindow = 0.05;
        private double _greatWindow = 0.1;
        private double _goodWindow = 0.15;

        protected override void OnInit()
        {
            base.OnInit();
            _hasBeenHit = false;
        }

        public override void OnTabBegan(double songDps)
        {
            base.OnTabBegan(songDps);
            if(_hasBeenHit) return;
            _hasBeenHit = true;

            NoteHitEvent hitEvent = new NoteHitEvent()
            {
                Note =  this,
                TabDps =  songDps - _songStartDSP
            };
            GameEvents.RaiseOnNoteHit(hitEvent);
            Completed();
        }
    }
}