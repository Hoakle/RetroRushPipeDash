using System.Collections;
using System.Collections.Generic;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush
{
    [CreateAssetMenu(fileName = "GameplayConfigData", menuName = "Game Data/Config/GameplayConfigData")]
    public class GameplayConfigData : ScriptableObject
    {
        [SerializeField] private List<UpgradeConfigData> _UpgradeConfigs;

        public UpgradeConfigData GetUpgradeConfig(PickableType type)
        {
            return _UpgradeConfigs.Find(f => f.Type == type);
        }
    }
}
