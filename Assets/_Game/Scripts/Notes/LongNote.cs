using UnityEngine;

namespace SMT3.Notes
{
    public class LongNote : NoteBase
    {
        [SerializeField] private SpriteRenderer _tail;
        protected override void OnInit()
        {
            base.OnInit();
            Resize();
        }

        private void Resize()
        {
            float newSize = _data.Duration * _speed;
            _noteVisual.size = new Vector2(_noteVisual.size.x, newSize);
            _tail.transform.localScale = new Vector2(_tail.transform.localScale.x, newSize * 20f - 20f);
        }
    }
}
