using System;
using System.Collections.Generic;
using HoakleEngine.Core.Game;
using UnityEngine;

namespace RetroRush.Game.Economics
{
    [Serializable]
    public class Wallet : GameSaveData
    {
        private Dictionary<CurrencyType, CurrencyData> _Currencies;
        public List<CurrencyType> _CurrencyTypes = new List<CurrencyType>();
        public List<CurrencyData> _CurrencyDatas = new List<CurrencyData>();
        public void Init()
        {
            _Currencies = new Dictionary<CurrencyType, CurrencyData>();
            
            if(!_CurrencyTypes.Contains(CurrencyType.Coin))
            {
                _CurrencyTypes.Add(CurrencyType.Coin);
                _CurrencyDatas.Add(new CurrencyData(CurrencyType.Coin, 0));
            }
            
            foreach (var currency in _CurrencyTypes)
            {
                _Currencies.Add(currency, _CurrencyDatas.Find(c => c.Type == currency));
            }
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
