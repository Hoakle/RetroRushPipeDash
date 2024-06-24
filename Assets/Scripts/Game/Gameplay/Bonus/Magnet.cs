using HoakleEngine.Core.Audio;
using Zenject;

namespace RetroRush.Game.Gameplay
{
    public class Magnet : Pickable
    {
        public override void SendEvent()
        {
            _BonusMediator.CollectPickable(PickableType.Magnet);
        }
        
        public override void PlayAudio()
        {
            _AudioPlayer.Play(AudioKeys.BonusCollect);
        }
    }
}
