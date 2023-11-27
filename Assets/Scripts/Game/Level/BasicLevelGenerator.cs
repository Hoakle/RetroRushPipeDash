using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class BasicLevelGenerator : ILevelGenerator
    {
        protected LevelDesignData LevelDesignData;

        private int _MaxDepth = 15;
        protected Vector3 _LevelCenter;
        public BasicLevelGenerator(LevelDesignData levelDesignData)
        {
            LevelDesignData = levelDesignData;
            _LevelCenter = GetCenter();
        }

        private Vector3 GetCenter()
        {
            //Utilisation de l'équation d'un cercle pour trouver deux points sur un cercle en fonction du rayon, de l'angle (défini par le nombre de face) et de la pronfondeur.
            Vector3 firstPoint = new Vector3(LevelDesignData.Radius * Mathf.Cos((-180f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad), LevelDesignData.Radius * Mathf.Sin((-180f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad), 0);
            Vector3 secondPoint = new Vector3(LevelDesignData.Radius * Mathf.Cos((180f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad), LevelDesignData.Radius * Mathf.Sin((180f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad), 0);
            
            //Récupération du centre des deux points obtenus
            return new Vector3((firstPoint.x + secondPoint.x) / 2f, (firstPoint.y + secondPoint.y) / 2f, 0);
        }
        public virtual bool NeedMoreDepth()
        {
            return LevelDesignData.LevelDepth < _MaxDepth;
        }

        public virtual void AddDepth()
        {
            for (int i = 0; i < LevelDesignData.NumberOfFace; i++)
            {
                LevelDesignData.PipeFaces.Add(CreatePipeFaceData(i));
            }
            
            LevelDesignData.CurrentDepth++;
        }
        
        public void UpdateLevel(float zPosition)
        {
            while (LevelDesignData.PipeFaces[0].Depth < (-zPosition / LevelDesignData.FaceDepth) - 5)
            {
                LevelDesignData.PipeFaces[0].OnRemoveFace?.Invoke();
                LevelDesignData.PipeFaces.RemoveAt(0);
            }
            
            if(NeedMoreDepth())
            {
                AddDepth();
                LevelDesignData.OnDepthAdded?.Invoke();
            }
        }

        protected virtual PipeFaceData CreatePipeFaceData(int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (LevelDesignData.Radius * LevelDesignData.Radius) - 2 * LevelDesignData.Radius * LevelDesignData.Radius * Mathf.Cos((360f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, LevelDesignData.FaceDepth);
            data.PickableType = PickableType.None;
            data.RotateAround = (360 / LevelDesignData.NumberOfFace) * index;
            data.Depth = LevelDesignData.CurrentDepth;
            data.Exist = true;
            return data;
        }
    }
}
