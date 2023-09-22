using HoakleEngine;
using HoakleEngine.Core;
using HoakleEngine.Core.Game;
using RetroRush.Engine;
using RetroRush.GameSave;
using UnityEngine;

namespace Scripts.Engine
{
    public class RetroRush : GameRoot
    {
        public RetroRush()
        {
            GameEngine = new GameEngineImpl(this);
            GraphicsEngine = new GraphicsEngineImpl(this);
            
            GameEngine.LinkEngine(GraphicsEngine);
            GraphicsEngine.LinkEngine(GameEngine);
        }
        
        protected override void Init()
        {
            base.Init();
            GraphicsEngine.Init(_Camera);
            GameEngine.Init();
        }

        protected override void InitGameSave(GameSaveContainer container)
        {
            container.SetSave(new GlobalGameSave());
            base.InitGameSave(container);
        }

        public void Update()
        {
            GameEngine.Update(_IsPaused);
            GraphicsEngine.Update(_IsPaused);
        }
    }
}