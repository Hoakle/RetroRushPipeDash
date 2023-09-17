using System;
using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class MainScreen : GraphicalUserInterface
    {
        [SerializeField] private Button m_StartButton = null;
        [SerializeField] private Button m_SettingsButton = null;
        [SerializeField] private Button m_UpgradesButton = null;
        
        private void Start()
        {
            m_StartButton.onClick.AddListener(StartGame);
            m_SettingsButton.onClick.AddListener(OpenSettings);
            m_UpgradesButton.onClick.AddListener(OpenUpgrades);
        }

        private void StartGame()
        {
            EventBus.Instance.Publish(EngineEventType.StartGame);
            Destroy(gameObject);
        }
    
        private void OpenSettings()
        {
        
        }

        private void OpenUpgrades()
        {
            _GuiEngine.CreateDataGUI<UpgradeGUI, List<UpgradeData>>(GUIKeys.UPGRADE_GUI, _GuiEngine.GameSave.GetSave<GlobalGameSave>()._Upgrades);
        }
        private void OnDestroy()
        {
            m_StartButton.onClick.RemoveListener(StartGame);
            m_SettingsButton.onClick.RemoveListener(OpenSettings);
            m_UpgradesButton.onClick.RemoveListener(OpenUpgrades);
        }
    }
}
