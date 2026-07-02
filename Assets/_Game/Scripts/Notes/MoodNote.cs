using SMT3.Data;
using SMT3.Game;

namespace SMT3.Notes
{
    public class MoodNote : NoteBase
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

        public override void CompleteHandle()
        {
            base.CompleteHandle();
            MetaData metaData = _data.Metas.Find(x => x.Key == "bg_color");
            GameEvents.RaiseOnMoodChanged(metaData);
        }
    }
}