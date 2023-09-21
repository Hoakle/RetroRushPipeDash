using HoakleEngine;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using UnityEngine;

namespace RetroRush.Game.Gameplay
{
    public class SpeedBoost : Pickable
    {
        public override void SendEvent()
        {
            EventBus.Instance.Publish(EngineEventType.SpeedBonus);
        }
        
        public override void PlayAudio()
        {
            AudioPlayer.Instance.Play(AudioKeys.BonusCollect);
        }
    }
}
