using System.Collections.Generic;
using HoakleEngine;
using HoakleEngine.Core;
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
            
            CameraControl = new ThirdPersonCameraControl(_GameRoot.Camera, new CameraSettingsData( -7f, -2f, -3f));
            _UpdateableList.Add(CameraControl);
            
            GuiEngine.CreateGUI<Header>(GUIKeys.HEADER);
            
            EventBus.Instance.Subscribe(EngineEventType.BackToMenu, DisplayMainMenu);
            EventBus.Instance.Subscribe(EngineEventType.StartGame, LoadLevelScene);
            
            GuiEngine.CreateGUI<LoadingScreen>(GUIKeys.LOADING);
            
            base.Init(camera);
        }
        
        private void DisplayMainMenu()
        {
            _GameRoot.GameSaveContainer.Save();
            CreateGraphicalRepresentation<MainMenu>("MainMenu");
        }

        private void LoadLevelScene()
        {
            SetCameraControl(new ThirdPersonCameraControl(_GameRoot.Camera, new CameraSettingsData( -7f, -2f, -3f)));
        }

        public void SetCameraControl(CameraControl cameraControl, List<Transform> targets = null)
        {
            if (_UpdateableList.Contains(CameraControl))
                _UpdateableList.Remove(CameraControl);

            CameraControl = cameraControl;
            _UpdateableList.Add(CameraControl);
            
            if(targets != null)
                foreach (var target in targets)
                {
                    CameraControl.AddTarget(target);
                }
            
            CameraControl.SetStartPositionAndSize();
        }
    }
}

