using System;
using HoakleEngine;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using RetroRush.Game.Level;
using RetroRush.GameSave;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Gameplay
{
    public class Coin : Pickable
    {
        public new Action<Coin> OnDispose;

        public float MagnetForce = 10f;
        
        private LevelDesignData _LevelDesignData;
        
        [Inject]
        public void Inject(
            LevelDesignData levelDesignData)
        {
            _LevelDesignData = levelDesignData;
        }
        
        public override void SendEvent()
        {
            _LevelDesignData.CollectCoin();
        }

        public override void PlayAudio()
        {
            _AudioPlayer.Play(AudioKeys.CoinCollect);
            _AudioPlayer.Play(AudioKeys.CoinCollect_02);
        }

        public override void Dispose()
        {
            MagnetForce = 10f;
            OnDispose?.Invoke(this);
            base.Dispose();
        }
    }
}
