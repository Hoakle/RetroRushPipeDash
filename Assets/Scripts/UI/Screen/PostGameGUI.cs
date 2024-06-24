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

namespace RetroRush.UI.Screen
{
    public class PostGameGUI : DataGUI<LevelDesignData>
    {
        [SerializeField] private Button _Close = null;
        [SerializeField] private TextMeshProUGUI _Score = null;
        [SerializeField] private TextMeshProUGUI _Title = null;
        [SerializeField] private TextMeshProUGUI _SubTitle = null;
        
        [SerializeField] private AdsButton _AdsButton = null;
        // Start is called before the first frame update
        private PostGameState _State;
        private static readonly int State = Animator.StringToHash("State");
        private static readonly int Stars = Animator.StringToHash("Stars");

        public override void OnReady()
        {
            SetState();
            
            _Close.onClick.AddListener(BackToMenu);

            base.OnReady();
        }

        protected override void Close()
        {
            _Close.onClick.RemoveListener(BackToMenu);

            if (Data.GameMode.Type == GameModeType.ENDLESS)
            {
                _AdsButton.OnClaimReward -= Continue;
            }
            else
            {
                _AdsButton.OnClaimReward += MultiplyCoin;
            }
            
            _GuiEngine.GameSave.Save();
            base.Close();
        }

        private void Continue()
        {
            EventBus.Instance.Publish(EngineEventType.Continue);
            Close();
        }
        private void BackToMenu()
        {
            EventBus.Instance.Publish(EngineEventType.BackToMenu);
            Close();
        }

        private void SetState()
        {
            if (Data.GameMode.Type == GameModeType.ENDLESS)
            {
                _Title.text = "Vous avez perdu !";
                _SubTitle.text = "Votre score";
                _Score.text = Data.Score.ToString();
                _State = PostGameState.CONTINUE;
                _GuiEngine.InitDataGUIComponent<AdsButton, AdsConfigData>(_AdsButton, _GuiEngine.ConfigContainer.GetConfig<AdsServicesConfigData>().GetAdsConfig(AdsServicesConfigData.RVADS_CONTINUE));
                _AdsButton.OnClaimReward += Continue;
            }
            else
            {
                _SubTitle.text = "Niveau " + Data.GameMode.CurrentLevel;
                _Score.text = " + " + Data.CoinCollected;
                _GuiEngine.InitDataGUIComponent<AdsButton, AdsConfigData>(_AdsButton, _GuiEngine.ConfigContainer.GetConfig<AdsServicesConfigData>().GetAdsConfig(AdsServicesConfigData.RVADS_COIN));
                _AdsButton.OnClaimReward += MultiplyCoin;
                if (Data.IsFinished)
                {
                    _Title.text = "Vous avez gagné !";
                    _State = PostGameState.LEVEL_WON;
                    _Animator.SetInteger(Stars, _GuiEngine.ConfigContainer.GetConfig<LevelConfigData>().GetStars(Data.GameMode.CurrentLevel, Data.CoinCollected));
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
            _GuiEngine.GameSave.GetSave<GlobalGameSave>().Wallet.Add(CurrencyType.Coin, Data.CoinCollected);
        }
    }

    public enum PostGameState
    {
        CONTINUE = 0,
        LEVEL_LOST = 1,
        LEVEL_WON = 2,
    }
}
