using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Config
{
    [CreateAssetMenu(fileName = "ConfigContainer", menuName = "Data/HoakleEngine/ConfigContainer")]
    public class ConfigContainer : ScriptableObject
    {
        [SerializeField] private List<ScriptableObject> _Configs = new List<ScriptableObject>();

        private Dictionary<Type, ScriptableObject> _ConfigCache = new Dictionary<Type, ScriptableObject>();
        
        public T GetConfig<T>() where T : ScriptableObject
        {
            Type type = typeof(T);
            if (!_ConfigCache.ContainsKey(type))
            {
                _ConfigCache[type] = _Configs.Find(so => so is T);
            }
            
            return (T) _ConfigCache[type];
        }
    }
}
