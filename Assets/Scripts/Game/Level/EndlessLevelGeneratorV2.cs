using System;
using System.Collections.Generic;
using System.Linq;
using RetroRush.Game.Gameplay;
using UnityEngine;
using Random = System.Random;

namespace RetroRush.Game.Level
{
    public class EndlessLevelGeneratorV2 : BasicLevelGenerator
    {
        private IPathCreator _PathCreator;
        private IPathCreator _PathCreator2;
        private float _PickableSpawnBonusRate;
        public EndlessLevelGeneratorV2(LevelDesignData levelDesignData, float pickableSpawnBonusRate) : base(levelDesignData)
        {
            _PathCreator = new PathCreator(levelDesignData,64, 60, 12);
            _PathCreator2 = new PathCreator(levelDesignData,64, 60, 24);
            _PickableSpawnBonusRate = pickableSpawnBonusRate;
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
            
            CreatePickableLayer();
            CreateObstacleLayer();
        }
        
        protected override PipeFaceData CreatePipeFaceData(int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;
            
            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (LevelDesignData.Radius * LevelDesignData.Radius) - 2 * LevelDesignData.Radius * LevelDesignData.Radius * Mathf.Cos((360f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, LevelDesignData.FaceDepth);
            data.RotateAround = (360f / LevelDesignData.NumberOfFace) * index;
            data.Depth = LevelDesignData.CurrentDepth;
            data.Index = index;
            var exist = _PathCreator.IsInPath(index, LevelDesignData.CurrentDepth / 3);
            exist = _PathCreator2.IsInPath(index, LevelDesignData.CurrentDepth / 3) || exist;
            data.Exist = exist;
            data.IsInPath = exist;
            data.PickableType = _PathCreator.HasCoin(index, LevelDesignData.CurrentDepth / 3) || _PathCreator2.HasCoin(index, LevelDesignData.CurrentDepth / 3) ? PickableType.Coin : PickableType.None;
            return data;
        }
        
        protected void CreatePickableLayer()
        {
            var ring = LevelDesignData.PipeFaces.Skip(LevelDesignData.PipeFaces.Count - LevelDesignData.NumberOfFace).ToList().FindAll(f => f.Exist);
            AddBonus(ring);
        }
        
        private void AddBonus(List<PipeFaceData> pipeFaceDatas)
        {
            var list = pipeFaceDatas.FindAll(f => f.Exist && f.PickableType == PickableType.None);
            if (list.Count > 0)
            {
                pipeFaceDatas[UnityEngine.Random.Range(0, pipeFaceDatas.Count)].PickableType = GeneratePickable();
            }
        }
        
        private PickableType GeneratePickable()
        {
            if(UnityEngine.Random.Range(0, 100) >= 90 - (90 * _PickableSpawnBonusRate / 100f))
                return (PickableType)UnityEngine.Random.Range(2, 5);

            return PickableType.None;
        }
        
        protected void CreateObstacleLayer()
        {
            var count = 0;
            while (count < 2)
            {
                var faces = LevelDesignData.PipeFaces.Skip(LevelDesignData.PipeFaces.Count - LevelDesignData.NumberOfFace).ToList().FindAll((f => !f.Exist));
                count++;
                if (UnityEngine.Random.Range(0, 100) >= 80)
                {
                    var face = faces[UnityEngine.Random.Range(0, faces.Count - 1)];
                    faces.Remove(face);
                    RemoveFaceByIndex(faces, face.Index + 1);
                    RemoveFaceByIndex(faces, face.Index - 1);
                
                    if(faces.Count <= 0)
                        return;
                
                    var face2 = faces[UnityEngine.Random.Range(0, faces.Count - 1)];

                    face.Exist = true;
                    face2.Exist = true;
                    face.HasObstacle = true;
                    face.LinkedFace = face2;
                }
            }
            
            var newFaces = LevelDesignData.PipeFaces.Skip(LevelDesignData.PipeFaces.Count - LevelDesignData.NumberOfFace).ToList().FindAll((f => !f.Exist));
            for (int i = UnityEngine.Random.Range(0, newFaces.Count); i > 0; i--)
            {
                var face = newFaces[UnityEngine.Random.Range(0, newFaces.Count - 1)];
                newFaces.Remove(face);
                face.Exist = true;
                newFaces.Add(face);
            }
        }
        
        private void RemoveFaceByIndex(List<PipeFaceData> list, int i)
        {
            var faceToRemove = list.Find(f => i % LevelDesignData.NumberOfFace == f.Index);
            if (faceToRemove == null)
                return;

            list.Remove(faceToRemove);
        }
    }
    
