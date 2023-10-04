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
        [SerializeField] private Button _MissionsButton = null;
        [SerializeField] private BestScore _BestScore = null;
        public override void OnReady()
        {
            _SettingsButton.onClick.AddListener(OpenSettings);
            _UpgradesButton.onClick.AddListener(OpenUpgrades);
            _MissionsButton.onClick.AddListener(OpenMissions);

            EventBus.Instance.Subscribe(EngineEventType.StartGame, Close);
            _GuiEngine.InitGUIComponent<BestScore>(_BestScore);
        }

        protected override void Close()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Close);
            base.Close();
        }
        
        private void OpenSettings()
        {
            _GuiEngine.CreateDataGUI<SettingsGUI, SettingsGameSave>(GUIKeys.SETTINGS_SCREEN,
                _GuiEngine.GameSave.GetSave<SettingsGameSave>());
        }

        private void OpenMissions()
        {
            _GuiEngine.CreateDataGUI<MissionsGUI, List<MissionData>>(GUIKeys.MISSION_SCREEN,
                _GuiEngine.GameSave.GetSave<GlobalGameSave>().Missions);
        }
        private void OpenUpgrades()
        {
            _GuiEngine.CreateDataGUI<UpgradeGUI, List<UpgradeData>>(GUIKeys.UPGRADE_GUI, _GuiEngine.GameSave.GetSave<GlobalGameSave>().Upgrades);
        }
        private void OnDestroy()
        {
            _SettingsButton.onClick.RemoveListener(OpenSettings);
            _UpgradesButton.onClick.RemoveListener(OpenUpgrades);
            _MissionsButton.onClick.RemoveListener(OpenMissions);
        }
    }
}
