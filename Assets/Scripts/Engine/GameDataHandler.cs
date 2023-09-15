using HoakleEngine.Core.Game;
using RetroRush.Game.Level;

namespace RetroRush.Engine
{
    public class GameDataHandlerImpl : GameDataHandler
    {
        private LevelData _LevelData;
        public GameDataHandlerImpl(GameEngine parent) : base(parent)
        {
            
        }

        public LevelData CreateNewLevel()
        {
            _LevelData = new LevelData();
            return _LevelData;
        }
    }
}
