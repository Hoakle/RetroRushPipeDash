using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public class Magnet : Pickable
    {
        public override void SendEvent()
        {
            EventBus.Instance.Publish(EngineEventType.Magnet);
        }
    }
}
