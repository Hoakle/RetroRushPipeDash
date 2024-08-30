using System;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using RetroRush.Game.Economics;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Components
{
    public class ToogleButton : DataGuiComponent<bool>
    {
        [SerializeField] private Button _Button = null;
        [SerializeField] private LocalizedText _ToogleText = null;
        
        public Action<bool> OnToogleChange;
        public override void OnReady()
        {
            UpdateText();
            
            _Button.onClick.AddListener(Toogle);
            base.OnReady();
        }

        private void OnDisable()
        {
            _Button.onClick.RemoveListener(Toogle);
        }

        private void UpdateText()
        {
            _ToogleText.SetKey(Data ? "Generic/On" : "Generic/Off");
        }

        private void Toogle()
        {
            Data = !Data;
            OnToogleChange?.Invoke(Data);
            UpdateText();
        }
        
    }
    
}
