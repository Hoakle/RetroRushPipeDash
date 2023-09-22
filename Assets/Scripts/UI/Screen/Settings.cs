using HoakleEngine.Core.Graphics;
using RetroRush.Camera;
using RetroRush.Engine;
using RetroRush.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class Settings : DataGUI<CameraSettingsData>
    {
        [SerializeField] private Button m_ApplyButton = null;
        [SerializeField] private Transform m_Content = null;

        private SettingsComponent _YOffsetComponent;
        private SettingsComponent _ZOffsetComponent;
        private SettingsComponent _ZLookAtComponent;
        private void Start()
        {
             _GuiEngine.CreateDataGUIComponent<SettingsComponent, float>(GUIKeys.SETTINGS_COMPONENT, Data.YOffset, m_Content, component =>
             {
                 _YOffsetComponent = component;
                 _YOffsetComponent.Init("YOffset", 20f, 0f, 1f);
             });
             _GuiEngine.CreateDataGUIComponent<SettingsComponent, float>(GUIKeys.SETTINGS_COMPONENT, Data.ZOffset, m_Content, component =>
             {
                 _ZOffsetComponent = component;
                 _ZOffsetComponent.Init("ZOffset", 0f, -20f, 1f);
             });
             _GuiEngine.CreateDataGUIComponent<SettingsComponent, float>(GUIKeys.SETTINGS_COMPONENT, Data.YLookAtOffset, m_Content, component =>
             {
                 _ZLookAtComponent = component;
                 _ZLookAtComponent.Init("ZLookAtOffset", 20f, 0f, 0.5f);
             });
            
            m_ApplyButton.onClick.AddListener(ApplySettings);
        }
        
        private void OnDestroy()
        {
            m_ApplyButton.onClick.RemoveListener(ApplySettings);
        }
        
        private void ApplySettings()
        {
            Data.YOffset = _YOffsetComponent.Data;
            Data.ZOffset = _ZOffsetComponent.Data;
            Data.YLookAtOffset = _ZLookAtComponent.Data;
        }
    }
}