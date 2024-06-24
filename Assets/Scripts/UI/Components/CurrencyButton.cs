using System;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Components
{
    public class CurrencyButton : DataGuiComponent<CurrencyHandler>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] protected TextMeshProUGUI _Text = null;
        [SerializeField] private Button _Button = null;
        
        public Action OnBuy;
        private int _Price;

        public override void OnReady()
        {
            _Button.onClick.AddListener(TryBuy);
            
            Data.Amount
                .TakeUntilDestroy(this)
                .Subscribe(UpdateValue);
        }

        public void SetPrice(int price)
        {
            _Price = price;
            _Text.text = _Price.ToString();
            UpdateValue(Data.Amount.Value);
        }
        
        private void UpdateValue(long value)
        {
            _Animator.SetBool("HasEnough", value >= _Price);
        }

        private void TryBuy()
        {
            if (Data.TrySpend(_Price))
            {
                OnBuy?.Invoke();
            }
        }
    }
}
