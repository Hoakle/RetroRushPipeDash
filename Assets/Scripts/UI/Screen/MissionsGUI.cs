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

namespace RetroRush.UI.Screen
{
    public class MissionsGUI : DataGUI<List<MissionData>>
    {
        [SerializeField] private Button _Close = null;
        [SerializeField] private Transform _Content = null;
        [SerializeField] private TextMeshProUGUI _Factor = null;
        // Start is called before the first frame update
        public override void OnReady()
        {
            _Close.onClick.AddListener(Close);
            foreach (var mission in Data)
            {
                _GuiEngine.CreateDataGUIComponent<MissionComponent, MissionData>(GUIKeys.MISSION_COMPONENT, mission, _Content);
            }

            var gameSave = _GuiEngine.GameSave.GetSave<GlobalGameSave>();
            _Factor.text = gameSave.GetMultiplicator() + "/" + gameSave.Missions.Count;
            
            base.OnReady();
        }
        
        private void OnDestroy()
        {
            _Close.onClick.RemoveListener(Close);
        }
    }
}
