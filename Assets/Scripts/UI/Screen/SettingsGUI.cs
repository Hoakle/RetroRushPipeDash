using System.Collections.Generic;
using System.Linq;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using HoakleEngine.Core.Services.MiscService;
using RetroRush.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class SettingsGUI : DataGUI<SettingsGameSave>
    {
        [SerializeField] private Button _CloseButton = null;
        [SerializeField] private Button _LanguagesButton = null;
        [SerializeField] private TextMeshProUGUI _Language = null;
        [SerializeField] private ToogleButton _MusicToogle = null;
        [SerializeField] private ToogleButton _SfxToogle = null;
        [SerializeField] private Button _ContactUs = null;

        private MiscThirdPartyService _MiscTP;
        private ILocalizationProvider _LocalizationProvider;
        
        [Inject]
        public void Inject(MiscThirdPartyService miscTp, ILocalizationProvider localizationProvider)
        {
            _MiscTP = miscTp;
            _LocalizationProvider = localizationProvider;
        }
        
        public override void OnReady()
        {
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_MusicToogle, Data.HasMusic.Value);
            _GuiEngine.InitDataGUIComponent<ToogleButton, bool>(_SfxToogle, Data.HasSfx.Value);

            _Language.text = _LocalizationProvider.AvailableLanguage[Data.Language];
            _LanguagesButton.onClick.AddListener(OnLanguageSelected);
            
            _CloseButton.onClick.AddListener(Close);
            _MusicToogle.OnToogleChange += ToogleMusic;
            _SfxToogle.OnToogleChange += ToogleSfx;
            
            _ContactUs.onClick.AddListener(ContactUs);

            base.OnReady();
        }

        private void OnLanguageSelected()
        {
            var list = _LocalizationProvider.AvailableLanguage.Keys.ToList();
            var index = list.IndexOf(Data.Language);
            if (index < list.Count - 1)
                index++;
            else
            {
                index = 0;
            }
            
            Data.Language = _LocalizationProvider.AvailableLanguage.ElementAt(index).Key;
            _LocalizationProvider.SetLanguage(Data.Language);
            _Language.text = _LocalizationProvider.AvailableLanguage[Data.Language];
            Data.Save();
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
