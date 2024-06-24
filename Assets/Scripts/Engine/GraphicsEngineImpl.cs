using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.UI.Screen;
using Zenject;

namespace RetroRush.Engine
{
    public class GraphicsEngineImpl : GraphicsEngine
    {
        private ThirdPersonCameraControl _ThirdPersonCameraControl;
        [Inject]
        public void Inject(ThirdPersonCameraControl cameraControl)
        {
            _ThirdPersonCameraControl = cameraControl;
        }
        
        public override void Init()
        {
            GuiEngine.LinkEngine(GetEngine<GameEngineImpl>());
            GuiEngine.LinkEngine(this);

            CameraControl = _ThirdPersonCameraControl;
            
            _UpdateableList.Add(CameraControl);
            
            GuiEngine.CreateGUI<Header>(GUIKeys.HEADER);
            
            EventBus.Instance.Subscribe(EngineEventType.BackToMenu, DisplayMainMenu);
            EventBus.Instance.Subscribe(EngineEventType.StartGame, LoadLevelScene);
            
            CreateGraphicalRepresentation<MainMenu>("MainMenu", null);
            base.Init();
        }
        
        private void DisplayMainMenu()
        {
            CreateGraphicalRepresentation<MainMenu>("MainMenu");
        }

        private void LoadLevelScene()
        {
            SetCameraControl(_ThirdPersonCameraControl);
        }

        public void SetCameraControl(CameraControl cameraControl)
        {
            if (_UpdateableList.Contains(CameraControl))
                _UpdateableList.Remove(CameraControl);

            CameraControl = cameraControl;
            _UpdateableList.Add(CameraControl);
        }
    }
}

