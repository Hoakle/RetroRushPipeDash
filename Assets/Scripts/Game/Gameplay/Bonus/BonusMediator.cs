using System.Collections.Generic;
using HoakleEngine.Core.Game;
using RetroRush.GameData;
using RetroRush.GameSave;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Gameplay
{
    public interface IBonusMediator
    {
        void CollectPickable(PickableType pickable);
        bool HasBonus(PickableType shield);
        IReadOnlyReactiveProperty<Bonus> OnBonusFadeOut { get; }
        IReadOnlyReactiveProperty<Bonus> OnBonusStarted { get; }
    }
    
    public class BonusMediator : IBonusMediator, ITickable
    {
        public IReadOnlyReactiveProperty<Bonus> OnBonusFadeOut
            => FadeOut;

        public IReadOnlyReactiveProperty<Bonus> OnBonusStarted
            => Started;
        
        private IBonusFactory _BonusFactory;
        private GlobalGameSave _GlobalGameSave;
        private Dictionary<PickableType, Bonus> _BonusDictionary = new Dictionary<PickableType, Bonus>();
        private List<PickableType> _UniqueBonusCollected = new List<PickableType>();
        private List<PickableType> _KeyToBeRemoved = new List<PickableType>();

        private IReactiveProperty<Bonus> FadeOut = new ReactiveProperty<Bonus>();
        private IReactiveProperty<Bonus> Started = new ReactiveProperty<Bonus>();

        [Inject]
        public void Inject(
            IGameState gameState,
            IBonusFactory bonusFactory,
            IGameEngine gameEngine,
            GlobalGameSave globalGameSave)
        {
            _BonusFactory = bonusFactory;
            _GlobalGameSave = globalGameSave;
            
            gameState.State
                .SkipLatestValueOnSubscribe()
                .Where(state => state == State.GameOver)
                .Subscribe(_ => ClearBonus());
        }
        
        public void CollectPickable(PickableType pickable)
        {
            if(!_UniqueBonusCollected.Contains(pickable))
                _UniqueBonusCollected.Add(pickable);
            
            if (_BonusDictionary.ContainsKey(pickable))
            {
                _BonusDictionary[pickable].RefreshDuration();
            }
            else
            {
                var bonus = _BonusFactory.GetBonus(pickable);
                bonus.OnEnd += RemoveBonus;
                _BonusDictionary.Add(pickable, bonus);
                Started.Value = bonus;
            }
        }

        private void RemoveBonus(Bonus bonus)
        {
            bonus.OnEnd -= RemoveBonus;
            _KeyToBeRemoved.Add(bonus.Type);
            FadeOut.Value = bonus;
        }

        private void ClearBonus()
        {
            if(_UniqueBonusCollected.Count >= 3)
                _GlobalGameSave.CompleteMission(MissionType.BOOST_COLLECTOR);
            
            foreach (var bonus in _BonusDictionary)
            {
                bonus.Value.OnEnd.Invoke(bonus.Value);
            }
            
            _UniqueBonusCollected.Clear();
        }
        
        public bool HasBonus(PickableType type)
        {
            return _BonusDictionary.ContainsKey(type);
        }

        public void Tick()
        {
            foreach (var bonus in _BonusDictionary)
            {
                bonus.Value.Tick();
            }

            foreach (var key in _KeyToBeRemoved)
            {
                _BonusDictionary.Remove(key);
            }
            
            _KeyToBeRemoved.Clear();
        }
    }
}
