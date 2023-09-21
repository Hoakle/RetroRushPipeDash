using System;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using UnityEngine;

namespace RetroRush.Camera
{
    public class StaticCameraControl : CameraControl
    {
        public Action OnTargetReached;
        public StaticCameraControl(UnityEngine.Camera camera) : base(camera)
        {
            
        }

        private float _PosInterpolationDuration;
        private float _RotInterpolationDuration;

        private bool _PosInterpolationDone;
        private bool _RotInterpolationDone;
        protected override void Move()
        {
            if (_Camera.transform.position != _Targets[0].position)
            {
                _Camera.transform.position = Vector3.Lerp(_Camera.transform.position, _Targets[0].position, _PosInterpolationDuration);
                _PosInterpolationDuration += 0.25f * Time.deltaTime;
                
                if (Vector3.Distance(_Camera.transform.position, _Targets[0].position) <= 0.1f)
                {
                    _PosInterpolationDuration = 0f;
                    _Camera.transform.position = _Targets[0].position;
                    _PosInterpolationDone = true;
                }
            }

            if (_Camera.transform.rotation != _Targets[0].rotation)
            {
                _Camera.transform.eulerAngles = Vector3.Lerp(_Camera.transform.eulerAngles, _Targets[0].eulerAngles, _RotInterpolationDuration);
                _RotInterpolationDuration += 0.25f * Time.deltaTime;
                
                if (Vector3.Distance(_Camera.transform.position, _Targets[0].position) <= 0.1f)
                {
                    _RotInterpolationDuration = 0f;
                    _Camera.transform.position = _Targets[0].position;
                    _RotInterpolationDone = true;
                }
            }

            if (_PosInterpolationDone && _RotInterpolationDone)
            {
                OnTargetReached?.Invoke();
                _PosInterpolationDone = false;
                _RotInterpolationDone = false;
            }
        }
        
        protected override void Zoom()
        {
            
        }

        public override void SetStartPositionAndSize()
        {
            _Camera.transform.position = _Targets[0].position;
            _Camera.transform.rotation = _Targets[0].rotation;
        }
    }
}