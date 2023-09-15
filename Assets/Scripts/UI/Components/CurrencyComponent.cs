using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using TMPro;
using UnityEngine;

namespace RetroRush
{
    public class CurrencyComponent : DataGuiComponent<CurrencyData>
    {
        [SerializeField] protected TextMeshProUGUI _Text = null;

        public void Start()
        {
            Data.OnValueChange += UpdateValue;
            UpdateValue();
        }

        public void OnDestroy()
        {
            Data.OnValueChange -= UpdateValue;
        }

        private void UpdateValue()
        {
            _Text.text = Data.Value.ToString();
        }
    }
}
