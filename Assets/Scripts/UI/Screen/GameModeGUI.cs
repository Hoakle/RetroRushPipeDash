using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.GameData;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RetroRush.UI.Screen
{
    public class GameModeGUI : DataGUI<ProgressionHandler>
    {
        [SerializeField] private Button _CloseButton = null;
        [SerializeField] private Button _EndlessMode = null;
        [SerializeField] private Button _StageMode = null;

        [Header("Stage Selection panel")] 
        [SerializeField] private Transform _GridContent = null;

        private LevelConfigData _LevelConfigData;
        private List<LevelComponent> _LevelComponents = null;
        private static readonly int StageSelection = Animator.StringToHash("StageSelection");

        [Inject]
        public void Inject(LevelConfigData levelConfigData)
        {
            _LevelConfigData = levelConfigData;
        }
        
        public override void OnReady()
        {
            _EndlessMode.onClick.AddListener(EndlessModeSelected);
            _StageMode.onClick.AddListener(StageSelected);
            _CloseButton.onClick.AddListener(Close);

            InitStagePanel();
            base.OnReady();
        }

        private void InitStagePanel()
        {
            _LevelComponents = new List<LevelComponent>();
            foreach (var level in _LevelConfigData.StageConfigs)
            {
                _GuiEngine.CreateDataGUIComponent<LevelComponent, StageConfigData>(GUIKeys.LEVEL_COMPONENT, level, _GridContent, (levelComponent) =>
                {
                    _LevelComponents.Add(levelComponent);
                    levelComponent.OnLevelSelected += LevelSelected;
                });
            }
        }
        
        private void EndlessModeSelected()
        {
            Data.SetGameMode(GameModeType.ENDLESS);
            EventBus.Instance.Publish(EngineEventType.GameModeChange);
            Close();
        }
        
        private void StageSelected()
        {
            Data.SetGameMode(GameModeType.STAGE);
            _Animator.SetTrigger(StageSelection);
            foreach (var levelComponent in _LevelComponents)
            {
                levelComponent.SetStars();
            }
        }

        private void LevelSelected(int level)
        {
            Data.SelectLevel(level);
            EventBus.Instance.Publish(EngineEventType.GameModeChange);
            Close();
        }
        private void OnDestroy()
        {
            Data.Save();
            
            _EndlessMode.onClick.RemoveListener(EndlessModeSelected);
            _StageMode.onClick.RemoveListener(StageSelected);
            _CloseButton.onClick.RemoveListener(Close);
            
            foreach (var level in _LevelComponents)
                level.OnLevelSelected -= LevelSelected;
        }
    }
}
