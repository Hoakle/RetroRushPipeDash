using System;
using HoakleEngine.Core.Communication;
using UnityEngine;
using UnityEngine.UIElements;

namespace RetroRush.Game.Player
{
    public class PlayerInputV2 : MonoBehaviour
    {
        public Action OnJump;
        public Action OnSlide;
        public Action<int> OnSideways;

        private Vector2 _InitialTouchPos;
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
                    }
                    else if(touch.phase == TouchPhase.Ended)
                    {
                        OnInputInProgress(pos);
                    }
                }
            }
            
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            {
                _InitialTouchPos = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp((int) MouseButton.LeftMouse))
            {
                OnInputInProgress(Input.mousePosition);
            }
#endif
            
        }

        private void OnInputInProgress(Vector2 pos)
        {
            if (Vector2.Distance(Input.mousePosition, _InitialTouchPos) > 10f)
            {
                var direction = (Vector2) Input.mousePosition - _InitialTouchPos;
                var angle = Vector2.SignedAngle(Vector2.right, direction);
                
                if (angle is < 45 and > -45)
                {
                    EventBus.Instance.Publish<int>(EngineEventType.MoveSideway, 1);
                }
                else if(angle is >= 45 and < 135)
                {
                    OnJump?.Invoke();
                }
                else if(angle is >= 135 or <= -135)
                {
                    EventBus.Instance.Publish<int>(EngineEventType.MoveSideway, -1);
                }
                else
                {
                    OnSlide?.Invoke();
                }
            }
        }
    }
}
