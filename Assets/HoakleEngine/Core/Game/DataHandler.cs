using System.Collections.Generic;

namespace HoakleEngine.Core.Game
{
    public abstract class GameDataHandler
    {
        protected GameEngine _GameEngine;

        public GameDataHandler(GameEngine parent)
        {
            _GameEngine = parent;
        }
    }
}
