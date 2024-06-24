using HoakleEngine;
using HoakleEngine.Core;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Game.Level;
using RetroRush.GameData;
using RetroRush.GameSave;
using Zenject;

namespace RetroRush.Engine
{
    public class GameEngineImpl : GameEngine
    {
        private LevelDesignData _LevelDesignData;
        private ProgressionHandler _ProgressionHandler;

        [Inject]
        public void Inject(
            LevelDesignData levelDesignData,
            ProgressionHandler progressionHandler)
        {
            _LevelDesignData = levelDesignData;
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
            _LevelDesignData.Reset();
            GetEngine<GraphicsEngine>().CreateGraphicalRepresentation<Level, LevelDesignData>("Level", _LevelDesignData);
            //GetEngine<GraphicsEngine>().GuiEngine.CreateGUI<DebugOverlay>(GUIKeys.DEBUG_OVERLAY);
        }
    }
}

