using System;
using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class MainScreen : GraphicalUserInterface
    {
        [SerializeField] private Button _SettingsButton = null;
        [SerializeField] private Button _UpgradesButton = null;
        [SerializeField] private BestScore _BestScore = null;
        public override void OnReady()
        {
            _SettingsButton.onClick.AddListener(OpenSettings);
            _UpgradesButton.onClick.AddListener(OpenUpgrades);

            EventBus.Instance.Subscribe(EngineEventType.StartGame, Dispose);
            _GuiEngine.InitGUIComponent<BestScore>(_BestScore);
        }

        protected override void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Dispose);
            base.Dispose();
        }
        
        private void OpenSettings()
        {
            _GuiEngine.CreateDataGUI<SettingsGUI, SettingsGameSave>(GUIKeys.SETTINGS_SCREEN,
                _GuiEngine.GameSave.GetSave<SettingsGameSave>());
        }

        private void OpenUpgrades()
        {
            _GuiEngine.CreateDataGUI<UpgradeGUI, List<UpgradeData>>(GUIKeys.UPGRADE_GUI, _GuiEngine.GameSave.GetSave<GlobalGameSave>()._Upgrades);
        }
        private void OnDestroy()
        {
            _SettingsButton.onClick.RemoveListener(OpenSettings);
            _UpgradesButton.onClick.RemoveListener(OpenUpgrades);
        }
    }
}
