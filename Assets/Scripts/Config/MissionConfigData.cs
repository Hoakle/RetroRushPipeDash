using System;
using RetroRush.GameData;

namespace RetroRush.Config
{
    [Serializable]
    public class MissionConfigData
    {
        public MissionType Type;
        public string Key;
        public string TranslationKey;
    }
}
