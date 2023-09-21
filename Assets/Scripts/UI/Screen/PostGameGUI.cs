using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.Game.Level;
using RetroRush.GameData;
using RetroRush.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class PostGameGUI : DataGUI<LevelData>
    {
        [SerializeField] private Button _Continue = null;
        [SerializeField] private TextMeshProUGUI _Score = null;
        // Start is called before the first frame update
        public override void OnReady()
        {
            _Continue.onClick.AddListener(BackToMenu);

            _Score.text = Data.Score.ToString();
            
            base.OnReady();
        }

        protected override void Dispose()
        {
            _Continue.onClick.RemoveListener(BackToMenu);
            base.Dispose();
        }

        private void BackToMenu()
        {
            EventBus.Instance.Publish(EngineEventType.BackToMenu);
            Dispose();
        }
    }
}
