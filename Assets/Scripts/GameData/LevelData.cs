using System;
using HoakleEngine.Core.Game;

namespace RetroRush.GameData
{
    [Serializable]
    public class LevelData : GameSaveData
    {
        public int Level;
        public int Stars;

        public LevelData(int level)
        {
            Level = level;
            Stars = 0;
        }
    }
}
