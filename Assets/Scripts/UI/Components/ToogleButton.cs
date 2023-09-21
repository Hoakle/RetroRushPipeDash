using System;
using HoakleEngine.Core.Graphics;
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
        [SerializeField] private TextMeshProUGUI _ToogleText = null;
        
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
            _ToogleText.text = Data ? "On" : "Off";
        }

        private void Toogle()
        {
            Debug.LogError("Toogle");
            Data = !Data;
            OnToogleChange?.Invoke(Data);
            UpdateText();
        }
        
    }
    
}
