using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using UnityEngine;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class Header : GraphicalUserInterface
    {
        [SerializeField] private CurrencyComponent _CoinComponent = null;
        [SerializeField] private CurrencyComponent _CpuComponent = null;

        private GlobalGameSave _GlobalSave;
        private CurrencyHandler _CpuCurrencyHandler;
        private CurrencyHandler _CoinCurrencyHandler;

        [Inject]
        public void Inject(
            [Inject (Id = CurrencyType.Coin)] CurrencyHandler coinHandler,
            [Inject (Id = CurrencyType.CPU)] CurrencyHandler cpuHandler)
        {
            _CoinCurrencyHandler = coinHandler;
            _CpuCurrencyHandler = cpuHandler;
        }
        
        public override void OnReady()
        {
            base.OnReady();
            
            _GuiEngine.InitDataGUIComponent<CurrencyComponent, CurrencyHandler>(_CoinComponent, _CoinCurrencyHandler);
            _GuiEngine.InitDataGUIComponent<CurrencyComponent, CurrencyHandler>(_CpuComponent, _CpuCurrencyHandler);
        }
    }
}
