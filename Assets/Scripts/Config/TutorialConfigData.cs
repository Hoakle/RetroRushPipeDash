using System;
using System.Collections.Generic;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "TutorialConfigData", menuName = "Game Data/Config/TutorialConfigData")]
    public class TutorialConfigData : ScriptableObject
    {
        public List<TutorialStepData> TutorialSteps;

        public TutorialStepData GetStep(TutorialStep step)
        {
            return TutorialSteps.Find(t => t.Step == step);
        }
    }
    
    [Serializable]
    public class TutorialStepData
    {
        public TutorialStep Step;
        public string TitleKey;
        public string ContentKey;
    }

    [Serializable]
    public enum TutorialStep
    {
        Welcome,
        
    }
}