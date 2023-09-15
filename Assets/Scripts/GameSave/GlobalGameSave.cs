using System;
using RetroRush.Game.Economics;
using UnityEngine;

namespace RetroRush.GameSave
{
    [CreateAssetMenu(fileName = "GlobalGameSave", menuName = "Game Data/GameSaves/GlobalGameSave")]
    [Serializable]
    public class GlobalGameSave : HoakleEngine.Core.Game.GameSave
    {
        public Wallet Wallet;
        public long BestScore;
        public override void Init()
        {
            Wallet.Init();
        }
    }
}
