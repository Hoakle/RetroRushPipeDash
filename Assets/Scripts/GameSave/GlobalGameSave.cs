using System;
using System.Collections.Generic;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.GameSave
{
    [CreateAssetMenu(fileName = "GlobalGameSave", menuName = "Game Data/GameSaves/GlobalGameSave")]
    [Serializable]
    public class GlobalGameSave : HoakleEngine.Core.Game.GameSave
    {
        public Wallet Wallet;
        public List<UpgradeData> _Upgrades;
        public long BestScore;
        public override void Init()
        {
            Wallet.Init();
            if (_Upgrades == null)
            {
                _Upgrades = new List<UpgradeData>();
                _Upgrades.Add(new UpgradeData(PickableType.Magnet));
                _Upgrades.Add(new UpgradeData(PickableType.SpeedBonus));
                _Upgrades.Add(new UpgradeData(PickableType.Shield));
            }
        }
    }
}
