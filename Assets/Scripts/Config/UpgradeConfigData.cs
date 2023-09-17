using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush.Config
{
    [CreateAssetMenu(fileName = "UpgradeConfigData", menuName = "Game Data/Config/UpgradeConfigData")]
    public class UpgradeConfigData : ScriptableObject
    {
        [SerializeField] private PickableType _Type;
        [SerializeField] private string _Title;
        [SerializeField] private string _Desc;
        [SerializeField] private Sprite _Icone;
        [SerializeField] private List<int> _ValuePerLevel;
        [SerializeField] private List<int> _PricePerLevel;
        [SerializeField] private float _Factor = 1.2f;
        public PickableType Type => _Type;
        public string Title => _Title;
        public string Desc => _Desc;
        public Sprite Icone => _Icone;

        public float Factor => _Factor;

        public float GetValue(int level)
        {
            return _ValuePerLevel[level - 1];
        }

        public int GetUpgradePrice(int level)
        {
            return _PricePerLevel[level - 1];
        }
    }
}
