using System;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Components
{
    public class CurrencyButton : DataGuiComponent<CurrencyData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] protected TextMeshProUGUI _Text = null;
        [SerializeField] private Button _Button = null;
        public Action OnBuy;
        
        private Wallet _Wallet = null;
        private int _Price;
        private bool HasEnough => Data.Value >= _Price;
        public override void OnReady()
        {
            _Wallet = _GuiEngine.GameSave.GetSave<GlobalGameSave>().Wallet;
            
            _Button.onClick.AddListener(TryBuy);
            Data.OnValueChange += UpdateValue;
            UpdateValue();
        }

        public void OnDestroy()
        {
            Data.OnValueChange -= UpdateValue;
        }

        public void SetPrice(int price)
        {
            _Price = price;
            _Text.text = _Price.ToString();
            UpdateValue();
        }
        private void UpdateValue()
        {
            _Animator.SetBool("HasEnough", HasEnough);
        }

        private void TryBuy()
        {
            if (_Wallet.TrySpend(Data.Type, _Price))
            {
                OnBuy?.Invoke();
            }
        }
    }
}
