using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoakleEngine;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.Game.Gameplay;
using RetroRush.GameData;
using RetroRush.GameSave;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = HoakleEngine.Core.Communication.EventBus;

namespace RetroRush.Game.Level
{
    public class Level : GraphicalObjectRepresentation<LevelData>
    {
        [SerializeField] private LevelInput _LevelInput = null;
        private LevelGenerator _LevelGenerator;

        private List<PipeFace> _PipeFaces;
        private List<Bonus> _BonusList;

        private GlobalGameSave _GlobalGameSave;
        private GameplayConfigData _GameplayConfig;
        public override void OnReady()
        {
            _GlobalGameSave = _GraphicsEngine.GameSave.GetSave<GlobalGameSave>();
            _GameplayConfig = _GraphicsEngine.ConfigContainer.GetConfig<GameplayConfigData>();
            
            _PipeFaces = new List<PipeFace>();
            _BonusList = new List<Bonus>();
            
            EventBus.Instance.Subscribe(EngineEventType.Magnet, OnMagnetPicked);
            EventBus.Instance.Subscribe(EngineEventType.Shield, OnShieldPicked);
            EventBus.Instance.Subscribe(EngineEventType.SpeedBonus, OnSpeedBonusPicked);
            EventBus.Instance.Subscribe(EngineEventType.GameOver, EndLevel);
            _LevelGenerator = new LevelGeneratorV3(Data, _GraphicsEngine.ConfigContainer.GetConfig<LevelConfigData>());
            
            _LevelInput.OnMove += RotateLevel;
            Data.OnDepthAdded += AddDepth;
            
            while (_LevelGenerator.NeedMoreDepth())
            {
                _LevelGenerator.AddDepth();
            }

            CreateLevelRepresentation();
            
            base.OnReady();
        }

        public override void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.Magnet, OnMagnetPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.Shield, OnShieldPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.SpeedBonus, OnSpeedBonusPicked);
            EventBus.Instance.UnSubscribe(EngineEventType.GameOver, EndLevel);
            _LevelInput.OnMove -= RotateLevel;
            Data.OnDepthAdded -= AddDepth;
            
            base.Dispose();
        }

        public void Update()
        {
            MoveLevel();

            TickBonus();

            Data.Score = (int) -transform.position.z * 10;
        }

        private void TickBonus()
        {
            for (int i = _BonusList.Count - 1; i >= 0; i--)
            {
                _BonusList[i].Tick();
            }
        }

        private void MoveLevel()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (Data.Speed * Data.SpeedFactor * Time.deltaTime));
            _LevelGenerator.UpdateLevel(transform.position.z);
        }

        private void RotateLevel(float angle)
        {
            //Rotation du tube
            transform.Rotate(0, 0, angle * Time.deltaTime);
        }

        private int _InstanciationNotReady = 0;
        private void CreateLevelRepresentation()
        {
            _GraphicsEngine.GuiEngine.CreateDataGUI<Score, LevelData>(GUIKeys.SCORE_GUI, Data);
            _InstanciationNotReady = Data.PipeFaces.Count;
            foreach (var face in Data.PipeFaces)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, transform, OnPipeFaceInstantiated);
            }

            StartCoroutine(SetObstacleCoroutine());
        }

        private void AddDepth()
        {
            var list = Data.PipeFaces.FindAll(f => f.Depth == Data.CurrentDepth - 1);
            _InstanciationNotReady = list.Count;
            foreach (var face in list)
            {
                AddGOR<PipeFace, PipeFaceData>("PipeFace", face, transform, OnPipeFaceInstantiated);
            }
            
            StartCoroutine(SetObstacleCoroutine());
        }

        private void OnPipeFaceInstantiated(PipeFace pipeFace)
        {
            _InstanciationNotReady--;
            pipeFace.OnDispose += OnPipeFaceDisposed;
            _PipeFaces.Add(pipeFace);
            
            if (GetBonus(PickableType.Shield) != null)
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
        }

        private Bonus GetBonus(PickableType type)
        {
            return _BonusList.Find(bonus => bonus.Type == type);
        }
        private void UpdateLevelGameplayParams()
        {
            Data.SpeedFactor = 1f;
            foreach (var bonus in _BonusList)
            {
                Data.SpeedFactor += bonus.GetSpeedFactor();
            }
            
        }
        private void OnSpeedBonusPicked()
        {
            //TODO Get value from save (player bonus upgrade data)
            var upgrade = _GameplayConfig.GetUpgradeConfig(PickableType.SpeedBonus);
            AddBonus(new SpeedBonus(upgrade.GetValue(_GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.SpeedBonus).Level),
                upgrade.Factor));
        }

        private void OnMagnetPicked()
        {
            //TODO Get value from save (player bonus upgrade data)
            var upgrade = _GameplayConfig.GetUpgradeConfig(PickableType.Magnet);
            AddBonus(new MagnetBonus(upgrade.GetValue(_GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.Magnet).Level)));
        }

        private void OnShieldPicked()
        {
            //TODO Get value from save (player bonus upgrade data)
            var upgrade = _GameplayConfig.GetUpgradeConfig(PickableType.Shield);
            AddBonus(new ShieldBonus(upgrade.GetValue(_GlobalGameSave._Upgrades.Find(b => b.Type == PickableType.Shield).Level)));
        }
#endregion
        private void EndLevel()
        {
            GlobalGameSave save = _GraphicsEngine.GetEngine<GameEngine>().GameSave.GetSave<GlobalGameSave>();
            save.BestScore = Math.Max(save.BestScore, Data.Score);
            Dispose();
        }
    }
    
}
