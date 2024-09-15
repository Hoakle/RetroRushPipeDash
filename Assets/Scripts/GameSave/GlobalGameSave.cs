using System;
using System.Collections.Generic;
using System.Linq;
using HoakleEngine;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.GameSave
{
    public class GlobalGameSave : GameSaveHandler<GlobalGameData>
    {
        public IReadOnlyList<UpgradeData> Upgrades
            => _Data.Upgrades;

        public IReadOnlyList<MissionData> Missions
            => _Data.Missions;

        public IReadOnlyReactiveProperty<long> BestScore
            => _BestScore;

        public bool IsReviewDone
        {
            get => _Data.IsReviewDone;
            set => _Data.IsReviewDone = value;
        }
        
        private GameplayConfigData _GameplayConfigData;
        private IPlayServicesTP _PlayServicesTP;
        private IReactiveProperty<long> _BestScore = new ReactiveProperty<long>();

        [Inject]
        public void Inject(
            GameplayConfigData gameplayConfigData,
            IPlayServicesTP playServicesTP)
        {
            _GameplayConfigData = gameplayConfigData;
            _PlayServicesTP = playServicesTP;
        }
        
        public GlobalGameSave() : base("GlobalGameSave")
        {

        }

        protected override void BuildData()
        {
            base.BuildData();
            InitMissions();
            InitUpgrades();
            _BestScore.Value = _Data.BestScore;
        }

        public void InitUpgrades()
        {
            _Data.Upgrades ??= new List<UpgradeData>();
            foreach (UpgradeConfigData config in _GameplayConfigData.UpgradeConfigs)
            {
                var upgrade = Upgrades.FirstOrDefault(u => u.Type == config.Type);
                if(upgrade == null)
                    _Data.Upgrades.Add(new UpgradeData(config.Type));
            }
        }

        private void InitMissions()
        {
            _Data.Missions ??= new List<MissionData>();

            foreach (var mission in _GameplayConfigData.MissionConfigs)
            {
                TryCreateMission(mission.Type);
            }
        }

        private void TryCreateMission(MissionType type)
        {
            if(_Data.Missions.FirstOrDefault(m => m.Type == type) == null)
                _Data.Missions.Add(new MissionData(type));
        }

        private MissionData GetMission(MissionType type)
        {
            return Missions.FirstOrDefault(m => m.Type == type);
        }
        
        public void CompleteMission(MissionType type)
        {
            GetMission(type).IsCompleted = true;
            _PlayServicesTP.UnlockAchievement(_GameplayConfigData.GetMission(type).Key);
            Save();
        }

        public List<string> GetCompletedMissionKeys()
        {
            return Missions.Where(m => m.IsCompleted).Select(m => _GameplayConfigData.GetMission(m.Type).Key).ToList();
        }
        
        public int GetMultiplicator()
        {
            return Missions.Count(m => m.IsCompleted) + 1;
        }
        
        public UpgradeData GetUpgrade(PickableType type)
        {
            return Upgrades.FirstOrDefault(u => u.Type == type);
        }

        public void RegisterScore(long scoreValue)
        {
            _BestScore.Value = Math.Max(scoreValue, _BestScore.Value);
            _Data.BestScore = _BestScore.Value;
            _PlayServicesTP.UpdateScore("CgkIybW_k5kQEAIQAg", _BestScore.Value);
        }
    }

    public struct GlobalGameData
    {
        public List<MissionData> Missions;
        public List<UpgradeData> Upgrades;
        public long BestScore;
        public bool IsReviewDone;
    }
}
