using System;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush
{
    public class PipeFaceData
    {
        public PickableType PickableType;
        public Vector3 Position;
        public Vector3 LocalScale;
        public float RotateAround;
        public int Depth;
        public int Index;
        public bool Exist;
        public bool IsInPath;

        public bool HasObstacle;
        public PipeFaceData LinkedFace;

        public PipeFaceData BeforeFace;
        public PipeFaceData BehindFace;
        public PipeFaceData RightFace;
        public PipeFaceData LeftFace;

        public Action OnRemoveFace;
        public Action OnDataChange;
        
    }
}
