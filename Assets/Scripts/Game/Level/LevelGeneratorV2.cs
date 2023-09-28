using System.Collections.Generic;
using System.Linq;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class LevelGeneratorV2 : BasicLevelGenerator
    {
        private PickablePath _PickablePath;
        
        public LevelGeneratorV2(LevelData levelData, LevelConfigData levelConfig) : base(levelData, levelConfig)
        {
            _PickablePath = new PickablePath();
        }
        
        public override void AddDepth()
        {
            if (CurrentChunk == null || ChunkDepthProgression >= CurrentChunk.ChunkDepth)
            {
                _PickablePath.Reset();
            }
            
            base.AddDepth();
            CreatePickableLayer();
        }
        
        protected override PipeFaceData CreatePipeFaceData(PipeFaceConfigData faceConfig)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (_LevelData.Radius * _LevelData.Radius) - 2 * _LevelData.Radius * _LevelData.Radius * Mathf.Cos((360f / _LevelData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, _LevelData.FaceDepth);
            data.RotateAround = (360 / _LevelData.NumberOfFace) * faceConfig.FaceIndex; //_LevelRepresentation.transform.rotation.eulerAngles.z
            data.Depth = _LevelData.CurrentDepth;
            data.Exist = faceConfig.Exist;
            data.Index = faceConfig.FaceIndex;
            return data;
        }

        protected void CreatePickableLayer()
        {
            
            var ring = _LevelData.PipeFaces.Skip(_LevelData.PipeFaces.Count - _LevelData.NumberOfFace).ToList().FindAll(f => f.Exist);
            if (ring.Count == 0)
                return;
            
            AddCoinInPath(ring);
            AddBonus(ring);
            /*if (_PickablePath.InProgress)
            {
                AddCoinInPath();

                if (_PickablePath.Lenght < _PickablePath.CurrentProgression)
                {
                    _PickablePath.InProgress = false;
                    _PickablePath.CurrentProgression = 0;
                }
            }
            else
            {
                if (_PickablePath.Offset <= _PickablePath.OffsetProgression)
                {
                    _PickablePath.OffsetProgression = 0;
                    _PickablePath.InProgress = true;
                    AddCoinInPath();
                }
                else
                {
                    _PickablePath.OffsetProgression++;
                }
            }*/


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
                pipeFaceDatas.Sort((f1, f2) => Mathf.Abs(_PickablePath.LastIndex % _LevelData.NumberOfFace - f1.Index % _LevelData.NumberOfFace).CompareTo(Mathf.Abs(_PickablePath.LastIndex % _LevelData.NumberOfFace - f2.Index % _LevelData.NumberOfFace)));
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
            if(Random.Range(0, 100) >= 90 - (90 * PickableSpawnBonusRate / 100f))
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
