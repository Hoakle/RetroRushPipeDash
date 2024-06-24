using System;
using HoakleEngine.Core.Graphics;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public abstract class Pickable : GraphicalObjectRepresentation<PickableType>
    {
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
        SpeedBonus = 2,
        Magnet = 3,
        Shield = 4,
        CoinFactor = 5,
        StartBoost = 6,
        PickableSpawn = 7,
        Finish
    }
}
