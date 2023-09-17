using System;
using System.Collections.Generic;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using UnityEngine;

namespace RetroRush.GameSave
{
    [CreateAssetMenu(fileName = "GlobalGameSave", menuName = "Game Data/GameSaves/GlobalGameSave")]
    [Serializable]
    public class GlobalGameSave : HoakleEngine.Core.Game.GameSave
    {
        public Wallet Wallet = new Wallet();
        public List<UpgradeData> _Upgrades = new List<UpgradeData>();
        public long BestScore;
        public override void Init()
        {
            Wallet.Init();
            if (_Upgrades.Count == 0)
            {
                _Upgrades.Add(new UpgradeData(PickableType.Magnet));
                _Upgrades.Add(new UpgradeData(PickableType.SpeedBonus));
                _Upgrades.Add(new UpgradeData(PickableType.Shield));
            }
        }
    }
}
