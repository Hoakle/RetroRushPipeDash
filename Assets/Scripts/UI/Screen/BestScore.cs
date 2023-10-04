using System;
using HoakleEngine;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class BestScore : GuiComponent
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;
        [SerializeField] private Button _Button = null;
        public override void OnReady()
        {
            _Button.onClick.AddListener(OpenLeaderboard);
            UpdateScore();
        }
        
        private void OnDestroy()
        {
            _Button.onClick.RemoveListener(OpenLeaderboard);
        }
        private void UpdateScore()
        {
            GlobalGameSave save = _GuiEngine.GetEngine<GraphicsEngine>().GameSave.GetSave<GlobalGameSave>();
            m_Text.text = save.BestScore.ToString();
        }

        private void OpenLeaderboard()
        {
            _GuiEngine.CreateDataGUI<LeaderboardGUI, LeaderboardData>(GUIKeys.LEADERBOARD_GUI, null);
        }
    }
}
