using System;
using UnityEngine;

namespace RetroRush.Game.Level
{
    public class LevelInput : MonoBehaviour
    {
        public Action<float> OnMove;
        
        private float _ScreenWidth;
        private Vector2 _InitialTouchPos;
        private void Awake()
        {
            _ScreenWidth = Screen.width;
            _InitialTouchPos = new Vector2();
        }
        
        private void Update()
        {
            if(Input.touchCount > 0)
            {
                for(int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 pos = touch.position;
                    
                    if(pos.x - _ScreenWidth / 2.0f <= 0)
                    {
                        if(touch.phase == TouchPhase.Began)
                        {
                            _InitialTouchPos = pos;
                        }
                        else if(touch.phase == TouchPhase.Moved)
                        {
                            OnMove?.Invoke((pos.x - _InitialTouchPos.x) * 4);
                            _InitialTouchPos = pos;
                        }
                    }
                }
            }
            else
            {
                //Si les input android ne sont pas utilisé on regarce si les standards le sont
                float horizontalMove = Input.GetAxis("Horizontal");
                if (horizontalMove != 0)
                {
                    OnMove?.Invoke(horizontalMove * -100);
                }
            }
        }
        
    }
}
