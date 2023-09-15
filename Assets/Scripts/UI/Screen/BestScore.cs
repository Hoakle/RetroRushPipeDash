using System;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;

namespace RetroRush.UI.Screen
{
    public class BestScore : GraphicalUserInterface
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;

        public override void OnReady()
        {
            EventBus.Instance.Subscribe(EngineEventType.StartGame, Dispose);
            UpdateScore();
        }

        private void Dispose()
        {
            EventBus.Instance.UnSubscribe(EngineEventType.StartGame, Dispose);
            Destroy(gameObject);
        }
        private void UpdateScore()
        {
            GlobalGameSave save = _GuiEngine.GetEngine<GraphicsEngine>().GameSave.GetSave<GlobalGameSave>();
            m_Text.text = save.BestScore.ToString();
        }
        
    }
}
