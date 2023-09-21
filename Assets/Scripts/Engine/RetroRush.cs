using HoakleEngine;
using RetroRush.Engine;
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

        public void Update()
        {
            GameEngine.Update(_IsPaused);
            GraphicsEngine.Update(_IsPaused);
        }
    }
}