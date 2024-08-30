using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using HoakleEngine.Core.Localization;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.UI.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRush
{
    public class GameModeComponent : DataGuiComponent<ProgressionHandler>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private LocalizedText _GameModeText = null;
        [SerializeField] private Button _SwitchButton = null;
        [SerializeField] private Button _PreviousButton = null;
        [SerializeField] private Button _NextButton = null;
        
        private static readonly int ModeType = Animator.StringToHash("GameModeType");
        private static readonly int Stars = Animator.StringToHash("Stars");
        
        public override void OnReady()
        {
            _SwitchButton.onClick.AddListener(OpenGameModeSelection);
            _PreviousButton.onClick.AddListener(DecrementLevel);
            _NextButton.onClick.AddListener(IncrementLevel);
            EventBus.Instance.Subscribe(EngineEventType.GameModeChange, UpdateGameMode);
            
            UpdateGameMode();
        }

        private void OpenGameModeSelection()
        {
            _GuiEngine.CreateDataGUI<GameModeGUI, ProgressionHandler>(GUIKeys.GAMEMODE_GUI, Data);
        }

        private void UpdateGameMode()
        {
            _Animator.SetInteger(ModeType, (int) Data.GameModeType);
            if (Data.GameModeType == GameModeType.ENDLESS)
                _GameModeText.SetKey("Home/GameMode_Endless");
            else
            {
                var level = Data.GetLevel(Data.CurrentLevel);
                _GameModeText.SetKey("Home/GameMode_Stage");
                _GameModeText.SetParameters(Data.CurrentLevel.ToString());
                _NextButton.gameObject.SetActive(level.Stars != 0 && Data.Levels.Count > level.Level);
                _PreviousButton.gameObject.SetActive(level.Level != 1);
                _Animator.SetInteger(Stars, level.Stars);
            }
        }

        private void IncrementLevel()
        {
            Data.TryIncrementLevel();
            UpdateGameMode();
        }

        private void DecrementLevel()
        {
            Data.TryDecrementLevel();
            UpdateGameMode();
        }
        
        private void OnDestroy()
        {
            _SwitchButton.onClick.RemoveListener(OpenGameModeSelection);
            _PreviousButton.onClick.RemoveListener(DecrementLevel);
            _NextButton.onClick.RemoveListener(IncrementLevel);
            EventBus.Instance.UnSubscribe(EngineEventType.GameModeChange, UpdateGameMode);
        }
    }
}
