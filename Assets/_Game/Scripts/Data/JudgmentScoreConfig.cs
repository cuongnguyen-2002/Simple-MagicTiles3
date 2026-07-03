using System.Collections.Generic;
using SMT3.Systems;
using UnityEngine;

namespace SMT3.Data
{
    [System.Serializable]
    public class JudgmentScoreConfigData
    {
        [SerializeField] private JudgmentType _judgmentType;
        [SerializeField] private int _judgmentScore;
        
        public JudgmentType JudgmentType => _judgmentType;
        public int JudgmentScore => _judgmentScore;
    }
    
    [CreateAssetMenu(fileName =  "JudgmentScoreConfig", menuName = "Data/JudgmentScoreConfig")]
    public class JudgmentScoreConfig : ScriptableObject
    {
        [SerializeField] private List<JudgmentScoreConfigData> _judgmentScoreConfigData;

        public int GetScore(JudgmentType judgmentType)
        {
            var item = _judgmentScoreConfigData.Find(x => x.JudgmentType == judgmentType);
            return item?.JudgmentScore ?? 0;
        }
    }
}