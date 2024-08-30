using System;
using HoakleEngine.Core.Game;
using RetroRush.Game.Gameplay;
using UnityEngine.Serialization;

namespace RetroRush.GameData
{
    [Serializable]
    public class MissionData
    {
        public MissionType Type;
        public bool IsCompleted;
        
        public MissionData(MissionType type) : base()
        {
            Type = type;
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
