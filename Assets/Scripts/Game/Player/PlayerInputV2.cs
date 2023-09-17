using System;
using UnityEngine;

namespace RetroRush.Game.PlayerNS
{
    public class PlayerInputV2 : MonoBehaviour
    {
        public Action OnJump;
        public Action OnSlide;
        public Action<int> OnSideways;

        private Vector2 _InitialTouchPos;

        private bool _InputInProgress;
        void Update()
        {
            if(Input.touchCount > 0)
            {
                for(int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 pos = touch.position;
                    
                    if (touch.phase == TouchPhase.Began)
                    {
                        _InitialTouchPos = pos;
                        _InputInProgress = true;
                    }
                    else if(touch.phase == TouchPhase.Moved && _InputInProgress)
                    {
                        Debug.LogError("Moved");
                        if (Vector2.Distance(pos, _InitialTouchPos) > 1f)
                        {
                            var direction = pos - _InitialTouchPos;
                            var angle = Vector2.SignedAngle(Vector2.right, direction);
                            if (angle is < 45 and > -45)
                            {
                                Debug.LogError("Side 1");
                                OnSideways?.Invoke(1);
                            }
                            else if(angle is >= 45 and < 135)
                            {
                                Debug.LogError("jump");
                                OnJump?.Invoke();
                            }
                            else if(angle is >= 135 and < 180)
                            {
                                Debug.LogError("Slide");
                                OnSlide?.Invoke();
                            }
                            else
                            {
                                Debug.LogError("Side -1");
                                OnSideways?.Invoke(-1);
                            }
                        }
                    }
                }
            }
        }
    }
}
