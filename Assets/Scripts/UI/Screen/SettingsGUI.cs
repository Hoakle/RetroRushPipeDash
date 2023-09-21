using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Services;
using RetroRush.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class SettingsGUI : DataGUI<SettingsGameSave>
    {
        [SerializeField] private Button _CloseButton = null;
        [SerializeField] private ToogleButton _MusicToogle = null;
        [SerializeField] private ToogleButton _SfxToogle = null;
        [SerializeField] private Button _ContactUs = null;
        public override void OnReady()
        {
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_MusicToogle, Data.HasMusic);
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_SfxToogle, Data.HasSfx);
            
            _CloseButton.onClick.AddListener(Dispose);
            _MusicToogle.OnToogleChange += ToogleMusic;
            _SfxToogle.OnToogleChange += ToogleSfx;
            
            _ContactUs.onClick.AddListener(ContactUs);

            base.OnReady();
        }

        protected override void Dispose()
        {
            _CloseButton.onClick.RemoveListener(Dispose);
            _MusicToogle.OnToogleChange -= ToogleMusic;
            _SfxToogle.OnToogleChange -= ToogleSfx;
            
            _ContactUs.onClick.RemoveListener(ContactUs);
            base.Dispose();
        }

        private void ToogleMusic(bool isActive)
        {
            Debug.LogError(isActive);
            Data.HasMusic = isActive;
            _GuiEngine.GameSave.Save();
        }

        private void ToogleSfx(bool isActive)
        {
            Data.HasSfx = isActive;
            _GuiEngine.GameSave.Save();
        }

        private void ContactUs()
        {
            _GuiEngine.ServicesContainer.GetService<MiscThirdPartyService>().OpenEmail();
        }
    }
}
