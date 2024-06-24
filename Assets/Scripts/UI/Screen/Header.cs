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

        private GlobalGameSave _GlobalSave;
        private CurrencyHandler _CoinCurrencyHandler;

        [Inject]
        public void Inject(GlobalGameSave gameSave,
            [Inject (Id = CurrencyType.Coin)] CurrencyHandler coinHandler)
        {
            _GlobalSave = gameSave;
            _CoinCurrencyHandler = coinHandler;
        }
        
        public override void OnReady()
        {
            base.OnReady();
            
            _GuiEngine.InitDataGUIComponent<CurrencyComponent, CurrencyHandler>(_CoinComponent, _CoinCurrencyHandler);
        }
    }
}
