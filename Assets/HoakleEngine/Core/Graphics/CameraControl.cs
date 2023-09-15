using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Graphics
{
    public abstract class CameraControl : IUpdateable
    {
        protected List<Transform> _Targets; // All the targets the camera needs to encompass.
        protected Camera _Camera;
        protected CameraControl (Camera camera)
        {
            _Camera = camera;
            _Targets = new List<Transform>();
        }

        public virtual void AddTarget(Transform target)
        {
            _Targets.Add(target);
        }

        public virtual void RemoveTarget(Transform target)
        {
            _Targets.Remove(target);
        }

        public void Update()
        {
            if (_Targets.Count == 0)
                return;
            
            // Move the camera towards a desired position.
            Move ();

            // Change the size of the camera based.
            Zoom ();
        }

        protected abstract void Move();

        protected abstract void Zoom();
        public abstract void SetStartPositionAndSize();
    }
}
