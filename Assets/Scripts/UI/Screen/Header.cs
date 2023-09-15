using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using UnityEngine;

namespace RetroRush.UI.Screen
{
    public class Header : GraphicalUserInterface
    {
        [SerializeField] private CurrencyComponent _CoinComponent = null;

        private GlobalGameSave _GlobalSave;
        public void Start()
        {
            _GlobalSave = _GuiEngine.GetEngine<GraphicsEngine>().GameSave.GetSave<GlobalGameSave>();
            _GuiEngine.InitDataGUIComponent<CurrencyComponent, CurrencyData>(_CoinComponent, _GlobalSave.Wallet.Get(CurrencyType.Coin));
        }
    }
}
