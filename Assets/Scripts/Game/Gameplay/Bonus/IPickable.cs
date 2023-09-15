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
                SendEvent();
                Dispose();
            }
                
        }

        public abstract void SendEvent();
    }

    public enum PickableType
    {
        None = 0,
        Coin = 1,
        SpeedBonus = 2,
        Magnet = 3,
        Shield = 4,
    }
}
