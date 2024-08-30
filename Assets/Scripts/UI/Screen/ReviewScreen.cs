using HoakleEngine;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services.PlayServices;
using RetroRush.GameData;
using RetroRush.GameSave;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush
{
    public class ReviewScreen : GraphicalUserInterface
    {
        [SerializeField] private Button _ReviewButton = null;
        [SerializeField] private Button _NoThanksButton = null;

        private IPlayServicesTP _PlayServicesTp;
        private GlobalGameSave _GlobalGameSave;
        
        [Inject]
        public void Inject(IPlayServicesTP playServicesTp,
            GlobalGameSave globalGameSave)
        {
            _PlayServicesTp = playServicesTp;
            _GlobalGameSave = globalGameSave;
        }

        public override void OnReady()
        {
            _ReviewButton.onClick.AddListener(LaunchReview);
            _NoThanksButton.onClick.AddListener(Close);
            _GlobalGameSave.CompleteMission(MissionType.JY_FUS);
            base.OnReady();
        }

        private void LaunchReview()
        {
            _PlayServicesTp.LaunchReview();
            Close();
        }

        protected override void Close()
        {
            _ReviewButton.onClick.RemoveListener(LaunchReview);
            _NoThanksButton.onClick.RemoveListener(Close);
            base.Close();
        }
    }
}
