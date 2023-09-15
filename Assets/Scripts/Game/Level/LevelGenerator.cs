using RetroRush.Game.Gameplay;

namespace RetroRush.Game.Level
{
    public abstract class LevelGenerator
    {
        public LevelGenerator()
        {

        }

        public abstract bool NeedMoreDepth();
        public abstract void AddDepth();
        public abstract void UpdateLevel(float zPosition);
    }
}
