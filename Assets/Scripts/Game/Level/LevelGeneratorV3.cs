using System.Collections.Generic;
using System.Linq;
using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class LevelGeneratorV3 : LevelGeneratorV2
    {
        private int Difficulty = 0;
        public LevelGeneratorV3(LevelData levelData, LevelConfigData levelConfig) : base(levelData, levelConfig)
        {
            
        }
        
        public override void AddDepth()
        {
            CreateOneRing();
            _LevelData.CurrentDepth++;
        }
        
        private void CreateOneRing()
        {
            //Création du tube
            for (int i = 0; i < _LevelData.NumberOfFace; i++)
            {
                _LevelData.PipeFaces.Add(CreatePipeFaceData(CheckExist(i, _LevelData.CurrentDepth), i));
            }
            
            CreateObstacleLayer();
            CreatePickableLayer();
        }
        

        private bool CheckExist(float x, float y)
        {
            if (_LevelData.CurrentDepth <= 5)
                return true;
            
            return Random.Range(0f, 1f) > 0.3f;
            float j = y % 10;
            float i = (0.2f * j) % 1 / (x + 1);

            var value = Mathf.PerlinNoise(i, y % 10f / 10f);
            Debug.LogError(value);
            return  value is > 0.3f and < 0.6f;
        }
        protected PipeFaceData CreatePipeFaceData(bool exist, int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (_LevelData.Radius * _LevelData.Radius) - 2 * _LevelData.Radius * _LevelData.Radius * Mathf.Cos((360f / _LevelData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, _LevelData.FaceDepth);
            data.RotateAround = (360 / _LevelData.NumberOfFace) * index; //_LevelRepresentation.transform.rotation.eulerAngles.z
            data.Depth = _LevelData.CurrentDepth;
            data.Exist = exist;
            data.Index = index;
            return data;
        }
    }

    
}
