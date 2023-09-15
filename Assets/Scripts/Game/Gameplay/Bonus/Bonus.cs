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

        public Action<Bonus> OnEnd;
        public Bonus(float duration)
        {
            Duration = duration;
        }

        public void Tick()
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
        public virtual float GetSpeedFactor()
        {
            return 0f;
        }
    }

    public class SpeedBonus : Bonus
    {
        private float SpeedFactor;
        public SpeedBonus(float duration, float factor) : base(duration)
        {
            SpeedFactor = factor;
            Type = PickableType.SpeedBonus;
        }
        
        public override float GetSpeedFactor()
        {
            return SpeedFactor;
        }

        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.SpeedBonusFadeOut);
        }
    }
    
    public class MagnetBonus : Bonus
    {
        public MagnetBonus(float duration) : base(duration)
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
        public ShieldBonus(float duration) : base(duration)
        {
            Type = PickableType.Shield;
        }
        
        protected override void RemoveBonus()
        {
            base.RemoveBonus();
            EventBus.Instance.Publish(EngineEventType.ShieldFadeOut);
        }
    }
}
