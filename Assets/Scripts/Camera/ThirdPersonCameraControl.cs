using HoakleEngine;
using HoakleEngine.Core.Game;
using HoakleEngine.Core.Graphics;
using UnityEngine;
using Zenject;

namespace RetroRush
{
    public class ThirdPersonCameraControl : CameraControl
    {
        private CameraSettingsData _CameraSettingsData;
        public CameraSettingsData CameraSettingsData => _CameraSettingsData;

        [Inject]
        public void Inject(CameraSettingsData cameraData)
        {
            _CameraSettingsData = cameraData;
        }
        
        protected override void Move()
        {
            Vector3 offset = _Targets[0].forward * _CameraSettingsData.ZOffset;
            _Camera.transform.position = new Vector3(_Targets[0].position.x + offset.x, _CameraSettingsData.YOffset, _Targets[0].position.z + offset.z);
            _Camera.transform.LookAt(new Vector3(_Targets[0].position.x, _CameraSettingsData.YLookAtOffset, _Targets[0].position.z));
        }

        protected override void Zoom()
        {
            
        }

        public override void SetStartPositionAndSize()
        {
            
        }
    }

    public class CameraSettingsData
    {
        public float ZOffset;
        public float YOffset;
        public float YLookAtOffset;

        public CameraSettingsData(float zOffset, float yOffset, float zLookAtOffset)
        {
            ZOffset = zOffset;
            YOffset = yOffset;
            YLookAtOffset = zLookAtOffset;
        }
    }
}
