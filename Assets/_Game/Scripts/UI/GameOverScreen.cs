using DG.Tweening;
using SMT3.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMT3.UI
{
    public class GameOverScreen : UIBase
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _restartButton;
        private Tween _gameOverTween;
        
        public override void Init(UIManager manager)
        {
            base.Init(manager);
            _restartButton.onClick.AddListener(OnRestartGame);
        }

        private void OnRestartGame()
        {
            GameEvents.RaiseOnGameReset();
            this.Hide();
        }

        public void SetScore(int score)
        {
            _scoreText.text = score.ToString();
            
            _gameOverTween = DOTween.To(() => 0, 
                x => _scoreText.text = x.ToString(), 
                score, 
                0.3f).SetEase(Ease.OutBounce);
            _gameOverTween.Play();
        }
    }
}