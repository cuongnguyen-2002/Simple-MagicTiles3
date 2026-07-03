using System;
using SMT3.Game;
using SMT3.Systems;
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
            GameEvents.RaiseSpawnVFX(VFXType.Tab, Center);
        }
    }
}