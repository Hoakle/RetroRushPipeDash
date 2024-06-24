using System;
using HoakleEngine.Core.Game;
using RetroRush.Game.Gameplay;

namespace RetroRush.GameData
{
    [Serializable]
    public class MissionData
    {
        public string Title;
        public MissionType Type;
        public string Description;
        public bool IsCompleted;
        
        public MissionData(MissionType type, string description) : base()
        {
            Type = type;
            Title = type.ToString().Replace("_", " ");
            Description = description;
        }
    }

    public enum MissionType
    {
            FIRST_RUN,
            BOOST_COLLECTOR,
            BUNNY_UP,
            JY_FUS,
            DANS_LES_ETOILES,
    }
}
