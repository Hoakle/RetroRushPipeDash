using UnityEngine;

namespace HoakleEngine.Core.Game
{
    public abstract class GameSave : ScriptableObject
    {
        public string SaveName;

        public abstract void Init();
    }
}
