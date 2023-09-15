using System;
using System.Collections.Generic;
using UnityEngine;

namespace RetroRush.Game.Economics
{
    [Serializable]
    public class Wallet
    {
        private Dictionary<CurrencyType, CurrencyData> _Currencies;

        public void Init()
        {
            _Currencies = new Dictionary<CurrencyType, CurrencyData>();
            _Currencies.Add(CurrencyType.Coin, new CurrencyData(CurrencyType.Coin, 0));
        }
        protected bool Exist(CurrencyType type)
        {
            if (_Currencies.ContainsKey(type))
                return true;
            else
            {
                Debug.LogError("Currency " + type + " doesn't exist!");
                return false;
            }
        }

        public bool HasEnough(CurrencyType type, long value)
        {
            if (Exist(type))
                return _Currencies[type].Value >= value;

            return false;
        }
        
        public void Add(CurrencyType type, long value)
        {
            if (Exist(type))
            {
                _Currencies[type].Value += value;
            }
        }

        public CurrencyData Get(CurrencyType type)
        {
            if (Exist(type))
            {
                return _Currencies[type];
            }

            return null;
        }
        
        public bool TrySpend(CurrencyType type, long value)
        {
            if(HasEnough(type, value))
            {
                _Currencies[type].Value -= value;
                return true;
            }

            return false;
        }
    }
}
