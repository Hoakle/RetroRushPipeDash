using System;
using System.Collections.Generic;
using HoakleEngine.Core.Game;

namespace RetroRush.GameData
{
    [Serializable]
    public class GameModeData : GameSaveData
    {
        public GameModeType Type;
        public List<LevelData> Levels = new List<LevelData>();

        public int CurrentLevel = 0;

        public void Init()
        {
            if(Levels.Count == 0)
                Levels.Add(new LevelData(1));
        }
        public LevelData GetLevel(int level)
        {
            return Levels.Count < level ? null : Levels[level - 1];
        }
    }

    public enum GameModeType
    {
        ENDLESS = 0,
        STAGE = 1,
    }
}
