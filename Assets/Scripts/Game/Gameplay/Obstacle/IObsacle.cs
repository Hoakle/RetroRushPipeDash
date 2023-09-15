using HoakleEngine.Core.Graphics;
using UnityEngine;

namespace RetroRush.Game.Gameplay.Obstacle
{
    public abstract class IObsacle : GraphicalObjectRepresentation<ObstacleType>
    {
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _IsReady)
            {
                SendEvent();
            }
            
        }

        protected abstract void SendEvent();
    }

    public enum ObstacleType
    {
        None = 0,
        Lazer = 1,
    }
}
