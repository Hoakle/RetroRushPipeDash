using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Game
{
    [CreateAssetMenu(fileName = "GameSaveContainer", menuName = "Data/HoakleEngine/GameSaveContainer")]
    public class GameSaveContainer : ScriptableObject
    {
        [SerializeField] private List<GameSave> _Saves = new List<GameSave>();

        private Dictionary<Type, GameSave> _SaveCache = new Dictionary<Type, GameSave>();
        
        public T GetSave<T>() where T : GameSave
        {
            Type type = typeof(T);
            if (!_SaveCache.ContainsKey(type))
            {
                _SaveCache[type] = _Saves.Find(so => so is T);
            }
            
            return (T) _SaveCache[type];
        }

        public void LoadSaves()
        {
            foreach (var save in _Saves)
            {
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(save.SaveName), save);
            }
        }

        public void Save()
        {
            foreach (var save in _Saves)
            {
                PlayerPrefs.SetString(save.SaveName, JsonUtility.ToJson(save));
            }
        }

        public void Init()
        {
            foreach (var save in _Saves)
            {
                save.Init();
            }
        }
    }
}
