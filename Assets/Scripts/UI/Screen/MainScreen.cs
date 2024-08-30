using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class MainScreen : GraphicalUserInterface
    {
        [SerializeField] private Button _SettingsButton = null;
        [SerializeField] private Button _UpgradesButton = null;
        [SerializeField] private Button _MissionsButton = null;
        [SerializeField] private BestScore _BestScore = null;

        [Header("GameMode")] 
        [SerializeField] private GameModeComponent _GameModeComponent = null;

        private ProgressionHandler _ProgressionHandler;
        private GlobalGameSave _GlobalGameSave;
        private SettingsGameSave _SettingsGameSave;

        [Inject]
        public void Inject(ProgressionHandler progressionHandler,
            GlobalGameSave globalGameSave,
            SettingsGameSave settingsGameSave)
        {
            _ProgressionHandler = progressionHandler;
            _GlobalGameSave = globalGameSave;
            _SettingsGameSave = settingsGameSave;
        }
        
        public override void OnReady()
        {
            _SettingsButton.onClick.AddListener(OpenSettings);
            _UpgradesButton.onClick.AddListener(OpenUpgrades);
            _MissionsButton.onClick.AddListener(OpenMissions);

            EventBus.Instance.Subscribe(EngineEventType.StartGame, Close);
            _GuiEngine.InitGUIComponent<BestScore>(_BestScore);
            _GuiEngine.InitDataGUIComponent<GameModeComponent, ProgressionHandler>(_GameModeComponent, _ProgressionHandler);
        }

        protected override void Close()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Close);
            base.Close();
        }
        
        private void OpenSettings()
        {
            _GuiEngine.CreateDataGUI<SettingsGUI, SettingsGameSave>(GUIKeys.SETTINGS_SCREEN,
                _SettingsGameSave);
        }

        private void OpenMissions()
        {
            _GuiEngine.CreateDataGUI<MissionsGUI, IReadOnlyList<MissionData>>(GUIKeys.MISSION_SCREEN,
                _GlobalGameSave.Missions);
        }
        private void OpenUpgrades()
        {
            _GuiEngine.CreateDataGUI<UpgradeGUI, IReadOnlyList<UpgradeData>>(GUIKeys.UPGRADE_GUI, _GlobalGameSave.Upgrades);
        }
        
        private void OnDestroy()
        {
            _SettingsButton.onClick.RemoveListener(OpenSettings);
            _UpgradesButton.onClick.RemoveListener(OpenUpgrades);
            _MissionsButton.onClick.RemoveListener(OpenMissions);
        }
    }
}
