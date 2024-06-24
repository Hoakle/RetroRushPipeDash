using System;
using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class MissionsGUI : DataGUI<IReadOnlyList<MissionData>>
    {
        [SerializeField] private Button _Close = null;
        [SerializeField] private Transform _Content = null;
        [SerializeField] private TextMeshProUGUI _Factor = null;

        [Inject]
        public void Inject(GlobalGameSave globalGameSave)
        {
            _Factor.text = "x" + globalGameSave.GetMultiplicator();
        }
        
        public override void OnReady()
        {
            _Close.onClick.AddListener(Close);
            foreach (var mission in Data)
            {
                _GuiEngine.CreateDataGUIComponent<MissionComponent, MissionData>(GUIKeys.MISSION_COMPONENT, mission, _Content);
            }
          
            base.OnReady();
        }
        
        private void OnDestroy()
        {
            _Close.onClick.RemoveListener(Close);
        }
    }
}
