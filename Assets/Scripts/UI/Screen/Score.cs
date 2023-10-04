using System;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Level;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;

namespace RetroRush
{
    public class Score : DataGUI<LevelData>
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;

        public override void OnReady()
        {
            EventBus.Instance.Subscribe(EngineEventType.GameOver, Close);
        }
        
        protected override void Close()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.GameOver, Close);
            base.Close();
        }

        public void Update()
        {
            m_Text.text = Data.Score.ToString();
        }
    }
}
