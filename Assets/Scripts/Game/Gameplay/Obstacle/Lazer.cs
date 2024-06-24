using HoakleEngine.Core.Communication;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Gameplay.Obstacle
{
    public class Lazer : IObsacle
    {
        private IGameState _GameState;
        private IBonusMediator _BonusMediator;
        
        [Inject]
        public void Inject(IGameState gameState,
            IBonusMediator bonusMediator)
        {
            _GameState = gameState;
            _BonusMediator = bonusMediator;
        }
        
        protected override void SendEvent()
        {
            if(!_BonusMediator.HasBonus(PickableType.Shield) && !_BonusMediator.HasBonus(PickableType.StartBoost))
                _GameState.SetState(State.PlayerDied);
        }
    }
}
