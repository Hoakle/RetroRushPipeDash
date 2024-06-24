using System;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Graphics;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Gameplay
{
    public interface IPickable
    {
        public void SendEvent();
        public void PlayAudio();
    }
    
    public abstract class Pickable : GraphicalObjectRepresentation<PickableType>, IPickable
    {
        protected IBonusMediator _BonusMediator;
        protected AudioPlayer _AudioPlayer;

        [Inject]
        public void Inject(IBonusMediator bonusMediator,
            AudioPlayer audioPlayer)
        {
            _BonusMediator = bonusMediator;
            _AudioPlayer = audioPlayer;
        }
        
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _IsReady)
            {
                PlayAudio();
                SendEvent();
                Dispose();
            }
        }
        
        public abstract void SendEvent();
        public abstract void PlayAudio();
    }

    public enum PickableType
    {
        None = 0,
        Coin = 1,
        SpeedBoost = 2,
        Magnet = 3,
        Shield = 4,
        CoinFactor = 5,
        StartBoost = 6,
        PickableSpawn = 7,
        Finish
    }
}
