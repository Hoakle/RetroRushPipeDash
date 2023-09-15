using HoakleEngine;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Camera;
using RetroRush.UI.Screen;
using UnityEngine;

namespace RetroRush.Engine
{
    public class GraphicsEngineImpl : GraphicsEngine
    {
        public GraphicsEngineImpl(GameRoot gameRoot) : base(gameRoot)
        {
            GuiEngine = new GUIEngineImpl(gameRoot);
        }

        public override void Init(UnityEngine.Camera camera)
        {
            GuiEngine.LinkEngine(GetEngine<GameEngineImpl>());
            GuiEngine.LinkEngine(this);
            
            CameraControl = new ThirdPersonCameraControl(_GameRoot.Camera, new CameraSettingsData( -7f, 3f, 1.5f));
            _UpdateableList.Add(CameraControl);
            
            GuiEngine.CreateGUI<Header>(GUIKeys.HEADER);
            
            EventBus.Instance.Subscribe(EngineEventType.GameOver, DisplayMainMenu);
            DisplayMainMenu();
            
            base.Init(camera);
        }

        private void DisplayMainMenu()
        {
            _GameRoot.GameSaveContainer.Save();
            GuiEngine.CreateGUI<MainScreen>(GUIKeys.MAIN_GUI);
            GuiEngine.CreateGUI<BestScore>(GUIKeys.BEST_SCORE);
        }
    }
}
