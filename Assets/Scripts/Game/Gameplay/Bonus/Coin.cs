using System;
using HoakleEngine;
using HoakleEngine.Core.Audio;
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

        public override void PlayAudio()
        {
            AudioPlayer.Instance.Play(AudioKeys.CoinCollect);
            AudioPlayer.Instance.Play(AudioKeys.CoinCollect_02);
        }

        public override void Dispose()
        {
            MagnetForce = 10f;
            OnDispose?.Invoke(this);
            base.Dispose();
        }
    }
}
