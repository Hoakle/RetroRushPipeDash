using HoakleEngine.Core;
using RetroRush.Game.Economics;
using UnityEngine;

namespace Scripts.Engine
{
    public class RetroRush : GameRoot
    {
        protected override void Init()
        {
            base.Init();
            _GameEngine.LinkEngine(_GraphicsEngine);
            _GraphicsEngine.LinkEngine(_GameEngine);
            _GraphicsEngine.Init();
            _GameEngine.Init();
        }

        public void Update()
        {
            _GameEngine.Update(_IsPaused);
            _GraphicsEngine.Update(_IsPaused);
        }
        
        #region GUI

        private bool _GUIOpened;
        private Rect _GUIRect;

        private CurrencyType _GUICurrencyType;
        private int _GUICurrencyAmount;
        private void OnGUI()
        {
            /*_GUIRect = new Rect(10, 10, 150, 100);
            if (GUI.Button(_GUIRect, ""))
            {
                _GUIOpened = !_GUIOpened;
            }

            if (_GUIOpened)
            {
                _GUIRect.y += _GUIRect.height + 10;
                _GUIRect.width = 300;
                GUI.Label(_GUIRect, "Wallet");

                _GUIRect.width = 150;
                _GUIRect.y += _GUIRect.height;
                foreach (var currency in GameSaveContainer.GetSave<GlobalGameSave>().Wallet.CurrencyTypes)
                {
                    if(GUI.Button(_GUIRect, currency.ToString()))
                    {
                        _GUICurrencyType = currency;
                    }
                    _GUIRect.x += _GUIRect.width;
                }
                
                _GUIRect.width = 300;
                _GUIRect.y += _GUIRect.height;
                _GUIRect.x = 10;
                
                _GUICurrencyAmount = Int32.Parse(GUI.TextField(_GUIRect, _GUICurrencyAmount.ToString()));
                
                _GUIRect.x += _GUIRect.width;
                if (GUI.Button(_GUIRect, "Add"))
                {
                    GameSaveContainer.GetSave<GlobalGameSave>().Wallet.Add(_GUICurrencyType, _GUICurrencyAmount);
                }
            }*/
        }

        #endregion
    }
}