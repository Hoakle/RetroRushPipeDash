using System;
using HoakleEngine.Core.Game;
using RetroRush.Game.Gameplay;

namespace RetroRush.GameData
{
    [Serializable]
    public class MissionData : GameSaveData
    {
        public MissionData(MissionType type, string description)
        {
            Type = type;
            Title = type.ToString().Replace("_", " ");
            Description = description;
        }

        public string Title;
        public MissionType Type;
        public string Description;
        public bool IsCompleted;
    }
    
    [Serializable]
    public enum MissionType
    {
            FIRST_RUN,
            BOOST_COLLECTOR,
            BUNNY_UP,
    }
}
