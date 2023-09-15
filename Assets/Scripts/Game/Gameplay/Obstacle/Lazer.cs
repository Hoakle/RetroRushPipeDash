using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay.Obstacle
{
    public class Lazer : IObsacle
    {
        protected override void SendEvent()
        {
            EventBus.Instance.Publish(EngineEventType.GameOver);
        }
    }
}
