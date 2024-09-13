using System.Collections;
using HoakleEngine;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Components;
using RetroRush.UI.Screen;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class MainMenu : GraphicalObjectRepresentation
    {
        [SerializeField] private TapToPlayComponent _TapToPlayComponent = null;
        [SerializeField] private Transform _MenuCameraPosition = null;
        [SerializeField] private Transform _StartGameCameraPosition = null;

        private GraphicsEngineImpl _GraphicsEngineImpl;
        private StaticCameraControl _StaticCameraControl;
        private AudioPlayer _AudioPlayer;
        private IPlayServicesTP _PlayServicesTp;
        private ProgressionHandler _ProgressionHandler;
        private GlobalGameSave _GlobalGameSave;

        [Inject]
        public void Inject(StaticCameraControl staticCameraControl,
            AudioPlayer audioPlayer,
            IPlayServicesTP playServicesTp,
            ProgressionHandler progressionHandler,
            GlobalGameSave globalGameSave)
        {
            _StaticCameraControl = staticCameraControl;
            _AudioPlayer = audioPlayer;
            _PlayServicesTp = playServicesTp;
            _ProgressionHandler = progressionHandler;
            _GlobalGameSave = globalGameSave;
        }
        
        public override void OnReady()
        {
            _GraphicsEngineImpl = ((GraphicsEngineImpl)_GraphicsEngine);
                
            _TapToPlayComponent.OnClick += StartGameCameraAnimation;
            EventBus.Instance.Subscribe(EngineEventType.StartGame, Dispose);
            
            _AudioPlayer.Play(AudioKeys.RainLoop);
            
            _GraphicsEngineImpl.GuiEngine.CreateGUI<MainScreen>(GUIKeys.MAIN_GUI, screen => EventBus.Instance.Publish(EngineEventType.MainMenuLoaded));
            _GraphicsEngineImpl.SetCameraControl(_StaticCameraControl);
            _StaticCameraControl.RemoveAllTarget();
            _StaticCameraControl.AddTarget(_MenuCameraPosition);
            _StaticCameraControl.SetStartPositionAndSize();

            CheckReviewProcess();
            
            base.OnReady();
        }

        public override void Dispose()
        {
            _TapToPlayComponent.OnClick -= StartGameCameraAnimation;
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Dispose);
            _AudioPlayer.Stop(AudioKeys.RainLoop);
            base.Dispose();
        }

        private void CheckReviewProcess()
        {
            var level2 = _ProgressionHandler.GetLevel(2);
            Debug.LogError("Check review: Is level 2 complete = " + (level2 is { Stars: > 0 }) + ", Is already review done = " + _GlobalGameSave.IsReviewDone);
            if(level2 is { Stars: > 0 } && !_GlobalGameSave.IsReviewDone)
            {
                _PlayServicesTp.OnReviewInfoReady += DisplayReviewPopup;
                _PlayServicesTp.PrepareReview();
            }
            
            if(_ProgressionHandler.GetTotalStars() >= 20)
                _GlobalGameSave.CompleteMission(MissionType.DANS_LES_ETOILES);
        }

        private void DisplayReviewPopup()
        {
            _GlobalGameSave.IsReviewDone = true;
            _PlayServicesTp.OnReviewInfoReady -= DisplayReviewPopup;
            _GraphicsEngineImpl.GuiEngine.CreateGUI<ReviewScreen>(GUIKeys.REVIEW_SCREEN);
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
