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
        public List<UpgradeData> Upgrades = new List<UpgradeData>();
        public List<MissionData> Missions = new List<MissionData>();
        
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
                
                var upgrade = Upgrades.Find(f => f.Type == pickable);
                if(upgrade == null)
                    Upgrades.Add(new UpgradeData(pickable));
            }

            InitMissions();
        }

        private void InitMissions()
        {
            TryCreateMission(MissionType.FIRST_RUN, "Jouer une première partie de Retro Rush: Pipe Dash");
            TryCreateMission(MissionType.BOOST_COLLECTOR, "Collecter les 3 types de boosts en une partie");
            TryCreateMission(MissionType.BUNNY_UP, "Saute 15 fois en une partie");
        }

        private void TryCreateMission(MissionType type, string description)
        {
            if(Missions.Find(m => m.Type == type) == null)
                Missions.Add(new MissionData(type, description));
        }

        public MissionData GetMission(MissionType type)
        {
            return Missions.Find(m => m.Type == type);
        }

        public int GetMultiplicator()
        {
            return Missions.FindAll(m => m.IsCompleted).Count;
        }
    }
}
