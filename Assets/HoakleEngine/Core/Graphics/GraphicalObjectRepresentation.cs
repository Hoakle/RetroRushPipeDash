using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Graphics
{
    public abstract class GraphicalObjectRepresentation<TData> : GraphicalObjectRepresentation
    {
        public TData Data { get; set; }
    }
    
    public abstract class GraphicalObjectRepresentation : MonoBehaviour
    {
        protected GraphicsEngine _GraphicsEngine;
        private List<GraphicalObjectRepresentation> _SubGORs = new List<GraphicalObjectRepresentation>();

        public Action<GraphicalObjectRepresentation> OnDispose;

        protected bool _IsReady;
        public bool IsReady => _IsReady;

        public void LinkEngine(GraphicsEngine graphicsEngine)
        {
            _GraphicsEngine = graphicsEngine;
        }

        public virtual void OnReady()
        {
            _IsReady = true;
        }
        public virtual void Dispose()
        {
            Type type = GetType();
            while (_SubGORs.Count > 0)
            {
                _SubGORs[0].OnDispose -= RemoveGOR;
                _SubGORs[0].Dispose();
                _SubGORs.RemoveAt(0);
            }
            
            OnDispose?.Invoke(this);

            if (_GraphicsEngine == null)
                return;

            _IsReady = false;
            _GraphicsEngine.Dispose(type, gameObject);
        }

        protected void AddGOR<T, TData>(string key, TData data, Transform parent = null, Action<T> onInstanciated = null) where T : GraphicalObjectRepresentation<TData>
        {
            _GraphicsEngine.CreateGraphicalRepresentation<T, TData>(key, data, parent, (gor) =>
            {
                gor.OnDispose += RemoveGOR;
                _SubGORs.Add(gor);
                onInstanciated?.Invoke(gor);
            });
        }

        protected void RemoveGOR(GraphicalObjectRepresentation gor)
        {
            gor.OnDispose -= RemoveGOR;
            _SubGORs.Remove(gor);
        }
    }
}
