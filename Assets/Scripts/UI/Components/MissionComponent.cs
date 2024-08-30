using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using RetroRush.Config;
using RetroRush.GameData;
using UnityEngine;
using Zenject;

namespace RetroRush.UI.Components 
{
    public class MissionComponent : DataGuiComponent<MissionData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private LocalizedText _Title = null;
        [SerializeField] private LocalizedText _Desc = null;

        [Inject]
        private MissionConfigData _MissionConfigData;
        
        
        public void Inject(GameplayConfigData gameplayConfigData)
        {
            _MissionConfigData = gameplayConfigData.GetMission(Data.Type);
        }
        public override void OnReady()
        {
            UpdateInfo();
            base.OnReady();
        }

        private void UpdateInfo()
        {
            _Animator.SetBool("IsCompleted", Data.IsCompleted);
            _Title.SetKey(_MissionConfigData.Key + "/Title");
            _Desc.SetKey(_MissionConfigData.Key);
        }
    }
}
