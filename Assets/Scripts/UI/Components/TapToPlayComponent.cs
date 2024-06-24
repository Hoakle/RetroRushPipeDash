using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using HoakleEngine;
using HoakleEngine.Core.Communication;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

namespace RetroRush.UI.Components
{
    public class TapToPlayComponent : MonoBehaviour
    {
        [SerializeField] private BoxCollider _Collider = null;

        private Camera _Camera;

        public bool IsClickable = true;
        
        public Action OnClick;

        [Inject]
        public void Inject(ICameraProvider cameraProvider)
        {
            _Camera = cameraProvider.Camera.Value;
        }
        
        private void Update()
        {
            if(Input.touchCount > 0)
            {
                if (!CanBeClicked())
                    return;
                
                for(int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 pos = touch.position;
                    
                    if (touch.phase == TouchPhase.Ended && IsOverCollider())
                    {
                        OnClick?.Invoke();
                    }
                }
            }
            
#if UNITY_EDITOR
            if(Input.GetMouseButtonUp((int) MouseButton.LeftMouse) && IsOverCollider())
            {
                if (!CanBeClicked())
                    return;
                
                OnClick?.Invoke();
            }
#endif
        }

        private bool IsOverCollider()
        {
            Ray ray;
#if UNITY_EDITOR          
            ray = _Camera.ScreenPointToRay(Input.mousePosition);
#else
            ray = _Camera.ScreenPointToRay(Input.GetTouch(0).position);
#endif
            RaycastHit hit;

            return _Collider.Raycast(ray, out hit, 100f);
        }

        private bool CanBeClicked()
        {
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            // This section prepares a list for all objects hit with the raycast
            cursor.position = Input.mousePosition;
            List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(cursor, objectsHit);
            foreach(RaycastResult rr in objectsHit) {
                if (rr.gameObject.transform is RectTransform) return false;
            }

            return true;
        }
    }
}
