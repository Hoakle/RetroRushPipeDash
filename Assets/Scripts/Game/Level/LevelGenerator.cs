using System;
using RetroRush.Game.Gameplay;

namespace RetroRush.Game.Level
{
    public abstract class LevelGenerator
    {
        public LevelGenerator()
        {

        }

        public float PickableSpawnBonusRate { get; set; }

        public abstract bool NeedMoreDepth();
        public abstract void AddDepth();
        public abstract void UpdateLevel(float zPosition);
    }
}
