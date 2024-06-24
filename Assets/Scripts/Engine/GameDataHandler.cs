using HoakleEngine.Core.Game;
using RetroRush.Game.Level;
using RetroRush.GameData;

namespace RetroRush.Engine
{
    public class GameDataHandlerImpl : GameDataHandler
    {
        private LevelDesignData _levelDesignData;
        public GameDataHandlerImpl(GameEngine parent) : base(parent)
        {
            
        }

        public LevelDesignData CreateNewLevel(GameModeData gameMode)
        {
            _levelDesignData = new LevelDesignData();
            _levelDesignData.GameMode = gameMode;
            return _levelDesignData;
        }
    }
}
