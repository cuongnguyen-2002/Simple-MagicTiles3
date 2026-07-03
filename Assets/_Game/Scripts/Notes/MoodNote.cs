using SMT3.Data;
using SMT3.Game;
using SMT3.Systems;

namespace SMT3.Notes
{
    public class MoodNote : NoteBase
    {
        private string _backgroundKey = "bg_color";
        public override void OnTabBegan(double songDps)
        {
            base.OnTabBegan(songDps);
            
            NoteHitEvent hitEvent = new NoteHitEvent()
            {
                Note =  this,
                TabTime =  songDps - _songStartDSP
            };
            GameEvents.RaiseOnNoteHit(hitEvent);
            GameEvents.RaiseSpawnVFX(VFXType.Mood, Center);
            
        }

        public override void CompleteHandle()
        {
            base.CompleteHandle();
            MetaData metaData = _data.Metas.Find(x => x.Key == _backgroundKey);
            GameEvents.RaiseOnMoodChanged(metaData);
        }
    }
}