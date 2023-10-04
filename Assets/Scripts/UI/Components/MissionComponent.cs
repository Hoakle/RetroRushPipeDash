using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.GameData;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Components 
{
    public class MissionComponent : DataGuiComponent<MissionData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private TextMeshProUGUI _Title = null;
        [SerializeField] private TextMeshProUGUI _Desc = null;
        
        public override void OnReady()
        {
            UpdateInfo();
            base.OnReady();
        }

        private void UpdateInfo()
        {
            _Animator.SetBool("IsCompleted", Data.IsCompleted);
            _Title.text = Data.Title;
            _Desc.text = Data.Description;
        }
    }
}
