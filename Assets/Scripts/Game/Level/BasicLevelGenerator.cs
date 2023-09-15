
using System.Collections.Generic;
using System.Linq;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class BasicLevelGenerator : LevelGenerator
    {
        protected LevelData _LevelData;
        private LevelConfigData _LevelConfigData;

        protected ChunkConfigData CurrentChunk;
        protected int ChunkDepthProgression;

        private int _MaxDepth = 15;
        protected Vector3 _LevelCenter;
        public BasicLevelGenerator(LevelData levelData, LevelConfigData levelConfig)
        {
            _LevelData = levelData;
            _LevelCenter = GetCenter();
            _LevelConfigData = levelConfig;
        }

        private Vector3 GetCenter()
        {
            //Utilisation de l'équation d'un cercle pour trouver deux points sur un cercle en fonction du rayon, de l'angle (défini par le nombre de face) et de la pronfondeur.
            Vector3 firstPoint = new Vector3(_LevelData.Radius * Mathf.Cos((-180f / _LevelData.NumberOfFace) * Mathf.Deg2Rad), _LevelData.Radius * Mathf.Sin((-180f / _LevelData.NumberOfFace) * Mathf.Deg2Rad), 0);
            Vector3 secondPoint = new Vector3(_LevelData.Radius * Mathf.Cos((180f / _LevelData.NumberOfFace) * Mathf.Deg2Rad), _LevelData.Radius * Mathf.Sin((180f / _LevelData.NumberOfFace) * Mathf.Deg2Rad), 0);
            
            //Récupération du centre des deux points obtenus
            return new Vector3((firstPoint.x + secondPoint.x) / 2f, (firstPoint.y + secondPoint.y) / 2f, 0);
        }
        public override bool NeedMoreDepth()
        {
            return _LevelData.LevelDepth < _MaxDepth;
        }

        public override void AddDepth()
        {
            CurrentChunk = GetChunkConfig();
            
            foreach (var faceConfig in CurrentChunk.PipeFaceConfigs.FindAll(f => f.Depth == ChunkDepthProgression))
            {
                _LevelData.PipeFaces.Add(CreatePipeFaceData(faceConfig));
            }

            CreateObstacleLayer();
            ChunkDepthProgression++;
            _LevelData.CurrentDepth++;
        }

        private ChunkConfigData GetChunkConfig()
        {
            if (CurrentChunk == null)
                return _LevelConfigData.FirstChunk;

            if (ChunkDepthProgression >= CurrentChunk.ChunkDepth)
            {
                ChunkDepthProgression = 0;
                var chunk = _LevelConfigData.GetNextChunk(CurrentChunk);
                return chunk;
            }

            return CurrentChunk;
        }
        public override void UpdateLevel(float zPosition)
        {
            while (_LevelData.PipeFaces[0].Depth < (-zPosition / _LevelData.FaceDepth) - 5)
            {
                _LevelData.PipeFaces[0].OnRemoveFace?.Invoke();
                _LevelData.PipeFaces.RemoveAt(0);
            }
            
            if(NeedMoreDepth())
            {
                AddDepth();
                _LevelData.OnDepthAdded?.Invoke();
            }
        }

        protected virtual PipeFaceData CreatePipeFaceData(PipeFaceConfigData faceConfig)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (_LevelData.Radius * _LevelData.Radius) - 2 * _LevelData.Radius * _LevelData.Radius * Mathf.Cos((360f / _LevelData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, _LevelData.FaceDepth);
            data.PickableType = GeneratePickable(faceConfig);
            data.RotateAround = (360 / _LevelData.NumberOfFace) * faceConfig.FaceIndex; //_LevelRepresentation.transform.rotation.eulerAngles.z
            data.Depth = _LevelData.CurrentDepth;
            data.Exist = faceConfig.Exist;
            return data;
        }

        private PickableType GeneratePickable(PipeFaceConfigData faceConfig)
        {
            if(!faceConfig.Exist)
                return PickableType.None;
                
            if(faceConfig.PickableType != 0)
                return faceConfig.PickableType;
            else
            {
                if(Random.Range(0, 100) >= 90)
                    return (PickableType)Random.Range(2, 5);

                return PickableType.None;
            }
        }

        protected void CreateObstacleLayer()
        {
            if (Random.Range(0, 100) >= 80)
            {
                var faces = _LevelData.PipeFaces.Skip(_LevelData.PipeFaces.Count - _LevelData.NumberOfFace).ToList().FindAll((f => f.Exist));
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
            var faceToRemove = list.Find(f => i % _LevelData.NumberOfFace == f.Index);
            if (faceToRemove == null)
                return;

            list.Remove(faceToRemove);
        }
    }
}
