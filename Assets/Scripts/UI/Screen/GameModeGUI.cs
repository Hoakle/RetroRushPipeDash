using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Config;
using RetroRush.Engine;
using RetroRush.GameData;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush.UI.Screen
{
    public class GameModeGUI : DataGUI<GameModeData>
    {
        [SerializeField] private Button _CloseButton = null;
        [SerializeField] private Button _EndlessMode = null;
        [SerializeField] private Button _StageMode = null;

        [Header("Stage Selection panel")] 
        [SerializeField] private Transform _GridContent = null;

        private List<LevelComponent> _LevelComponents = null;
        private static readonly int StageSelection = Animator.StringToHash("StageSelection");

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
            foreach (var level in _GuiEngine.ConfigContainer.GetConfig<LevelConfigData>().StageConfigs)
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
            Data.Type = GameModeType.ENDLESS;
            EventBus.Instance.Publish(EngineEventType.GameModeChange);
            Close();
        }
        
        private void StageSelected()
        {
            Data.Type = GameModeType.STAGE;
            _Animator.SetTrigger(StageSelection);
        }

        private void LevelSelected(int level)
        {
            Data.CurrentLevel = level;
            EventBus.Instance.Publish(EngineEventType.GameModeChange);
            Close();
        }
        private void OnDestroy()
        {
            _EndlessMode.onClick.RemoveListener(EndlessModeSelected);
            _StageMode.onClick.RemoveListener(StageSelected);
            _CloseButton.onClick.RemoveListener(Close);
            
            foreach (var level in _LevelComponents)
                level.OnLevelSelected -= LevelSelected;
        }

        protected override void Close()
        {
            _GuiEngine.GameSave.Save();
            base.Close();
        }
    }
}
