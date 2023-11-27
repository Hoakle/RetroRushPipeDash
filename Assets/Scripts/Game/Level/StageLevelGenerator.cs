using RetroRush.Config;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class StageLevelGenerator : BasicLevelGenerator
    {
        
        protected StageConfigData CurrentStage;

        public StageLevelGenerator(LevelDesignData levelDesignData, StageConfigData stageConfig) : base(levelDesignData)
        {
            CurrentStage = stageConfig;
        }

        public override bool NeedMoreDepth()
        {
            return base.NeedMoreDepth() && CurrentStage.StageDepth >= LevelDesignData.CurrentDepth;
        }
        protected override PipeFaceData CreatePipeFaceData(int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (LevelDesignData.Radius * LevelDesignData.Radius) - 2 * LevelDesignData.Radius * LevelDesignData.Radius * Mathf.Cos((360f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, LevelDesignData.FaceDepth);
            data.RotateAround = (360 / LevelDesignData.NumberOfFace) * index;
            data.Depth = LevelDesignData.CurrentDepth;
            
            if (CurrentStage.StageDepth == LevelDesignData.CurrentDepth)
            {
                //Last ring of the level with special design
                data.Exist = true;
                data.PickableType = PickableType.Finish;
            }
            else
            {
                var faceType = CurrentStage.PipeFaceConfigs[index * LevelDesignData.CurrentDepth];
                
                data.PickableType = GeneratePickable(faceType);
                data.Exist = faceType != PipeFaceType.EMPTY;
            }
            
            return data;
        }
        
        private PickableType GeneratePickable(PipeFaceType faceType)
        {
            switch (faceType)
            {
                case PipeFaceType.COIN:
                    return PickableType.Coin;
                case PipeFaceType.SPEED:
                    return PickableType.SpeedBonus;
                case PipeFaceType.MAGNET:
                    return PickableType.Magnet;
                case PipeFaceType.SHIELD:
                    return PickableType.Shield;
                default:
                    return PickableType.None;
            }
        }
    }
}
