using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class LevelData
    {
        public long Score;
        public float Speed = 10f;
        public float SpeedFactor = 1f;
        public float DifficultySpeedFactor => Mathf.Lerp(1f, 2f, CurrentDepth / 500f);
        public int NumberOfFace = 12;
        public int Radius = 5;
        public int FaceDepth = 4;
        
        public int CurrentDepth;
        
        public List<PipeFaceData> PipeFaces = new();

        public int LevelDepth => PipeFaces.Count > 0 ? PipeFaces.Last().Depth - PipeFaces.First().Depth + 1 : 0;
        
        public Action OnDepthAdded;
    }
}
