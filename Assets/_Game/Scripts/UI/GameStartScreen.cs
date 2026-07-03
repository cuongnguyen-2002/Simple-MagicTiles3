using SMT3.Game;
using SMT3.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SMT3.UI
{
    public class GameStartScreen : UIBase
    {
        [SerializeField] private Button _startButton;

        public override void Init(UIManager manager)
        {
            base.Init(manager);
            _startButton.onClick.AddListener(OnGameStart);
        }

        private void OnGameStart()
        {
            GameEvents.RaiseOnGameStarted();
            this.Hide();
        }
    }
}
