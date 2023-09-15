using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class UpgradeGUI : DataGUI<List<UpgradeData>>
    {
        [SerializeField] private Button m_Close = null;
        [SerializeField] private Transform m_Content = null;
        // Start is called before the first frame update
        void Start()
        {
            m_Close.onClick.AddListener(Close);
            foreach (var upgrade in Data)
            {
                AddGuiComponent<UpgradeComponent, UpgradeData>(GUIKeys.UPGRADE_COMPONENT, upgrade, m_Content);
            }
            
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
