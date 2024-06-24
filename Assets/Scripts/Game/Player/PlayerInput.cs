using System;
using UnityEngine;

namespace RetroRush.Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Action OnJump;

        private float _ScreenWidth;
        private void Awake()
        {
            _ScreenWidth = Screen.width;
        }

        void Update()
        {
            if(Input.touchCount > 0)
            {
                for(int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 pos = touch.position;
                    
                    if (pos.x - _ScreenWidth / 2.0f > 0 && touch.phase == TouchPhase.Began)
                    {
                        OnJump?.Invoke();
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    OnJump?.Invoke();
                }
            }
        }
    }
}
