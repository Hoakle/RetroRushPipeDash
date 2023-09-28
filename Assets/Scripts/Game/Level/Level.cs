using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.Game.Gameplay;
using RetroRush.Game.PlayerNS;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Screen;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class Level : GraphicalObjectRepresentation<LevelData>
    {
        [SerializeField] private Transform _LevelContainer;
        private LevelGenerator _LevelGenerator;

        private List<PipeFace> _PipeFaces;
        private List<Bonus> _BonusList;

        private GlobalGameSave _GlobalGameSave;
        private GameplayConfigData _GameplayConfig;
        
        private UpgradeData _CoinUpgrade;
        private UpgradeConfigData _CoinConfig;

        private Player _Player;
        
        //Used for interpolation
        private float _RotationAngle;
        private float _RotationProgression;
        private float _InterpolationDuration = 0f;
        private int _PendingRotation;

        private bool _IsGameOver;
        
        public override void OnReady()
        {
            AudioPlayer.Instance.Play(AudioKeys.MainLevelLoop);
            
            _GlobalGameSave = _GraphicsEngine.GameSave.GetSave<GlobalGameSave>();
            _GameplayConfig = _GraphicsEngine.ConfigContainer.GetConfig<GameplayConfigData>();
            
            _CoinConfig = _GameplayConfig.GetUpgradeConfig(PickableType.CoinFactor);
            _CoinUpgrade = _GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.CoinFactor);
            
            _PipeFaces = new List<PipeFace>();
            _BonusList = new List<Bonus>();
            
            _IsGameOver = false;
            
            EventBus.Instance.Subscribe<int>(EngineEventType.MoveSideway, RotateLevel);
            EventBus.Instance.Subscribe(EngineEventType.Magnet, OnMagnetPicked);
            EventBus.Instance.Subscribe(EngineEventType.Shield, OnShieldPicked);
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonus, OnSpeedBonusPicked);
            EventBus.Instance.Subscribe(EngineEventType.Lazer, OnHitLazer);
            EventBus.Instance.Subscribe(EngineEventType.PlayerDied, StopLevel);
            EventBus.Instance.Subscribe(EngineEventType.GameOver, EndLevel);
            EventBus.Instance.Subscribe(EngineEventType.BackToMenu, Dispose);
            
            _LevelGenerator = new LevelGeneratorV3(Data, _GraphicsEngine.ConfigContainer.GetConfig<LevelConfigData>());
            _LevelGenerator.PickableSpawnBonusRate = _GameplayConfig.GetUpgradeConfig(PickableType.PickableSpawn)
                .GetValue(_GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.PickableSpawn).Level);
            
            //_LevelInput.OnMove += RotateLevel;
            Data.OnDepthAdded += AddDepth;
            
            while (_LevelGenerator.NeedMoreDepth())
            {
                _LevelGenerator.AddDepth();
            }

            _GraphicsEngine.CreateGraphicalRepresentation<Player, PlayerData>("Player", new PlayerData(), null, (player) =>
            {
                _Player = player;
                _Player.transform.position += new Vector3(0, -Data.Radius + 1f, 0);

                if (GetBonus(PickableType.StartBoost) != null)
                    _Player.ActiveStartBoost();

            });
            
            CreateLevelRepresentation();
            
            AddStartBoost();
            
            base.OnReady();
        }

        public override void Dispose()
        {
            AudioPlayer.Instance.Stop(AudioKeys.MainLevelLoop);
            
            EventBus.Instance.UnSubscribe<int>(EngineEventType.MoveSideway, RotateLevel);
            EventBus.Instance.UnSubscribe(EngineEventType.Magnet, OnMagnetPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.Shield, OnShieldPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonus, OnSpeedBonusPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.Lazer, OnHitLazer);
            EventBus.Instance.UnSubscribe(EngineEventType.PlayerDied, StopLevel);
            EventBus.Instance.UnSubscribe(EngineEventType.GameOver, EndLevel);
            EventBus.Instance.UnSubscribe(EngineEventType.BackToMenu, Dispose);
            
            _LevelContainer.transform.position = Vector3.zero;
            _LevelContainer.transform.position = Vector3.zero;
            
            //_LevelInput.OnMove -= RotateLevel;
            Data.OnDepthAdded -= AddDepth;
            
            base.Dispose();
        }

        public void Update()
        {
            if (!_IsGameOver)
            {
                MoveLevel();

                TickBonus();

                Data.Score = (int) - _LevelContainer.position.z * 10;
            }
            
        }

        private void TickBonus()
        {
            for (int i = _BonusList.Count - 1; i >= 0; i--)
            {
                if (_BonusList[i].Type == PickableType.StartBoost)
                {
                    ((StartBonus)_BonusList[i]).Distance -= Data.GetFinalSpeed();
                }
                
                _BonusList[i].Tick();
                
            }
        }

        private void MoveLevel()
        {
            _LevelContainer.position = new Vector3(_LevelContainer.position.x, _LevelContainer.position.y, _LevelContainer.position.z - (Data.Speed * Data.SpeedFactor * Data.DifficultySpeedFactor * Time.deltaTime));
            _LevelGenerator.UpdateLevel(_LevelContainer.position.z);

            if (Math.Abs(_RotationAngle - _RotationProgression) > 0.01f)
            {
                //Rotation du tube
                var lerp = Mathf.Lerp(0, _RotationAngle - _RotationProgression, _InterpolationDuration);
                _RotationProgression += lerp;
                _LevelContainer.Rotate(0, 0, lerp);
                _InterpolationDuration += Time.deltaTime / _PendingRotation;
                
                if (Math.Abs(_RotationAngle - _RotationProgression) == 0f)
                {
                    _InterpolationDuration = 0f;
                    _RotationAngle = 0f;
                    _RotationProgression = 0f;
                    _PendingRotation = 0;
                }
            }

            if(_Player != null)
                _Player.NotifyMovement(Data.Speed * Data.SpeedFactor * Data.DifficultySpeedFactor * Time.deltaTime);
            
            if(GetBonus(PickableType.CoinFactor) == null && Mathf.Abs(_LevelContainer.position.z) >= _CoinConfig.GetValue(_CoinUpgrade.Level))
                AddBonus(new CoinFactorBonus(0,0));
        }

        private void StopLevel()
        {
            _IsGameOver = true;
        }

        private void RotateLevel(int direction)
        {
            //Calculate the angle to mode to the next plane
            _RotationAngle += (360 / Data.NumberOfFace) * -direction;
            _PendingRotation++;
            _InterpolationDuration = (_InterpolationDuration * (_PendingRotation - 1)) / (float) _PendingRotation;
        }

        private int _InstanciationNotReady = 0;

        private void CreateLevelRepresentation()
        {
            _GraphicsEngine.GuiEngine.CreateDataGUI<Score, LevelData>(GUIKeys.SCORE_GUI, Data);
            _InstanciationNotReady = Data.PipeFaces.Count;
            foreach (var face in Data.PipeFaces)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, _LevelContainer, OnPipeFaceInstantiated);
            }

            StartCoroutine(SetObstacleCoroutine());
        }

        private void AddDepth()
        {
            var list = Data.PipeFaces.FindAll(f => f.Depth == Data.CurrentDepth - 1);
            _InstanciationNotReady = list.Count;
            foreach (var face in list)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, _LevelContainer, OnPipeFaceInstantiated);
            }
            
            StartCoroutine(SetObstacleCoroutine());
        }

        private void OnPipeFaceInstantiated(PipeFace pipeFace)
        {
            _InstanciationNotReady--;
            pipeFace.OnDispose += OnPipeFaceDisposed;
            _PipeFaces.Add(pipeFace);
            
            if (GetBonus(PickableType.Shield) != null || GetBonus(PickableType.StartBoost) != null)
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
#region Bonus
        private void AddBonus(Bonus bonus)
        {
            var currentBonus = GetBonus(bonus.Type);
            if (currentBonus != null)
                currentBonus.Duration += 4f;
            else
            {
                _BonusList.Add(bonus);
                bonus.OnEnd += RemoveBonus;
                UpdateLevelGameplayParams();
            }
        }

        private void RemoveBonus(Bonus bonus)
        {
            bonus.OnEnd -= RemoveBonus;
            _BonusList.Remove(bonus);
            UpdateLevelGameplayParams();
        }

        private Bonus GetBonus(PickableType type)
        {
            return _BonusList.Find(bonus => bonus.Type == type);
        }
        private void UpdateLevelGameplayParams()
        {
            Data.SpeedFactor = 1f;
            var totalSpeedFactor = 0f;
            foreach (var bonus in _BonusList)
            {
                if(bonus.Type is PickableType.SpeedBonus or PickableType.StartBoost)
                    totalSpeedFactor += bonus.GetFactor();
            }

            if (totalSpeedFactor > 0)
                Data.SpeedFactor = totalSpeedFactor;
        }
        private void OnSpeedBonusPicked()
        {
            if (GetBonus(PickableType.StartBoost) != null)
                return;
            
            var upgradeConfig = _GameplayConfig.GetUpgradeConfig(PickableType.SpeedBonus);
            var upgradeData = _GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.SpeedBonus);
            AddBonus(new SpeedBonus(upgradeConfig.GetValue(upgradeData.Level),
                upgradeConfig.GetFactor(upgradeData.Level)));
        }

        private void OnMagnetPicked()
        {
            var upgradeConfig = _GameplayConfig.GetUpgradeConfig(PickableType.Magnet);
            var upgradeData = _GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.Magnet);
            AddBonus(new MagnetBonus(upgradeConfig.GetValue(upgradeData.Level), upgradeConfig.GetFactor(upgradeData.Level)));
        }

        private void OnShieldPicked()
        {
            if (GetBonus(PickableType.StartBoost) != null)
                return;
            
            var upgradeConfig = _GameplayConfig.GetUpgradeConfig(PickableType.Shield);
            var upgradeData = _GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.Shield);
            AddBonus(new ShieldBonus(upgradeConfig.GetValue(upgradeData.Level), upgradeConfig.GetFactor(upgradeData.Level)));
        }

        private void AddStartBoost()
        {
            var upgradeConfig = _GameplayConfig.GetUpgradeConfig(PickableType.StartBoost);
            var upgradeData = _GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.StartBoost);
            
            AddBonus(new StartBonus(upgradeConfig.GetValue(upgradeData.Level), upgradeConfig.GetFactor(upgradeData.Level)));
        }
#endregion

        private void OnHitLazer()
        {
            if(GetBonus(PickableType.Shield) == null && GetBonus(PickableType.StartBoost) == null)
                EventBus.Instance.Publish(EngineEventType.PlayerDied);
        }
        private void EndLevel()
        {
            GlobalGameSave save = _GraphicsEngine.GetEngine<GameEngine>().GameSave.GetSave<GlobalGameSave>();
            save.BestScore = Math.Max(save.BestScore, Data.Score);
            
            _GraphicsEngine.GuiEngine.CreateDataGUI<PostGameGUI, LevelData>(GUIKeys.POSTGAME, Data);
        }
    }
    
}
