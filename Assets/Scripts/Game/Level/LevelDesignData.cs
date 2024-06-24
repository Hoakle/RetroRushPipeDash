using System;
using System.Collections.Generic;
using System.Linq;
using RetroRush.Game.Economics;
using RetroRush.Game.Gameplay;
using UniRx;
using UnityEngine;
using Zenject;

namespace RetroRush.Game.Level
{
    public class LevelDesignData
    {
        public int NumberOfFace => _NumberOfFace;
        public int Radius => _Radius;
        public int FaceDepth => _FaceDepth;
        public float Speed => _Speed;
        
        private const int _NumberOfFace = 11;
        private const int _Radius = 5;
        private const int _FaceDepth = 4;
        private const float _Speed = 10f;
        
        private IBonusMediator _BonusMediator;
        private CurrencyHandler _CoinCurrencyHandler;
        private float _CoinFactor = 1;
        
        [Inject]
        public void Inject(IBonusMediator bonusMediator,
            [Inject (Id = CurrencyType.Coin)] CurrencyHandler coinHandler)
        {
            _CoinCurrencyHandler = coinHandler;
            
            bonusMediator.OnBonusStarted
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.SpeedBoost or PickableType.StartBoost)
                .Subscribe(bonus => SpeedFactor += bonus.GetFactor());
            
            bonusMediator.OnBonusFadeOut
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.SpeedBoost or PickableType.StartBoost)
                .Subscribe(bonus => SpeedFactor -= bonus.GetFactor());
            
            bonusMediator.OnBonusStarted
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.CoinFactor)
                .Subscribe(bonus => _CoinFactor = bonus.GetFactor());
            
            bonusMediator.OnBonusFadeOut
                .SkipLatestValueOnSubscribe()
                .Where(bonus => bonus.Type is PickableType.CoinFactor)
                .Subscribe(bonus => _CoinFactor = 1);
        }
        
        public IReactiveProperty<long> Score = new ReactiveProperty<long>();
        public float CoinCollected;
        public bool IsFinished = false;
        public float SpeedFactor = 1f;
        public float Distance;
        public float GetFinalSpeed()
        {
            return Speed * SpeedFactor * Time.deltaTime;
        }

        public void Reset()
        {
            PipeFaces.Clear();
            CurrentDepth = 0;
            Distance = 0;
            IsFinished = false;
            Score.Value = 0;
            CoinCollected = 0;
            SpeedFactor = 1;
        }

        public void CollectCoin()
        {
            CoinCollected += _CoinFactor;
            _CoinCurrencyHandler.Add((long) _CoinFactor);
        }

        public int CurrentDepth;
        
        public List<PipeFaceData> PipeFaces = new();

        public int LevelDepth => PipeFaces.Count > 0 ? PipeFaces.Last().Depth - PipeFaces.First().Depth + 1 : 0;

        public Action OnDepthAdded;
    }
}
