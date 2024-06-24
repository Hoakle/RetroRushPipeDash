using System;
using HoakleEngine.Core.Game;
using RetroRush.Game.Gameplay;

namespace RetroRush.GameData
{
    [Serializable]
    public class UpgradeData
    {
        public UpgradeData(PickableType type)
        {
            Type = type;
            Level = 1;
        }
        public PickableType Type;
        public int Level;
    }
}
