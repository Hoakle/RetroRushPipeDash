using System;
using HoakleEngine.Core.Game;
using UnityEngine;

namespace RetroRush.Game.Economics
{
    [Serializable]
    public class CurrencyData : GameSaveData
    {
        [SerializeField]
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
