using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public class Shield : Pickable
    {
        public override void SendEvent()
        {
            EventBus.Instance.Publish(EngineEventType.Shield);
        }
    }
}
