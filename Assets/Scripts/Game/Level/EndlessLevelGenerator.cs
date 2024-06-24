using System.Collections.Generic;
using System.Linq;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class EndlessLevelGenerator : BasicLevelGenerator
    {
        private PickablePath _PickablePath;
        private float _PickableSpawnBonusRate;
        
        public EndlessLevelGenerator(LevelDesignData levelDesignData, float pickableSpawnBonusRate) : base(levelDesignData)
        {
            _PickableSpawnBonusRate = pickableSpawnBonusRate;
            _PickablePath = new PickablePath();
        }
        
        public override void AddDepth()
        {
            CreateOneRing();
            LevelDesignData.CurrentDepth++;
        }
        
        private void CreateOneRing()
        {
            //Création du tube
            for (int i = 0; i < LevelDesignData.NumberOfFace; i++)
            {
                LevelDesignData.PipeFaces.Add(CreatePipeFaceData(i));
            }
            
            CreateObstacleLayer();
            CreatePickableLayer();
        }
        

        private bool CheckExist(float x, float y)
        {
            if (LevelDesignData.CurrentDepth <= 5)
                return true;
            
            return Random.Range(0f, 1f) > 0.3f;
        }
        
        protected override PipeFaceData CreatePipeFaceData(int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (LevelDesignData.Radius * LevelDesignData.Radius) - 2 * LevelDesignData.Radius * LevelDesignData.Radius * Mathf.Cos((360f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, LevelDesignData.FaceDepth);
            data.RotateAround = (360f / LevelDesignData.NumberOfFace) * index;
            data.Depth = LevelDesignData.CurrentDepth;
            data.Exist = CheckExist(index, LevelDesignData.CurrentDepth);
            data.Index = index;
            return data;
        }
        
        protected void CreatePickableLayer()
        {
            
            var ring = LevelDesignData.PipeFaces.Skip(LevelDesignData.PipeFaces.Count - LevelDesignData.NumberOfFace).ToList().FindAll(f => f.Exist);
            if (ring.Count == 0)
                return;
            
            AddCoinInPath(ring);
            AddBonus(ring);
        }
        
        protected void CreateObstacleLayer()
        {
            if (Random.Range(0, 100) >= 80)
            {
                var faces = LevelDesignData.PipeFaces.Skip(LevelDesignData.PipeFaces.Count - LevelDesignData.NumberOfFace).ToList().FindAll((f => f.Exist));
                if(faces.Count <= 0)
                    return;
                
                var face = faces[Random.Range(0, faces.Count - 1)];
                faces.Remove(face);
                RemoveFaceByIndex(faces, face.Index + 1);
                RemoveFaceByIndex(faces, face.Index - 1);
                
                if(faces.Count <= 0)
                    return;
                
                var face2 = faces[Random.Range(0, faces.Count - 1)];
                
                face.HasObstacle = true;
                face.LinkedFace = face2;
            }
        }
        
        private void RemoveFaceByIndex(List<PipeFaceData> list, int i)
        {
            var faceToRemove = list.Find(f => i % LevelDesignData.NumberOfFace == f.Index);
            if (faceToRemove == null)
                return;

            list.Remove(faceToRemove);
        }
        
        private void AddCoinInPath(List<PipeFaceData> pipeFaceDatas)
        {
            if (!_PickablePath.InProgress)
            {
                var face = pipeFaceDatas[Random.Range(0, pipeFaceDatas.Count)];
                face.PickableType = PickableType.Coin;
                _PickablePath.LastIndex = face.Index;
            }
            else
            {
                pipeFaceDatas.Sort((f1, f2) => Mathf.Abs(_PickablePath.LastIndex % LevelDesignData.NumberOfFace - f1.Index % LevelDesignData.NumberOfFace).CompareTo(Mathf.Abs(_PickablePath.LastIndex % LevelDesignData.NumberOfFace - f2.Index % LevelDesignData.NumberOfFace)));
                var face = pipeFaceDatas[0];
                face.PickableType = PickableType.Coin;
                _PickablePath.LastIndex = face.Index;
            }
        }

        private void AddBonus(List<PipeFaceData> pipeFaceDatas)
        {
            var list = pipeFaceDatas.FindAll(f => f.PickableType != PickableType.None);
            if (list.Count > 0)
            {
                pipeFaceDatas[Random.Range(0, pipeFaceDatas.Count)].PickableType = GeneratePickable();
            }
        }
        
        private PickableType GeneratePickable()
        {
            if(Random.Range(0, 100) >= 90 - (90 * _PickableSpawnBonusRate / 100f))
                return (PickableType)Random.Range(2, 5);

            return PickableType.None;
        }
    }
    
    public class PickablePath
    {
        public int Offset = 5;
        public int Lenght = 10;
        
        public bool InProgress;
        public int CurrentProgression;
        public int LastIndex;

        public int OffsetProgression;
        
        public int CurrentDepth;

        public void Reset()
        {
            InProgress = false;
            LastIndex = 0;
        }
    }
}
