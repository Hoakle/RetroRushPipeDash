using System;
using System.Collections.Generic;
using System.Linq;
using HoakleEngine.Core.Game;
using RetroRush.Config;
using UnityEngine;
using Zenject;

namespace RetroRush.GameData
{
    public class ProgressionHandler : GameSaveHandler<ProgressionData>
    {
        public IReadOnlyList<LevelData> Levels
            => _Data.Levels;

        public int CurrentLevel
            => _Data.CurrentLevel;
        public GameModeType GameModeType
            => _Data.GameModeType;

        private LevelConfigData _LevelConfigData;
        public ProgressionHandler(string identifier) : base(identifier) { }

        [Inject]
        public void Inject(LevelConfigData levelConfigData)
        {
            _LevelConfigData = levelConfigData;
        }

        protected override void CreateData()
        {
            _Data.GameModeType = GameModeType.STAGE;
            _Data.CurrentLevel = 1;
            _Data.Levels = new List<LevelData>();
            foreach (var stage in _LevelConfigData.StageConfigs)
            { 
                _Data.Levels.Add(new LevelData(stage.Id));
            }
        }
        
        public int MaxLevel
        {
            get
            {
                var level = _Data.Levels.Find(l => l.Stars == 0);
                if (level != null)
                {
                    return level.Level;
                }

                return _Data.Levels.Last().Level;
            }
        } 
        
        public LevelData GetLevel(int level)
        {
            return Levels.Count < level ? null : Levels[level - 1];
        }

        public void CompleteLevel(int stars)
        {
            var level = GetLevel(CurrentLevel);
            level.Stars = Math.Max(stars, level.Stars);
            Save();
        }

        public void TryIncrementLevel()
        {
            if (GetLevel(CurrentLevel + 1) != null)
                _Data.CurrentLevel++;
        }
        
        public void TryDecrementLevel()
        {
            if (GetLevel(CurrentLevel - 1) != null)
                _Data.CurrentLevel--;
        }

        public void SetGameMode(GameModeType type)
            => _Data.GameModeType = type;

        public void SelectLevel(int level)
            => _Data.CurrentLevel = level;

        public int GetTotalStars()
        {
            int count = 0;
            foreach (var level in Levels)
            {
                count += level.Stars;
            }

            return count;
        }
    }

    public struct ProgressionData
    {
        public int CurrentLevel;
        public GameModeType GameModeType;
        public List<LevelData> Levels;
    }
    
    [Serializable]
    public class LevelData
    {
        public int Level;
        public int Stars;

        public LevelData(int level)
        {
            Level = level;
        }
    }
    
    public enum GameModeType
    {
        ENDLESS = 0,
        STAGE = 1,
    }
}
