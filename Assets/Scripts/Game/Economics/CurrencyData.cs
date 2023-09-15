using System;
using UnityEngine;

namespace RetroRush.Game.Economics
{
    public class CurrencyData
    {
        protected long _Value;
        public CurrencyType Type;
        public Action OnValueChange;
        public long Value
        {
            get => _Value;
            set
            {
                _Value = value;
                OnValueChange?.Invoke();
            }
        }
        
        public CurrencyData(CurrencyType type, long value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum CurrencyType
    {
        Coin,
    }
}
