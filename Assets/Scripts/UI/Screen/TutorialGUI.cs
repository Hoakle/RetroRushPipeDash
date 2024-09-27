using System;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using RetroRush.Config;
using RetroRush.GameData;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class TutorialGUI : DataGUI<TutorialStep>
    {
        [SerializeField] private LocalizedText _Title = null;
        [SerializeField] private LocalizedText _Content = null;
        
        private TutorialHandler _TutorialHandler;
        private TutorialStepData _TutorialStepData;
        private Action _ValidationAction;
        
        [Inject]
        public void Inject(TutorialConfigData tutorialConfigData,
            TutorialHandler tutorialHandler)
        {
            _TutorialHandler = tutorialHandler;
            _TutorialStepData = tutorialConfigData.GetStep(Data);
        }
        
        public override void OnReady()
        {
            _Title.SetKey(_TutorialStepData.TitleKey);
            _Content.SetKey(_TutorialStepData.ContentKey);
            base.OnReady();
        }

        public void ValidateStep()
        {
            _TutorialHandler.Validate(Data);
            Close();
        }
    }
}
