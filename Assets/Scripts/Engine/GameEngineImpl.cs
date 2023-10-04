using HoakleEngine;
using HoakleEngine.Core;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Level;
using RetroRush.GameData;
using RetroRush.GameSave;

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

        public void CompleteMission(MissionType type)
        {
            var mission = GameSave.GetSave<GlobalGameSave>().GetMission(type);
            mission.IsCompleted = true;
            
            ServicesContainer.GetService<PlayServicesTP>().UnlockAchievement(ConfigContainer.GetConfig<GameplayConfigData>().GetMission(type).Key);
        }
    }
}

