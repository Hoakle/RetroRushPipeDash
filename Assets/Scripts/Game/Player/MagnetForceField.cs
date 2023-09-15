using System;
using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Game.Gameplay;
using UnityEngine;

namespace RetroRush
{
    public class MagnetForceField : MonoBehaviour
    {
        [SerializeField] private SphereCollider _MagnetForceField = null;

        private List<Coin> _Coins;

        private void Start()
        {
            _Coins = new List<Coin>();
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe(EngineEventType.Magnet, ActiveMagnet);
            EventBus.Instance.Subscribe(EngineEventType.MagnetFadeOut, UnActiveMagnet);
        }

        private void OnDisable()
        {
            Active = false;
            for(int i = _Coins.Count - 1; i > 0; i--)
            {
                _Coins[i].OnDispose -= RemoveCoin;
                _Coins.RemoveAt(i);
            }
            
            EventBus.Instance.UnSubscribe(EngineEventType.Magnet, ActiveMagnet);
            EventBus.Instance.UnSubscribe(EngineEventType.MagnetFadeOut, UnActiveMagnet);
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

        private bool Active
        {
            set => _MagnetForceField.enabled = value;
        }
        
        private void ActiveMagnet()
        {
            Active = true;
        }

        private void UnActiveMagnet()
        {
            Active = false;
        }
    }
}
