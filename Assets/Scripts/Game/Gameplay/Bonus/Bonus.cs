using System;
using HoakleEngine.Core.Communication;
using RetroRush.Config;
using RetroRush.Game.Level;
using RetroRush.GameData;
using UnityEngine;
using Zenject;
using EventBus = HoakleEngine.Core.Communication.EventBus;

namespace RetroRush.Game.Gameplay
{
    public abstract class Bonus
    {
        public PickableType Type => _Type;
        public float Duration => _Duration;
        public float ElapsedTime => _ElapsedTime;
        public Action<Bonus> OnEnd;

        protected GameModeType _GameMode;
        protected PickableType _Type;
        protected float _Duration;
        protected float _Factor;
        private float _ElapsedTime;

        [Inject]
        public void Inject(ProgressionHandler progressionHandler)
        {
            _GameMode = progressionHandler.GameModeType;
        }
        
        public virtual void Tick()
        {
            _ElapsedTime += Time.deltaTime;
            if(_ElapsedTime >= _Duration)
            {
                RemoveBonus();
            }
        }

        protected virtual void RemoveBonus()
        {
            OnEnd?.Invoke(this);
        }
        
        public virtual float GetFactor()
        {
            return _Factor;
        }

        public void RefreshDuration()
        {
            _ElapsedTime = Mathf.Max(0,_ElapsedTime - _Duration);
        }
    }

    public class SpeedBonus : Bonus
    {
        [Inject]
        public void Inject(
            [Inject(Id = GameIdentifier.SpeedBoostConfig)] UpgradeConfigData configData,
            [Inject(Id = GameIdentifier.SpeedBoostData)] UpgradeData upgradeData)
        {
            _Type = PickableType.SpeedBoost;
            int level = _GameMode == GameModeType.ENDLESS ? upgradeData.Level : 1;
            _Duration = configData.GetValue(level);
            _Factor = configData.GetFactor(level);
        }
    }
    
    public class MagnetBonus : Bonus
    {
        [Inject]
        public void Inject(
            [Inject(Id = GameIdentifier.MagnetConfig)] UpgradeConfigData configData,
            [Inject(Id = GameIdentifier.MagnetData)] UpgradeData upgradeData)
        {
            _Type = PickableType.Magnet;
            int level = _GameMode == GameModeType.ENDLESS ? upgradeData.Level : 1;
            _Duration = configData.GetValue(level);
            _Factor = configData.GetFactor(level);
        }
        
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
        }
    }
    
    public class ShieldBonus : Bonus
    {
        private bool _WarningTrigered;
        
        [Inject]
        public void Inject(
            [Inject(Id = GameIdentifier.ShieldConfig)] UpgradeConfigData configData,
            [Inject(Id = GameIdentifier.ShieldData)] UpgradeData upgradeData)
        {
            _Type = PickableType.Shield;
            int level = _GameMode == GameModeType.ENDLESS ? upgradeData.Level : 1;
            _Duration = configData.GetValue(level);
            _Factor = configData.GetFactor(level);
        }
        
        public override void Tick()
        {
            base.Tick();
            if(ElapsedTime >= Duration - 1.5f && !_WarningTrigered)
            {
                _WarningTrigered = true;
                EventBus.Instance.Publish(EngineEventType.ShieldFadeOutWarning);
            }
        }
    }
    
    public class StartBonus : Bonus
    {
        private LevelDesignData _LevelDesignData;
        private float _Distance;
        
        [Inject]
        public void Inject(
            [Inject(Id = GameIdentifier.StartBoostConfig)] UpgradeConfigData configData,
            [Inject(Id = GameIdentifier.StartBoostData)] UpgradeData upgradeData,
            LevelDesignData levelDesignData)
        {
            _Type = PickableType.StartBoost;
            int level = _GameMode == GameModeType.ENDLESS ? upgradeData.Level : 1;
            _Distance = configData.GetValue(level);
            _Duration = _Distance;
            _Factor = configData.GetFactor(level);
            _LevelDesignData = levelDesignData;
        }

        private bool _WarningNotTriggered = true;
        public override void Tick()
        {
            _Distance -= _LevelDesignData.GetFinalSpeed();
            
            if(_Distance <= 100 && _WarningNotTriggered)
            {
                _WarningNotTriggered = false;
                EventBus.Instance.Publish(EngineEventType.ShieldFadeOutWarning);
            }
            
            if(_Distance <= 0)
                RemoveBonus();
        }
    }
    
    public class CoinFactorBonus : Bonus
    {
        [Inject]
        public void Inject(
            [Inject(Id = GameIdentifier.CoinFactorConfig)] UpgradeConfigData configData,
            [Inject(Id = GameIdentifier.CoinFactorData)] UpgradeData upgradeData)
        {
            _Type = PickableType.CoinFactor;
            int level = _GameMode == GameModeType.ENDLESS ? upgradeData.Level : 1;
            _Duration = configData.GetValue(level);
            _Factor = configData.GetFactor(level);
        }
    }
}
