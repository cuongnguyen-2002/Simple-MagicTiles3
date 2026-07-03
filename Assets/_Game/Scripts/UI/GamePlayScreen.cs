using System;
using DG.Tweening;
using SMT3.Game;
using SMT3.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMT3.UI
{
    public class GamePlayScreen : UIBase
    {
        [SerializeField] private Slider _gameProgressSlider;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _judgeText;
        [SerializeField] private ScoreSystem _scoreSystem;
        private Sequence _scoreSequence;
        private Sequence _judgeSequence;
        private float _scoreScaleMultiplier = 1.2f;
        private float _judgeScaleMultiplier = 1.3f;
        private float _scaleDuration = 0.2f;

        private void OnEnable()
        {
            _scoreSystem.OnScoreChange += UpdateScore;
            _scoreSystem.OnJudgedChange += UpdateJudgeText;
        }

        private void OnDisable()
        {
            _scoreSystem.OnScoreChange -= UpdateScore;
            _scoreSystem.OnJudgedChange -= UpdateJudgeText;
        }

        private void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
            PlayPunchScale(_scoreText.rectTransform, ref _scoreSequence, _scoreScaleMultiplier, _scaleDuration);

        }

        private void UpdateJudgeText(string judge)
        {
            _judgeText.text = judge;
            PlayPunchScale(_judgeText.rectTransform, ref _judgeSequence, _judgeScaleMultiplier, _scaleDuration,
                Ease.InBounce);
        }

        private void PlayPunchScale(RectTransform target,
            ref Sequence sequence,
            float scale,
            float duration,
            Ease ease = Ease.Linear)
        {
            target.localScale = Vector3.one;
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.Append(target.DOScale(Vector3.one * scale, duration));
            sequence.Append(target.DOScale(Vector3.one, duration));
            sequence.SetEase(ease);
            sequence.Play();
        }
        
        public void SetProgress(float progress)
        {
            _gameProgressSlider.value = progress;
        }
    }
}
