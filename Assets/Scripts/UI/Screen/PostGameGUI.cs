using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Config.Ads;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.UI.Components;
using RetroRush.Config;
using RetroRush.Game.Economics;
using RetroRush.Game.Level;
using RetroRush.GameData;
using RetroRush.GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class PostGameGUI : DataGUI<LevelDesignData>
    {
        [SerializeField] private Button _Close = null;
        [SerializeField] private TextMeshProUGUI _Score = null;
        [SerializeField] private TextMeshProUGUI _Coin = null;
        [SerializeField] private TextMeshProUGUI _Title = null;
        [SerializeField] private TextMeshProUGUI _SubTitle = null;
        [SerializeField] private AdsButton _AdsButton = null;

        private PostGameState _State;
        private static readonly int State = Animator.StringToHash("State");
        private static readonly int Stars = Animator.StringToHash("Stars");
        private static readonly int AdsWatched = Animator.StringToHash("AdsWatched");

        private ProgressionHandler _ProgressionHandler;
        private CurrencyHandler _CoinCurrencyHandler;
        private AdsServicesConfigData _AdsServicesConfigData;
        private LevelConfigData _LevelConfigData;
        
        [Inject]
        public void Inject(ProgressionHandler progressionHandler,
            [Inject (Id = CurrencyType.Coin)] CurrencyHandler coinHandler,
            AdsServicesConfigData adsServicesConfigData,
            LevelConfigData levelConfigData)
        {
            _ProgressionHandler = progressionHandler;
            _CoinCurrencyHandler = coinHandler;
            _AdsServicesConfigData = adsServicesConfigData;
            _LevelConfigData = levelConfigData;
        }
        
        public override void OnReady()
        {
            SetState();
            
            _Close.onClick.AddListener(BackToMenu);

            base.OnReady();
        }

        protected override void Close()
        {
            _Close.onClick.RemoveListener(BackToMenu);
            
            base.Close();
        }

        private void Continue()
        {
            _AdsButton.OnClaimReward -= Continue;
            Data.IsFinished = true;
            EventBus.Instance.Publish(EngineEventType.Continue);
            Close();
        }
        private void BackToMenu()
        {
            EventBus.Instance.Publish(EngineEventType.BackToMenu);
            if(_ProgressionHandler.GameModeType == GameModeType.STAGE && Data.IsFinished)
                _ProgressionHandler.TryIncrementLevel();
            
            Close();
        }

        private void SetState()
        {
            _Coin.text = " + " + Data.CoinCollected;
            
            if (_ProgressionHandler.GameModeType == GameModeType.ENDLESS)
            {
                _Title.text = "Vous avez perdu !";
                _SubTitle.text = "Votre score";
                _Score.text = Data.Score.ToString();
                _State = PostGameState.CONTINUE;
                if(!Data.IsFinished)
                {
                    _GuiEngine.InitDataGUIComponent<AdsButton, AdsConfigData>(_AdsButton,
                        _AdsServicesConfigData.GetAdsConfig(AdsServicesConfigData.RVADS_CONTINUE));
                    _AdsButton.OnClaimReward += Continue;
                }
                else
                {
                    _GuiEngine.InitDataGUIComponent<AdsButton, AdsConfigData>(_AdsButton, _AdsServicesConfigData.GetAdsConfig(AdsServicesConfigData.RVADS_COIN));
                    _AdsButton.OnClaimReward += MultiplyCoin;
                }
                
            }
            else
            {
                _SubTitle.text = "Niveau " + _ProgressionHandler.CurrentLevel;
                _GuiEngine.InitDataGUIComponent<AdsButton, AdsConfigData>(_AdsButton, _AdsServicesConfigData.GetAdsConfig(AdsServicesConfigData.RVADS_COIN));
                _AdsButton.OnClaimReward += MultiplyCoin;
                if (Data.IsFinished)
                {
                    _Title.text = "Vous avez gagné !";
                    _State = PostGameState.LEVEL_WON;
                    _Animator.SetInteger(Stars, _LevelConfigData.GetStars(_ProgressionHandler.CurrentLevel, (int) Data.CoinCollected));
                }
                else
                {
                    _Title.text = "Vous avez perdu !";
                    _State = PostGameState.LEVEL_LOST;
                }
            }
            
            _Animator.SetInteger(State, (int) _State);
        }

        private void MultiplyCoin()
        {
            _AdsButton.OnClaimReward -= MultiplyCoin;
            _CoinCurrencyHandler.Add((int) Data.CoinCollected);
            
            _Coin.text = " + " + Data.CoinCollected * 2;
            _Animator.SetBool(AdsWatched, true);
        }
    }

    public enum PostGameState
    {
        CONTINUE = 0,
        LEVEL_LOST = 1,
        LEVEL_WON = 2,
    }
}
