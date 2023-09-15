using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine
{
    [CreateAssetMenu(fileName = "UISkinData", menuName = "Game Data/UISkin/UISkinData")]
    public class UISkinData : ScriptableObject
    {
        public Color HeaderColor;
        public Color BodyColor;
    }
}
