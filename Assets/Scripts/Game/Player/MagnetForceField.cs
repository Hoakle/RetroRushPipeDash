using System.Collections.Generic;
using RetroRush.Game.Gameplay;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Player
{
    public class MagnetForceField : MonoBehaviour
    {
        [SerializeField] private SphereCollider _MagnetForceField = null;

        private List<Coin> _Coins = new List<Coin>();
        private bool Active
        {
            set => _MagnetForceField.enabled = value;
        }

        [Inject]
        public void Inject(IBonusMediator bonusMediator)
        {
            bonusMediator.OnBonusStarted
                .SkipLatestValueOnSubscribe()
                .Where(b => b.Type == PickableType.Magnet)
                .Subscribe(_ => ActiveMagnet());
            
            bonusMediator.OnBonusFadeOut
                .SkipLatestValueOnSubscribe()
                .Where(b => b.Type == PickableType.Magnet)
                .Subscribe(_ => UnActiveMagnet());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Coin"))
            {
                var coin = other.GetComponent<Coin>();
                coin.OnDispose += RemoveCoin;
                _Coins.Add(coin);
            }
        }

        private void RemoveCoin(Coin coin)
        {
            _Coins.Remove(coin);
            coin.OnDispose -= RemoveCoin;
        }

        private void Update()
        {
            foreach (var coin in _Coins)
            {
                coin.transform.position = Vector3.MoveTowards(coin.transform.position, transform.position, coin.MagnetForce * Time.deltaTime);
                coin.MagnetForce += 0.5f;
            }
        }

        private void ActiveMagnet()
        {
            Active = true;
        }

        private void UnActiveMagnet()
        {
            Active = false;
            for(int i = _Coins.Count - 1; i > 0; i--)
            {
                _Coins[i].OnDispose -= RemoveCoin;
                _Coins.RemoveAt(i);
            }
        }
    }
}
