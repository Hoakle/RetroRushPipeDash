using System.Collections;
using System.Collections.Generic;
using HoakleEngine;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class LeaderboardGUI : DataGUI<LeaderboardData>
    {
        [SerializeField] private Button _Close = null;
        [SerializeField] private Transform _Content = null;
        [SerializeField] private Button _Button = null;
        [SerializeField] private TextMeshProUGUI _ButtonText = null;

        private PlayServicesTP _PlayServices;
        private List<RankingComponent> _Components;

        private bool _IsPlayerCentered = true;

        private Coroutine _Coroutine;
        public override void OnReady()
        {
            _Components = new List<RankingComponent>();
            _Close.onClick.AddListener(Close);
            _Button.onClick.AddListener(SwitchLeaderboard);
            
            _PlayServices = _GuiEngine.ServicesContainer.GetService<PlayServicesTP>();
            _PlayServices.OnScoreLoaded += DisplayScore;

            UpdateInfo();
            
            base.OnReady();
        }
        
        private void OnDestroy()
        {
            if(_Coroutine != null)
                StopCoroutine(_Coroutine);
            
            _PlayServices.OnScoreLoaded -= DisplayScore;
            _Close.onClick.RemoveListener(Close);
            _Button.onClick.RemoveListener(SwitchLeaderboard);
        }

        private int _ComponentCount = 0;
        private void DisplayScore(LeaderboardData data)
        {
            Data = data;
            foreach (Transform child in _Content.transform) {
                Destroy(child.gameObject);
            }
            foreach (var score in Data.Scores)
            {
                _ComponentCount++;
                _GuiEngine.CreateDataGUIComponent<RankingComponent, ScoreData>(GUIKeys.RANKING_COMPONENT, score, _Content,
                    component =>
                    {
                        component.IsOdd = _Components.Count % 2 == 0;
                        component.IsUser = Data.UserData == component.Data;
                        _Components.Add(component);
                    } );
            }

            _Coroutine = StartCoroutine(StopLoading());
        }

        private IEnumerator StopLoading()
        {
            yield return new WaitUntil(() => _Components.Count == _ComponentCount);
            _Animator.SetBool("Loading", false);
        }

        private void UpdateInfo()
        {
            _Animator.SetBool("Loading", true);
            _PlayServices.LoadScore("CgkIybW_k5kQEAIQAg", _IsPlayerCentered);
            _ButtonText.text = _IsPlayerCentered ? "Top 10" : "Ma Position";
        }

        private void SwitchLeaderboard()
        {
            _IsPlayerCentered = !_IsPlayerCentered;
            UpdateInfo();
        }
    }
}
