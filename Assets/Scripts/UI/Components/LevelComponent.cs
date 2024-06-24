using System;
using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.GameData;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RetroRush
{
    public class LevelComponent : DataGuiComponent<StageConfigData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private TextMeshProUGUI _LevelLabel = null;
        [SerializeField] private Button _Button = null;

        public Action<int> OnLevelSelected;
        
        private static readonly int Starts = Animator.StringToHash("Starts");
        
        public override void OnReady()
        {
            _Button.onClick.AddListener(OnClick);

            _LevelLabel.text = Data.Id.ToString();
            
            var level = _GuiEngine.GameSave.GetSave<GlobalGameSave>().GameMode.GetLevel(Data.Id);
            _Animator.SetInteger(Starts, level?.Stars ?? 0);
            
            base.OnReady();
        }

        private void OnDestroy()
        {
            _Button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            OnLevelSelected?.Invoke(Data.Id);
        }
        
    }
}
