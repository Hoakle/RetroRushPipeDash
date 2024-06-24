using HoakleEngine;
using HoakleEngine.Core.Audio;
using HoakleEngine.Core.Communication;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Gameplay
{
    public class Shield : Pickable
    {
        public override void SendEvent()
        {
            _BonusMediator.CollectPickable(PickableType.Shield);
        }
        
        public override void PlayAudio()
        {
            _AudioPlayer.Play(AudioKeys.BonusCollect);
        }
    }
}
