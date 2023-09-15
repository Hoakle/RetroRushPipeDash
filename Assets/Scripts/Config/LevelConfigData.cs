using System.Collections.Generic;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "LevelConfigData", menuName = "Game Data/Config/LevelConfigData")]
    public class LevelConfigData : ScriptableObject
    {
        [SerializeField]
        private ChunkConfigData _FirstChunk;
        public ChunkConfigData FirstChunk => _FirstChunk;
        
        [SerializeField]
        private List<ChunkConfigData> _ChunkConfigs = new List<ChunkConfigData>();

        public ChunkConfigData GetNextChunk(ChunkConfigData currentChunk)
        {
            return _ChunkConfigs[Random.Range(1, _ChunkConfigs.Count)];
        }
    }
}
