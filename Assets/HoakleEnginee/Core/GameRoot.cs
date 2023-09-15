using System.Collections;
using HoakleEngine.Core.Config;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoakleEngine
{
    public abstract class GameRoot : MonoBehaviour
    {
        protected GameEngine GameEngine;
        protected GraphicsEngine GraphicsEngine;

        [SerializeField] private GraphicsPool _GraphicsPool = null;
        [SerializeField] private ConfigContainer _ConfigContainer = null;
        [SerializeField] private GameSaveContainer _GameSaveContainer = null;
        public GraphicsPool GraphicsPool => _GraphicsPool;
        public ConfigContainer ConfigContainer => _ConfigContainer;
        public GameSaveContainer GameSaveContainer => _GameSaveContainer;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Init();
        }

        protected virtual void Init()
        {
            _GameSaveContainer.LoadSaves();
            _GameSaveContainer.Init();
        }
        
        public Coroutine StartEngineCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void StopEngineCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        #region GraphicsEngine

        [SerializeField] protected EventSystem _EventSystem = null;

        [SerializeField] protected Camera _Camera;
        public Camera Camera => _Camera;

        #endregion
    }
}
