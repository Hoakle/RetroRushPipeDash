using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.GameData;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Components 
{
    public class UpgradeComponent : DataGuiComponent<UpgradeData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private LocalizedText _Title = null;
        [SerializeField] private LocalizedText _Desc = null;
        [SerializeField] private Image _Icone = null;
        [SerializeField] private CurrencyButton _UpgradeButton = null;

        private UpgradeConfigData _Config;
        private CurrencyHandler _CoinCurrencyHandler;
        private GlobalGameSave _GlobalGameSave;
        private GameplayConfigData _GameplayConfigData;
        
        [Inject]
        public void Inject([Inject (Id = CurrencyType.Coin)] CurrencyHandler coinHandler,
            GameplayConfigData gameplayConfigData,
            GlobalGameSave globalGameSave)
        {
            _CoinCurrencyHandler = coinHandler;
            _GlobalGameSave = globalGameSave;
            _GameplayConfigData = gameplayConfigData;
        }
        
        public override void OnReady()
        {
            _Config = _GameplayConfigData.GetUpgradeConfig(Data.Type);
            _GuiEngine.InitDataGUIComponent<CurrencyButton, CurrencyHandler>(_UpgradeButton, _CoinCurrencyHandler);
            _UpgradeButton.OnBuy += Upgrade;

            UpdateInfo();
            base.OnReady();
        }

        private void UpdateInfo()
        {
            _Animator.SetBool("IsMaxRank", Data.Level == _Config.MaxLevel);
            _Title.SetKey(_Config.Title);
            _Desc.SetKey(_Config.Desc);
            _Desc.SetParameters( _Config.GetValue(Data.Level).ToString(), _Config.GetFactor(Data.Level).ToString());
            _Icone.sprite = _Config.Icone;
            
            if(Data.Level != _Config.MaxLevel)
                _UpgradeButton.SetPrice(_Config.GetUpgradePrice(Data.Level));
        }
        
        private void Upgrade()
        {
            Data.Level++;
            UpdateInfo();
            _GlobalGameSave.Save();
        }
    }
}
