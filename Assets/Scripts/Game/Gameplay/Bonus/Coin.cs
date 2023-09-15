using System;
using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public class Coin : Pickable
    {
        public new Action<Coin> OnDispose;

        public float MagnetForce = 10f;
        public override void SendEvent()
        {
            EventBus.Instance.Publish(EngineEventType.Coin);
        }

        public override void Dispose()
        {
            MagnetForce = 10f;
            OnDispose?.Invoke(this);
            base.Dispose();
        }
    }
}
