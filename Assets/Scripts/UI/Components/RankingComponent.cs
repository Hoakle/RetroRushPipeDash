using HoakleEngine;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.GameData;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Components 
{
    public class RankingComponent : DataGuiComponent<ScoreData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private TextMeshProUGUI _Username = null;
        [SerializeField] private TextMeshProUGUI _Score = null;
        [SerializeField] private TextMeshProUGUI _Rank = null;
        [SerializeField] private Image _Icone = null;

        public bool IsOdd
        {
            set => _Animator.SetBool("IsOdd", value);
        }
        
        public bool IsUser
        {
            set => _Animator.SetBool("IsUser", value);
        }
        public override void OnReady()
        {
            UpdateInfo();
            base.OnReady();
        }

        private void UpdateInfo()
        {
            _Username.text = Data.UserId;
            _Score.text = Data.Score.ToString();
            _Rank.text = Data.Rank.ToString();
        }
    }
}
