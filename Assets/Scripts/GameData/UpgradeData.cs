using System.Collections;
using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush
{
    public class UpgradeData
    {
        public UpgradeData(PickableType type)
        {
            Type = type;
            Level = 1;
        }
        public PickableType Type;
        public int Level;
    }
}
