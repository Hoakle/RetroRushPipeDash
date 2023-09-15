using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroRush.Game.Level
{
    public class LevelData
    {
        public long Score;
        public float Speed = 7.5f;
        public float SpeedFactor = 1f;
        public int NumberOfFace = 12;
        public int Radius = 5;
        public int FaceDepth = 4;
        
        public int CurrentDepth;
        
        public List<PipeFaceData> PipeFaces = new();

        public int LevelDepth => PipeFaces.Count > 0 ? PipeFaces.Last().Depth - PipeFaces.First().Depth + 1 : 0;
        
        public Action OnDepthAdded;
    }
}
