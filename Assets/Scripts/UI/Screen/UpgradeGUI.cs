using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class UpgradeGUI : DataGUI<List<UpgradeData>>
    {
        [SerializeField] private Button m_Close = null;
        [SerializeField] private Transform m_Content = null;
        // Start is called before the first frame update
        public override void OnReady()
        {
            m_Close.onClick.AddListener(Close);
            foreach (var upgrade in Data)
            {
                _GuiEngine.CreateDataGUIComponent<UpgradeComponent, UpgradeData>(GUIKeys.UPGRADE_COMPONENT, upgrade, m_Content);
            }
            
            base.OnReady();
        }

        private void Close()
        {
            Dispose();
        }
        private void OnDestroy()
        {
            m_Close.onClick.RemoveListener(Close);
        }
    }
}
