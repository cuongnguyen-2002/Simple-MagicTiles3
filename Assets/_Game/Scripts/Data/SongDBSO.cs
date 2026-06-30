using UnityEngine;

namespace SMT3.Data
{
    [CreateAssetMenu(fileName = "SongDBSO", menuName = "Data/SongDBSO")]
    public class SongDBSO : ScriptableObject
    {
        [SerializeField] private AudioClip _song;
        [SerializeField] private TextAsset _songData;
        public AudioClip song => _song;
        public TextAsset songData => _songData;
    }
}