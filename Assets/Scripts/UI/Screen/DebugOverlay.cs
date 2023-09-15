using HoakleEngine.Core.Graphics;
using RetroRush.Camera;
using RetroRush.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class DebugOverlay : GraphicalUserInterface
    {
        [SerializeField] private Button m_SettingsButton = null;

        private void Start()
        {
            m_SettingsButton.onClick.AddListener(OpenSettings);
        }

        private void OpenSettings()
        {
            _GuiEngine.CreateDataGUI<Settings, CameraSettingsData>(GUIKeys.SETTINGS_SCREEN, ((ThirdPersonCameraControl)_GuiEngine.GetEngine<GraphicsEngine>().CameraControl).CameraSettingsData);
        }

        private void OnDestroy()
        {
            m_SettingsButton.onClick.RemoveListener(OpenSettings);
        }
    }
}