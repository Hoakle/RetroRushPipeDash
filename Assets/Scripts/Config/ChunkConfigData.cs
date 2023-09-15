using System;
using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "ChunkConfigData", menuName = "Game Data/Config/ChunkConfigData")]
    public class ChunkConfigData : ScriptableObject
    {
        public int Id;
        public int ChunkDepth;
        public int Weight;
        
        public List<int> IncompatibleNextChunk = new List<int>();
        public List<PipeFaceConfigData> PipeFaceConfigs = new List<PipeFaceConfigData>();
    }

    [Serializable]
    public class PipeFaceConfigData
    {
        public int Depth;
        public int FaceIndex;
        public PickableType PickableType;
        public bool Exist;
    }
}
