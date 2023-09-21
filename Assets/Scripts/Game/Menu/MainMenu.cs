using System.Collections;
using System.Collections.Generic;
using HoakleEngine;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Camera;
using RetroRush.Engine;
using RetroRush.UI.Components;
using RetroRush.UI.Screen;
using UnityEngine;

namespace RetroRush
{
    public class MainMenu : GraphicalObjectRepresentation
    {
        [SerializeField] private TapToPlayComponent _TapToPlayComponent = null;
        [SerializeField] private Transform _MenuCameraPosition = null;
        [SerializeField] private Transform _StartGameCameraPosition = null;

        private GraphicsEngineImpl _GraphicsEngineImpl;
        public override void OnReady()
        {
            _GraphicsEngineImpl = ((GraphicsEngineImpl)_GraphicsEngine);
                
            _TapToPlayComponent.OnClick += StartGameCameraAnimation;
            EventBus.Instance.Subscribe(EngineEventType.StartGame, Dispose);
            _TapToPlayComponent.SetCamera(_GraphicsEngine.CameraControl.Camera);
            
            AudioPlayer.Instance.Play(AudioKeys.RainLoop);
            
            _GraphicsEngineImpl.GuiEngine.CreateGUI<MainScreen>(GUIKeys.MAIN_GUI);
            _GraphicsEngineImpl.SetCameraControl(new StaticCameraControl(_GraphicsEngineImpl.GuiEngine.Camera), new List<Transform>(){ _MenuCameraPosition });
            
            base.OnReady();
        }

        public override void Dispose()
        {
            _TapToPlayComponent.OnClick -= StartGameCameraAnimation;
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Dispose);
            AudioPlayer.Instance.Stop(AudioKeys.RainLoop);
            base.Dispose();
        }

        private void StartGameCameraAnimation()
        {
            ((StaticCameraControl)_GraphicsEngine.CameraControl).OnTargetReached += StartGame;
            _GraphicsEngine.CameraControl.RemoveAllTarget();
            _GraphicsEngine.CameraControl.AddTarget(_StartGameCameraPosition);
        }

        private void StartGame()
        {
            ((StaticCameraControl)_GraphicsEngine.CameraControl).OnTargetReached -= StartGame;
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            EventBus.Instance.Publish(EngineEventType.StartGame);
        }
    }
}
