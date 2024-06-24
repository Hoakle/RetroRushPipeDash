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
using Zenject;

namespace RetroRush
{
    public class LevelComponent : DataGuiComponent<StageConfigData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private TextMeshProUGUI _LevelLabel = null;
        [SerializeField] private Button _Button = null;

        public Action<int> OnLevelSelected;
        
        private static readonly int Starts = Animator.StringToHash("Stars");
        private static readonly int IsAvailable = Animator.StringToHash("IsAvailable");
        private ProgressionHandler _ProgressionHandler;
        
        [Inject]
        public void Inject(ProgressionHandler progressionHandler)
        {
            _ProgressionHandler = progressionHandler;
        }
        
        public override void OnReady()
        {
            _Button.onClick.AddListener(OnClick);

            _LevelLabel.text = Data.Id.ToString();
            _Animator.SetBool(IsAvailable, _ProgressionHandler.MaxLevel >= Data.Id);

            base.OnReady();
        }

        public void SetStars()
        {
            var level = _ProgressionHandler.GetLevel(Data.Id);
            _Animator.SetInteger(Starts, level?.Stars ?? 0);
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
