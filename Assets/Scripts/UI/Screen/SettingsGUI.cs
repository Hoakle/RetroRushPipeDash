using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services;
using RetroRush.UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class SettingsGUI : DataGUI<SettingsGameSave>
    {
        [SerializeField] private Button _CloseButton = null;
        [SerializeField] private ToogleButton _MusicToogle = null;
        [SerializeField] private ToogleButton _SfxToogle = null;
        [SerializeField] private Button _ContactUs = null;

        private MiscThirdPartyService _MiscTP;
        
        [Inject]
        public void Inject(MiscThirdPartyService miscTp)
        {
            _MiscTP = miscTp;
        }
        
        public override void OnReady()
        {
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_MusicToogle, Data.HasMusic.Value);
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_SfxToogle, Data.HasSfx.Value);
            
            _CloseButton.onClick.AddListener(Close);
            _MusicToogle.OnToogleChange += ToogleMusic;
            _SfxToogle.OnToogleChange += ToogleSfx;
            
            _ContactUs.onClick.AddListener(ContactUs);

            base.OnReady();
        }

        protected override void Close()
        {
            _CloseButton.onClick.RemoveListener(Close);
            _MusicToogle.OnToogleChange -= ToogleMusic;
            _SfxToogle.OnToogleChange -= ToogleSfx;
            
            _ContactUs.onClick.RemoveListener(ContactUs);
            base.Close();
        }

        private void ToogleMusic(bool isActive)
        {
            Data.ToggleMusic(isActive);
        }

        private void ToogleSfx(bool isActive)
        {
            Data.ToggleSfx(isActive);
        }

        private void ContactUs()
        {
            _MiscTP.OpenEmail();
        }
    }
}
