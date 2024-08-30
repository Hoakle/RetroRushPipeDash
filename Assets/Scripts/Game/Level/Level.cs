using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.Game.Gameplay;
using RetroRush.Game.Player;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Screen;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Level
{
    public class Level : GraphicalObjectRepresentation<LevelDesignData>
    {
        [SerializeField] private Transform _LevelContainer;
        
        private ILevelGenerator _LevelGenerator;
        private IBonusMediator _BonusMediator;

        private List<PipeFace> _PipeFaces;

        private GlobalGameSave _GlobalGameSave;

        private Player.Player _Player;
        
        //Used for interpolation
        private float _RotationAngle;
        private float _RotationProgression;
        private float _InterpolationDuration = 0f;
        private int _PendingRotation;

        private IGameState _GameState;
        private ProgressionHandler _ProgressionHandler;
        private AudioPlayer _AudioPlayer;
        private GameplayConfigData _GameplayConfigData;
        private LevelConfigData _LevelConfigData;

        private const float INTERPOLATION_FACTOR = 2f;
        
        [Inject]
        public void Inject(
            IGameState gameState, 
            IBonusMediator bonusMediator,
            ProgressionHandler progressionHandler,
            GlobalGameSave globalGameSave,
            AudioPlayer audioPlayer,
            GameplayConfigData gameplayConfigData,
            LevelConfigData levelConfigData)
        {
            _GameState = gameState;
            _BonusMediator = bonusMediator;
            _ProgressionHandler = progressionHandler;
            _GlobalGameSave = globalGameSave;
            _AudioPlayer = audioPlayer;
            _GameplayConfigData = gameplayConfigData;
            _LevelConfigData = levelConfigData;

            _GameState.State
                .Where(state => state == State.GameOver)
                .Subscribe(_ => EndLevel());

            _GameState.State
                .Where(state => state == State.WinLevel)
                .Subscribe(_ => WinLevel());
        }

        public override void OnReady()
        {
            _AudioPlayer.Play(AudioKeys.MainLevelLoop);

            _PipeFaces = new List<PipeFace>();
            
            _GameState.SetState(State.Start);
            
            EventBus.Instance.Subscribe<int>(EngineEventType.MoveSideway, RotateLevel);
            EventBus.Instance.Subscribe(EngineEventType.Continue, Continue);
            EventBus.Instance.Subscribe(EngineEventType.BackToMenu, Close);

            InitLevel();
            _LevelContainer.Rotate(0, 0, -90);
            _LevelContainer.Rotate(0, 0, (360f / Data.NumberOfFace) * 6);
            Data.OnDepthAdded += AddDepth;
            
            while (_LevelGenerator.NeedMoreDepth())
            {
                _LevelGenerator.AddDepth();
            }

            _GraphicsEngine.CreateGraphicalRepresentation<Player.Player, PlayerData>("Player", new PlayerData(), null, (player) =>
            {
                _Player = player;
                _Player.transform.position += new Vector3(0, -Data.Radius + 1f, 0);
                if (_ProgressionHandler.GameModeType == GameModeType.ENDLESS)
                    _BonusMediator.CollectPickable(PickableType.StartBoost);
            });
            
            CreateLevelRepresentation();

            base.OnReady();
        }

        private void InitLevel()
        {
            _LevelContainer.rotation = Quaternion.identity;
            if (_ProgressionHandler.GameModeType == GameModeType.ENDLESS)
            {
                _LevelGenerator = new EndlessLevelGenerator(Data, _GameplayConfigData.GetUpgradeConfig(PickableType.PickableSpawn)
                    .GetValue(_GlobalGameSave.GetUpgrade(PickableType.PickableSpawn).Level));
            }
            else
            {
                _LevelGenerator = new StageLevelGenerator(Data, _LevelConfigData.GetStage(_ProgressionHandler.CurrentLevel));
            }
        }

        public override void Dispose()
        {
            _AudioPlayer.Stop(AudioKeys.MainLevelLoop);
            
            EventBus.Instance.UnSubscribe<int>(EngineEventType.MoveSideway, RotateLevel);
            EventBus.Instance.UnSubscribe(EngineEventType.Continue, Continue);
            EventBus.Instance.UnSubscribe(EngineEventType.BackToMenu, Close);
            
            _LevelContainer.transform.position = Vector3.zero;
            _LevelContainer.transform.position = Vector3.zero;
            
            //_LevelInput.OnMove -= RotateLevel;
            Data.OnDepthAdded -= AddDepth;
            
            base.Dispose();
        }

        public void Update()
        {
            if (_GameState.State.Value == State.Start)
            {
                MoveLevel();
                CheckCoinFactorBonus();
                Data.Score.Value = ((int) - Data.Distance * 10) * _GlobalGameSave.GetMultiplicator();
            }
            
        }

        private void MoveLevel()
        {
            _LevelContainer.position = new Vector3(_LevelContainer.position.x, _LevelContainer.position.y, _LevelContainer.position.z - (Data.GetFinalSpeed()));
            Data.Distance = _LevelContainer.position.z;
            _LevelGenerator.UpdateLevel(Data.Distance);

            if (Math.Abs(_RotationAngle - _RotationProgression) > 0.01f)
            {
                //Rotation du tube
                var lerp = Mathf.Lerp(0, _RotationAngle - _RotationProgression, _InterpolationDuration);
                _RotationProgression += lerp;
                _LevelContainer.Rotate(0, 0, lerp);
                _InterpolationDuration += (Time.deltaTime / _PendingRotation) * INTERPOLATION_FACTOR;
                
                if (Math.Abs(_RotationAngle - _RotationProgression) == 0f)
                {
                    _InterpolationDuration = 0f;
                    _RotationAngle = 0f;
                    _RotationProgression = 0f;
                    _PendingRotation = 0;
                }
            }

            if(_Player != null)
                _Player.NotifyMovement(Data.GetFinalSpeed());
        }

        private void CheckCoinFactorBonus()
        {
            if (_BonusMediator.HasBonus(PickableType.CoinFactor))
                return;
            
            if(Data.Distance >= _GameplayConfigData.GetUpgradeConfig(PickableType.CoinFactor).GetValue(_GlobalGameSave.GetUpgrade(PickableType.CoinFactor).Level))
                _BonusMediator.CollectPickable(PickableType.CoinFactor);
        }
        private void RotateLevel(int direction)
        {
            //Calculate the angle to mode to the next plane
            _RotationAngle += (360f / Data.NumberOfFace) * -direction;
            _PendingRotation++;
            _InterpolationDuration = (_InterpolationDuration * (_PendingRotation - 1)) / (float) _PendingRotation;
        }

        private int _InstanciationNotReady = 0;

        private void CreateLevelRepresentation()
        {
            _GraphicsEngine.GuiEngine.CreateDataGUI<Score, LevelDesignData>(GUIKeys.SCORE_GUI, Data);
            _InstanciationNotReady = Data.PipeFaces.Count;
            foreach (var face in Data.PipeFaces)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, _LevelContainer, OnPipeFaceInstantiated);
            }

            //StartCoroutine(SetObstacleCoroutine());
        }

        private void AddDepth()
        {
            var list = Data.PipeFaces.FindAll(f => f.Depth == Data.CurrentDepth - 1);
            _InstanciationNotReady = list.Count;
            foreach (var face in list)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, _LevelContainer, OnPipeFaceInstantiated);
            }
            
            //StartCoroutine(SetObstacleCoroutine());
        }

        private void OnPipeFaceInstantiated(PipeFace pipeFace)
        {
            _InstanciationNotReady--;
            pipeFace.OnDispose += OnPipeFaceDisposed;
            _PipeFaces.Add(pipeFace);
            
            if (_BonusMediator.HasBonus(PickableType.Shield) || _BonusMediator.HasBonus(PickableType.StartBoost))
            {
                pipeFace.ActiveShield();
            }
        }

        private void OnPipeFaceDisposed(GraphicalObjectRepresentation gor)
        {
            gor.OnDispose -= OnPipeFaceDisposed;
            _PipeFaces.Remove((PipeFace) gor);
        }

        private IEnumerator SetObstacleCoroutine()
        {
            var faces = _PipeFaces.Skip(Data.PipeFaces.Count - Data.NumberOfFace).ToList();
            yield return new WaitUntil(() =>
            {
                return faces.TrueForAll(face => face.IsReady) && _InstanciationNotReady == 0;
            });

            var face = faces.Find(f => f.Data.HasObstacle);
            if(face != null)
                face.SetObstacle();
        }

        private void EndLevel()
        {
            _GraphicsEngine.GuiEngine.CreateDataGUI<PostGameGUI, LevelDesignData>(GUIKeys.POSTGAME, Data);
        }

        private void WinLevel()
        {
            _GameState.SetState(State.GameOver);
            Data.IsFinished = true;
            _ProgressionHandler.CompleteLevel(_LevelConfigData.GetStars(_ProgressionHandler.CurrentLevel, (int) Data.CoinCollected));
        }
        
        private void Continue()
        {
            _GameState.SetState(State.Start);
            _Player.transform.position = new Vector3(0, -Data.Radius + 1f, 0);
        }
        
        private void Close()
        {
            _GlobalGameSave.RegisterScore(Data.Score.Value);
            _GlobalGameSave.CompleteMission(MissionType.FIRST_RUN);
            Dispose();
        }
    }
}
