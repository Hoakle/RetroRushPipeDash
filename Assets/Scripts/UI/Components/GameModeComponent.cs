using System.Collections;
using System.Collections.Generic;
using HoakleEngine.Core.Communication;
using HoakleEngine.Core.Graphics;
using RetroRush.Engine;
using RetroRush.GameData;
using RetroRush.GameSave;
using RetroRush.UI.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace RetroRush
{
    public class GameModeComponent : DataGuiComponent<GameModeData>
    {
        [SerializeField] private Animator _Animator = null;
        [SerializeField] private TextMeshProUGUI _GameModeText = null;
        [SerializeField] private Button _SwitchButton = null;
        [SerializeField] private Button _PreviousButton = null;
        [SerializeField] private Button _NextButton = null;

        private GameModeData _GameMode;
        private static readonly int ModeType = Animator.StringToHash("GameModeType");
        private static readonly int Stars = Animator.StringToHash("Stars");

        public override void OnReady()
        {
            _SwitchButton.onClick.AddListener(OpenGameModeSelection);
            _PreviousButton.onClick.AddListener(PreviousLevel);
            _NextButton.onClick.AddListener(NextLevel);
            EventBus.Instance.Subscribe(EngineEventType.GameModeChange, UpdateGameMode);

            _GameMode = _GuiEngine.GameSave.GetSave<GlobalGameSave>().GameMode;
            UpdateGameMode();
        }
        private void OpenGameModeSelection()
        {
            _GuiEngine.CreateDataGUI<GameModeGUI, GameModeData>(GUIKeys.GAMEMODE_GUI, Data);
        }

        private void UpdateGameMode()
        {
            _Animator.SetInteger(ModeType, (int) _GameMode.Type);
            if (_GameMode.Type == GameModeType.ENDLESS)
                _GameModeText.text = "Endless";
            else
            {
                var level = _GameMode.GetLevel(_GameMode.CurrentLevel);
                _GameModeText.text = "Stage " + _GameMode.CurrentLevel;

                _NextButton.gameObject.SetActive(level.Stars != 0 && _GameMode.Levels.Count <= level.Level);
                _PreviousButton.gameObject.SetActive(level.Level != 1);
                
                _Animator.SetInteger(Stars, level.Stars);
            }
        }

        private void NextLevel()
        {
            _GameMode.CurrentLevel += 1;
        }

        private void PreviousLevel()
        {
            _GameMode.CurrentLevel -= 1;
        }
        
        private void OnDestroy()
        {
            _SwitchButton.onClick.AddListener(OpenGameModeSelection);
        }
    }
}
