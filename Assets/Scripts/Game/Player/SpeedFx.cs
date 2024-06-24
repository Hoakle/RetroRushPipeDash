using HoakleEngine.Addons;
using RetroRush.Game.Gameplay;
using UniRx;
using Zenject;

namespace RetroRush.Game.Player
{
    public class SpeedFx : MeshTrail
    {
        [Inject]
        public void Inject(IBonusMediator bonusMediator)
        {
            bonusMediator.OnBonusStarted
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.SpeedBoost or PickableType.StartBoost)
                .Subscribe(_ => IsActive = true);
            
            bonusMediator.OnBonusFadeOut
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.SpeedBoost or PickableType.StartBoost)
                .Subscribe(_ => IsActive = false);
        }
    }
}
