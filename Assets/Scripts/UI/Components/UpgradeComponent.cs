using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using RetroRush.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush
{
    public class UpgradeComponent : DataGuiComponent<UpgradeData>
    {
        [SerializeField] private TextMeshProUGUI _Title = null;
        [SerializeField] private TextMeshProUGUI _Desc = null;
        [SerializeField] private Image _Icone = null;
        [SerializeField] private CurrencyButton _UpgradeButton = null;

        private UpgradeConfigData _Config;
        // Start is called before the first frame update
        void Start()
        {
            _Config = _GuiEngine.ConfigContainer.GetConfig<GameplayConfigData>().GetUpgradeConfig(Data.Type);
            
            _GuiEngine.InitDataGUIComponent<CurrencyButton, CurrencyData>(_UpgradeButton, _GuiEngine.GameSave.GetSave<GlobalGameSave>().Wallet.Get(CurrencyType.Coin));
            _UpgradeButton.OnBuy += Upgrade;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            _Title.text = _Config.Title;
            _Desc.text = _Config.Desc.Replace("{0}", _Config.GetValue(Data.Level).ToString());
            _Icone.sprite = _Config.Icone;
            _UpgradeButton.SetPrice(_Config.GetUpgradePrice(Data.Level));
        }
        private void Upgrade()
        {
            Data.Level++;
            UpdateInfo();
        }
    }
}
