using HoakleEngine;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.Engine;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class BestScore : GuiComponent
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;
        [SerializeField] private Button _Button = null;

        private GlobalGameSave _GlobalGameSave;
        
        [Inject]
        public void Inject(GlobalGameSave globalGameSave)
        {
            globalGameSave.BestScore.Subscribe(UpdateScore);
        }
        
        public override void OnReady()
        {
            _Button.onClick.AddListener(OpenLeaderboard);
        }
        
        private void OnDestroy()
        {
            _Button.onClick.RemoveListener(OpenLeaderboard);
        }
        
        private void UpdateScore(long score)
        {
            m_Text.text = score.ToString();
        }

        private void OpenLeaderboard()
        {
            _GuiEngine.CreateDataGUI<LeaderboardGUI, LeaderboardData>(GUIKeys.LEADERBOARD_GUI, null);
        }
    }
}
