using HoakleEngine.Core.Audio;

namespace RetroRush.Game.Gameplay
{
    public class SpeedBoost : Pickable
    {
        public override void SendEvent()
        {
            _BonusMediator.CollectPickable(PickableType.SpeedBoost);
        }
        
        public override void PlayAudio()
        {
            _AudioPlayer.Play(AudioKeys.BonusCollect);
        }
    }
}
