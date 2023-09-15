using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoakleEngine
{
    public class UISkin : MonoBehaviour
    {
        [SerializeField] private UISkinData _UISkinData = null;
        [SerializeField] private Image _Header = null;
        [SerializeField] private Image _Body = null;

        [ExecuteInEditMode]
        private void Update()
        {
            if (Application.isEditor)
                SetSkin();
        }
        
        private void SetSkin()
        {
            if( _UISkinData == null)
                return;

            _Header.color = _UISkinData.HeaderColor;
            _Body.color = _UISkinData.BodyColor;
        }
    }
}
