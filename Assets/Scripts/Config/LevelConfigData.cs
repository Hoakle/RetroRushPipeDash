using System.Collections.Generic;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "LevelConfigData", menuName = "Game Data/Config/LevelConfigData")]
    public class LevelConfigData : ScriptableObject
    {
        [SerializeField]
        private List<StageConfigData> _StageConfigs = new List<StageConfigData>();

        public List<StageConfigData> StageConfigs => _StageConfigs;
        public StageConfigData GetStage(int level)
        {
            return _StageConfigs[level - 1];
        }

        public int GetStars(int level, int coinCollected)
        {
            return 2;
        }
    }
}
