using System;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Economics;
using RetroRush.Game.Level;
using RetroRush.GameSave;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class Score : DataGUI<LevelDesignData>
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;

        [Inject]
        public void Inject(IGameState gameState,
            LevelDesignData levelDesignData)
        {
            gameState.State
                .Where(state => state == State.GameOver)
                .TakeUntilDestroy(this)
                .Subscribe(_ => Close());

            levelDesignData.Score.Subscribe(UpdateScore);
        }

        private void UpdateScore(long score)
        {
            m_Text.text = score.ToString();
        }
    }
}
