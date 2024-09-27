using System.Collections.Generic;
using HoakleEngine.Core.Game;
using RetroRush.Config;

namespace RetroRush.GameData
{
    public class TutorialHandler : GameSaveHandler<TutorialData>
    {
        public TutorialHandler() : base("TutorialHandler") { }

        protected override void CreateData()
        {
            _Data.ValidatedSteps = new List<TutorialStep>();
        }

        public bool IsDone(TutorialStep step)
        {
            return _Data.ValidatedSteps.Contains(step);
        }

        public void Validate(TutorialStep step)
        {
            if(!IsDone(step))
            {
                _Data.ValidatedSteps.Add(step);
                Save();
            }
        }
    }

    public struct TutorialData
    {
        public List<TutorialStep> ValidatedSteps;
    }
}
