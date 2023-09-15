using System;
using System.Globalization;
using HoakleEngine.Core.Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Components
{
    public class SettingsComponent : DataGuiComponent<float>
    {
        [SerializeField] private TextMeshProUGUI m_NameText = null;
        [SerializeField] private TextMeshProUGUI m_Value = null;
        [SerializeField] private Button m_IncreaseButton = null;
        [SerializeField] private Button m_DecreaseButton = null;
    
        private float m_Max, m_Min, m_Step;
        public void Init(string name, float max, float min, float step)
        {
            m_NameText.text = name + ": ";
            m_Max = max;
            m_Min = min;
            m_Step = step;
        }
        void Start()
        {
            m_Value.text = Data.ToString(CultureInfo.CurrentCulture);
            m_IncreaseButton.onClick.AddListener(Increase);
            m_DecreaseButton.onClick.AddListener(Decrease);
        }

        private void OnDestroy()
        {
            m_IncreaseButton.onClick.RemoveListener(Increase);
            m_DecreaseButton.onClick.RemoveListener(Decrease);
        }
    
        private void Decrease()
        {
            Data = Math.Max(m_Min, Data - m_Step);
            m_Value.text = Data.ToString(CultureInfo.CurrentCulture);
        }

        private void Increase()
        {
            Data = Math.Min(m_Max, Data + m_Step);
            m_Value.text = Data.ToString(CultureInfo.CurrentCulture);
        }
    }
}
