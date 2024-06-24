using HoakleEngine;
using HoakleEngine.Core.Communication;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace RetroRush
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _Canvas = null;
        [SerializeField] private Image _Fill = null;
        [SerializeField] private Camera _Camera = null;
        [SerializeField] private GameObject _GlobalVolume = null;
        
        private AsyncOperation _AsyncOperation;
        private bool _IsLoadingComplete;
        private float _LoadingTime = 0.7f;
        private float _TimeSpend;

        [Inject]
        public void Inject(ICameraProvider cameraProvider)
        {
            cameraProvider.SetCamera(_Camera);
            cameraProvider.Camera
                .SkipLatestValueOnSubscribe()
                .TakeUntilDestroy(this)
                .Subscribe(camera =>
                {
                    Destroy(_Camera.gameObject);
                    _Canvas.worldCamera = camera;
                });
        }
        
        public void Start()
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_Camera.gameObject);
            DontDestroyOnLoad(_GlobalVolume);
            EventBus.Instance.Subscribe(EngineEventType.MainMenuLoaded, CompleteLoading);
            _AsyncOperation = SceneManager.LoadSceneAsync(1);
        }
        
        public void Update()
        {
            if(!_IsLoadingComplete)
            {
                _Fill.fillAmount = _AsyncOperation.progress / 5f;
            }
            else
            {
                _Fill.fillAmount = Mathf.Lerp(_AsyncOperation.progress / 5f, 1f, _TimeSpend / _LoadingTime);
                _TimeSpend += Time.deltaTime;
                if(_TimeSpend >= _LoadingTime)
                    Close();
            }
        }

        private void CompleteLoading()
        {
            _IsLoadingComplete = true;
        }
        
        private void Close()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.MainMenuLoaded, CompleteLoading);
            Destroy(gameObject);
        }
    }
}
