using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "GameplayConfigData", menuName = "Game Data/Config/GameplayConfigData")]
    public class GameplayConfigData : ScriptableObject
    {
        [SerializeField] private List<UpgradeConfigData> _UpgradeConfigs;
        [SerializeField] private List<MissionConfigData> _MissionConfigs;

        public UpgradeConfigData GetUpgradeConfig(PickableType type)
        {
            return _UpgradeConfigs.Find(f => f.Type == type);
        }

        public MissionConfigData GetMission(MissionType type)
        {
            return _MissionConfigs.Find(f => f.Type == type);
        }
    }
}
