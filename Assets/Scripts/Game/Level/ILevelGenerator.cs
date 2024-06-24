using System;
using RetroRush.Game.Gameplay;

namespace RetroRush.Game.Level
{
    public interface ILevelGenerator
    {
        public bool NeedMoreDepth();
        public void AddDepth();
        public void UpdateLevel(float zPosition);
    }
}
