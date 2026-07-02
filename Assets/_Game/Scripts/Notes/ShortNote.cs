using System;
using SMT3.Game;
using UnityEngine;

namespace SMT3.Notes
{
    public class ShortNote : NoteBase
    {
        public override void OnTabBegan(double songDps)
        {
            base.OnTabBegan(songDps);
            NoteHitEvent hitEvent = new NoteHitEvent()
            {
                Note =  this,
                TabTime =  songDps - _songStartDSP
            };
            GameEvents.RaiseOnNoteHit(hitEvent);
        }
    }
}