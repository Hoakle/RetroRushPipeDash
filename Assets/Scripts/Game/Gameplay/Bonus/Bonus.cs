using System;
using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public class Bonus
    {
        public PickableType Type;
        
        public float Duration = 4f;
        public float ElapsedTime = 0;
        
        private float Factor;

        public Action<Bonus> OnEnd;
        public Bonus(float duration, float factor)
        {
            Duration = duration;
            Factor = factor;
        }

        public virtual void Tick()
        {
            ElapsedTime += Time.deltaTime;
            if(ElapsedTime >= Duration)
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
            return Factor;
        }
    }

    public class SpeedBonus : Bonus
    {
        public SpeedBonus(float duration, float factor) : base(duration, factor)
        {

            Type = PickableType.SpeedBonus;
        }

        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.SpeedBonusFadeOut);
        }
    }
    
    public class MagnetBonus : Bonus
    {
        public MagnetBonus(float duration, float factor) : base(duration, factor)
        {
            Type = PickableType.Magnet;
        }
        
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.MagnetFadeOut);
        }
    }
    
    public class ShieldBonus : Bonus
    {
        public ShieldBonus(float duration, float factor) : base(duration, factor)
        {
            Type = PickableType.Shield;
        }
        
        public override void Tick()
        {
            base.Tick();
            if(ElapsedTime >= Duration - 1.5f)
            {
                EventBus.Instance.Publish(EngineEventType.ShieldFadeOutWarning);
            }
        }
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.ShieldFadeOut);
        }
    }
    
    public class StartBonus : Bonus
    {
        public float Distance;
        public StartBonus(float distance, float factor) : base(distance, factor)
        {
            Type = PickableType.StartBoost;
            Distance = distance;
            EventBus.Instance.Publish(EngineEventType.StartBoost);
        }

        private bool _WarningNotTriggered = true;
        public override void Tick()
        {
            if(Distance <= 100 && _WarningNotTriggered)
            {
                _WarningNotTriggered = false;
                EventBus.Instance.Publish(EngineEventType.ShieldFadeOutWarning);
            }
            
            if(Distance <= 0)
                RemoveBonus();
        }
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.ShieldFadeOut);
            EventBus.Instance.Publish(EngineEventType.SpeedBonusFadeOut);
        }
    }
    
    public class CoinFactorBonus : Bonus
    {
        public CoinFactorBonus(float distance, float factor) : base(distance, factor)
        {
            Type = PickableType.CoinFactor;
            EventBus.Instance.Publish(EngineEventType.CoinFactorStarted);
        }

        public override void Tick()
        {
            
        }
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
        }
    }
}
