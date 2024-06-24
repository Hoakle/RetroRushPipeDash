using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class CurrencyComponent : DataGuiComponent<CurrencyHandler>
    {
        [SerializeField] protected TextMeshProUGUI _Text = null;

        [Inject]
        public void Inject(
            [Inject (Id = CurrencyType.Coin)] CurrencyHandler coinCurrency)
        {
            coinCurrency.Amount.Subscribe(UpdateValue);
        }

        public override void OnReady()
        {
            UpdateValue(Data.Amount.Value);
        }

        private void UpdateValue(long amount)
        {
            _Text.text = amount.ToString();
        }
    }
}
