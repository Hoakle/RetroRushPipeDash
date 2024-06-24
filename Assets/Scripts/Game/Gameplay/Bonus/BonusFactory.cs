using System;
using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using Zenject;

namespace RetroRush
{
    public interface IBonusFactory
    {
        public Bonus GetBonus(PickableType type);
    }
    
    public class BonusFactory : IBonusFactory
    {
        private DiContainer _Container;
        
        [Inject]
        public void Inject(DiContainer container)
        {
            _Container = container;
        }
        
        public Bonus GetBonus(PickableType type)
        {
            Bonus bonus;
            switch (type)
            {
                case PickableType.Magnet:
                    bonus = new MagnetBonus();
                    _Container.Inject(bonus);
                    return bonus;
                case PickableType.Shield:
                    bonus = new ShieldBonus();
                    _Container.Inject(bonus);
                    return bonus;
                case PickableType.CoinFactor:
                    bonus = new CoinFactorBonus();
                    _Container.Inject(bonus);
                    return bonus;
                case PickableType.SpeedBoost:
                    bonus = new SpeedBonus();
                    _Container.Inject(bonus);
                    return bonus;
                case PickableType.StartBoost:
                    bonus = new StartBonus();
                    _Container.Inject(bonus);
                    return bonus;
            }

            return null;
        }
    }
}
