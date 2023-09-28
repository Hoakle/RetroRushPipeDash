using System;
using System.Collections.Generic;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using UnityEngine;

namespace RetroRush.GameSave
{
    [Serializable]
    public class GlobalGameSave : HoakleEngine.Core.Game.GameSave
    {
        public Wallet Wallet = new Wallet();
        public List<UpgradeData> _Upgrades = new List<UpgradeData>();
        public long BestScore;

        public GlobalGameSave()
        {
            SaveName = "GlobalGameSave";
        }
        public override void Init()
        {
            Wallet.Init();
            foreach (PickableType pickable in Enum.GetValues(typeof(PickableType)))
            {
                if(pickable is PickableType.None or PickableType.Coin)
                    continue;
                
                var upgrade = _Upgrades.Find(f => f.Type == pickable);
                if(upgrade == null)
                    _Upgrades.Add(new UpgradeData(pickable));
            }
        }
    }
}
