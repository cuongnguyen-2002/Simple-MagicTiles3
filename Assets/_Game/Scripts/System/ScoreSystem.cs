using System;
using SMT3.Data;
using SMT3.Game;
using UnityEngine;

namespace SMT3.Systems
{
    public class ScoreSystem : MonoBehaviour
    {
        private int _currentScore = 0;
        public int CurrentScore => _currentScore;
        private JudgmentType _currentJudgmentType;
        
        public event Action<int> OnScoreChange;
        public event Action<string> OnJudgedChange;
        [SerializeField] private JudgmentScoreConfig _scoreConfig;
        [SerializeField] private Transform _spawnHitPoint;
        private void OnEnable()
        {
            GameEvents.OnJudged += JudgmentHandler;
            GameEvents.OnGameStarted += ResetScore;
            GameEvents.OnGameReset += ResetScore;
        }

        private void OnDisable()
        {
            GameEvents.OnJudged -= JudgmentHandler;
            GameEvents.OnGameStarted -= ResetScore;
            GameEvents.OnGameReset -= ResetScore;
            
        }

        private void JudgmentHandler(JudgmentType judgmentType)
        {
            int score = _scoreConfig.GetScore(judgmentType);
            AddScore(score);    
            Judgment(judgmentType);
            GameEvents.RaiseSpawnVFX(VFXType.Hit, _spawnHitPoint.position);
        }

        private void AddScore(int score)
        {
            _currentScore += score;
            OnScoreChange?.Invoke(_currentScore);
        }

        private void Judgment(JudgmentType judgmentType)
        {
            _currentJudgmentType = judgmentType;
            OnJudgedChange?.Invoke(judgmentType.ToString());
        }
        
        private void ResetScore()
        {
            _currentScore = 0;
            OnScoreChange?.Invoke(_currentScore);
            OnJudgedChange?.Invoke("");
        }
    }
}
