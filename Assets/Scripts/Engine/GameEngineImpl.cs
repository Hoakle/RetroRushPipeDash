using HoakleEngine;
using HoakleEngine.Core;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Level;

namespace RetroRush.Engine
{
    public class GameEngineImpl : GameEngine
    {
        
        public GameEngineImpl(GameRoot gameRoot) : base(gameRoot)
        {
            _GameDataHandler = new GameDataHandlerImpl(this);
        }

        public override void Init()
        {
            EventBus.Instance.Subscribe(EngineEventType.StartGame, StartGame);
            base.Init();
        }

        public override void Update(bool isPaused)
        {
            base.Update(isPaused);
        }

        public void StartGame()
        {
            GetEngine<GraphicsEngine>().CreateGraphicalRepresentation<Level, LevelData>("Level", ((GameDataHandlerImpl)_GameDataHandler).CreateNewLevel());
            //GetEngine<GraphicsEngine>().GuiEngine.CreateGUI<DebugOverlay>(GUIKeys.DEBUG_OVERLAY);
        }
    }
}

