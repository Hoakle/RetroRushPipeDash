using RetroRush.Game.Gameplay;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Level
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameState>().AsSingle();
            Container.BindInterfacesTo<BonusMediator>().AsSingle();
            Container.BindInterfacesTo<BonusFactory>().AsSingle();
        }
    }
}