    public interface IPathCreator
    {
        bool IsInPath(int index, int currentDepth);
        bool HasCoin(int index, int currentDepth);
    }

    public class PathCreator : IPathCreator
    {
        private int _NumFaces;
        private double _Amplitude;  // L'amplitude de l'oscillation cosinus (6 pour 12 faces)
        private double _Period;  // La période de la fonction cosinus
        private int _CurrentLength;  // Longueur actuelle du chemin généré
        private int _MaxLength;  // Longueur maximale avant de générer un nouveau chemin
        private double _Phase;  // Phase actuelle pour assurer la continuité
        private int _Offset;
        private Random _Random;
        
        private Dictionary<int, int> _IndexCache;

        public PathCreator(LevelDesignData levelDesignData, float period, int maxLength, int seed)
        {
            _NumFaces = levelDesignData.NumberOfFace;
            _Amplitude = (double) _NumFaces / 2;
            _Period = period;
            _MaxLength = maxLength;
            _Random = new Random(seed);
            _IndexCache = new Dictionary<int, int>();
            InitializePath();
        }

        private void InitializePath()
        {
            _Phase = _Random.NextDouble() * 2 * Math.PI;
            _Offset = (_NumFaces / 2) + 1 - GetFaceIndex(0);
            _IndexCache.Add(0, GenerateNextPath(0));
            _CurrentLength = -1;
        }
        
        public int GenerateNextPath(int iterations)
        {
            if (_CurrentLength >= _MaxLength)
            {
                // Si la longueur du chemin actuel atteint la limite, démarrer un nouveau chemin
                AdjustPhaseForContinuity(iterations);
                _CurrentLength = -1;
            }

            // Calcul de l'index de la face à partir de l'oscillation cosinus
            var index = GetFaceIndex(iterations) + _Offset;
            if (index < 0)
                index += _NumFaces;

            return index % _NumFaces;
        }
        
        public bool IsInPath(int index, int currentDepth)
        {
            if(index == 0)
                _CurrentLength++;

            if (_IndexCache.TryGetValue(currentDepth, out var value))
                return index == value;
            
            _IndexCache.Add(currentDepth, GenerateNextPath(currentDepth));

            if (_IndexCache.ContainsKey(currentDepth - 1))
                _IndexCache.Remove(currentDepth - 1);
            
            return index == _IndexCache[currentDepth];
        }

        private int _MaxCoinLine = 2;
        private int _StartDepth;
        private int _StepSize = 2;
        private bool _LineInProgress;
        public bool HasCoin(int index, int currentDepth)
        {
            if (index != _IndexCache[currentDepth])
                return false;
            
            if (_LineInProgress)
            {
                if(currentDepth - _StartDepth <= _MaxCoinLine)
                    return true;

                _LineInProgress = false;
                _StartDepth = currentDepth;
                return false;
            }
            else
            {
                if (_MaxLength - _MaxCoinLine <= _CurrentLength)
                    return false;
                
                if (currentDepth - _StartDepth <= _StepSize)
                    return false;

                var canStartPath = true;
                for (int i = 1; i <= _MaxCoinLine; i++)
                {
                    if (_IndexCache.TryGetValue(currentDepth + i, out var value))
                        canStartPath = index == value;
                    else
                    {
                        _IndexCache.Add(currentDepth + i, GenerateNextPath(currentDepth + 1));
                        canStartPath = index == _IndexCache[currentDepth + i];
                    }
                }
                
                _StartDepth = currentDepth;
                _LineInProgress = canStartPath;
                return canStartPath;
            }
        }

