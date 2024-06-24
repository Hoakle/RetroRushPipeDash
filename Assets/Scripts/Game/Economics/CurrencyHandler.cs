using System;
using HoakleEngine.Core.Game;
using UniRx;

namespace RetroRush.Game.Economics
{
    [Serializable]
    public class CurrencyHandler : GameSaveHandler<CurrencyData>
    {
        protected IReactiveProperty<long> _Amount = new ReactiveProperty<long>();
        public CurrencyType Type
            => _Data.Type;

        public IReadOnlyReactiveProperty<long> Amount => _Amount;

        public CurrencyHandler(CurrencyType type) : base(type.ToString())
        {
            _Data.Type = type;
        }

        protected override void BuildData()
        {
            base.BuildData();
            _Amount.Value = _Data.Value;
        }

        public bool TrySpend(int price)
        {
            if (HasEnough(price))
            {
                _Amount.Value -= price;
                Save();
                return true;
            }

            return false;
        }
        
        public bool HasEnough(long value)
        {
            return Amount.Value >= value;
        }
        
        public void Add(long value)
        {
            _Amount.Value += value;
            Save();
        }

        public override void Save()
        {
            _Data.Value = _Amount.Value;
            base.Save();
        }
    }

    public struct CurrencyData
    {
        public CurrencyType Type;
        public long Value;
    }

    public enum CurrencyType
    {
        Coin,
    }
}
