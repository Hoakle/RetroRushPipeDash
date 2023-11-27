using System;
using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "StageConfigData", menuName = "Game Data/Config/StageConfigData")]
    public class StageConfigData : ScriptableObject
    {
        public int Id;
        public int StageDepth;
        
        public List<PipeFaceType> PipeFaceConfigs = new List<PipeFaceType>();
    }

    [Serializable]
    public enum PipeFaceType
    {
        BASE = 0,
        COIN = 1,
        LAZER = 2,
        SHIELD = 3,
        SPEED = 4,
        MAGNET = 5,
        EMPTY = 6
    }
}