        private bool log = false;
        // Ajuste la phase du chemin suivant pour assurer la continuité avec la fin de l'ancien chemin
        private void AdjustPhaseForContinuity(int iterations)
        {
            
            // Calcul de la phase de fin du chemin précédent
            double endFace = GetFaceIndex(iterations - 1);
            
            // Nouvelle phase aléatoire (on garde le cosinus continu)
            _Phase = _Random.NextDouble() * 2 * Math.PI;
            double newFace = GetFaceIndex(iterations);
            
            // Calcul du décalage nécessaire pour continuer le chemin sans saut
            _Offset = (int) (endFace + _Offset - newFace);
        }

        private int GetFaceIndex(int iterations)
        {
            double cosValue = Math.Cos((2 * Math.PI / _Period) * iterations + _Phase);
            return (int)(_Amplitude * (cosValue + 1)) % _NumFaces;
        }

        private void LogError(object log)
        {
            if(this.log)
                Debug.LogError(log);
        }
    }
    /*public class EndlessLevelGeneratorV2 : BasicLevelGenerator
    {
        private IPathCreator _PathCreator;
        public EndlessLevelGeneratorV2(LevelDesignData levelDesignData) : base(levelDesignData)
        {
            _PathCreator = new PathCreator(48, 4);
        }

        protected override PipeFaceData CreatePipeFaceData(int index)
        {
            PipeFaceData data = new PipeFaceData();
            data.Position = _LevelCenter;

            //Ajustement de la taille de la dalle en fonction du radius et du nombre de dalle du tube
            data.LocalScale = new Vector3(Mathf.Sqrt(2 * (LevelDesignData.Radius * LevelDesignData.Radius) - 2 * LevelDesignData.Radius * LevelDesignData.Radius * Mathf.Cos((360f / LevelDesignData.NumberOfFace) * Mathf.Deg2Rad)), 0.1f, LevelDesignData.FaceDepth);
            data.RotateAround = (360 / LevelDesignData.NumberOfFace) * index;
            data.Depth = LevelDesignData.CurrentDepth;

            if(_PathCreator.IsInPath(index, LevelDesignData.CurrentDepth, LevelDesignData.NumberOfFace))
            {
                data.PickableType = PickableType.Coin;
                data.Exist = true;
            }
            else
            {
                data.PickableType = PickableType.None;
                data.Exist = false;
            }
            return data;
        }
    }

    public interface IPathCreator
    {
        bool IsInPath(int index, int currentDepth, int maxIndex);
    }

    public class PathCreator : IPathCreator
    {
        private int _Length;
        private int _ChunkSize;
        private int _PreviousOffset;
        private int _RandomOffset;
        private int _RandomDuration;

        public PathCreator(int length, int chunkSize)
        {
            _Length = length;
            _ChunkSize = chunkSize;
            ResetPath();
        }

        public bool IsInPath(int index, int currentDepth, int maxIndex)
        {
            CheckPathProgression(index, currentDepth);
            var progress = (float) ((currentDepth + _RandomOffset + _PreviousOffset) / _ChunkSize % _Length ) / _Length;
            Debug.LogError(_RandomOffset);
            Debug.LogError("Depth = " + currentDepth + ", index = " + index + ", calc = " + (int) ((Math.Cos(2 * Math.PI * progress) + 1 ) / 2 * maxIndex));
            return (int) ((Math.Cos(2 * Math.PI * progress) + 1 ) / 2 * maxIndex) == index;
        }

        private void CheckPathProgression(int index, int currentDepth)
        {
            if (index == 0 && currentDepth % _RandomDuration == 0)
                ResetPath();
        }

        private void ResetPath()
        {
            _PreviousOffset = _RandomOffset;
            _RandomOffset = Random.Range(0, _Length);
            _RandomDuration = Random.Range(_Length / 4, _Length / 2);
        }
    }*/
}
