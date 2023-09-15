using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Graphics
{
    public interface IUserInterface
    {
        public GameObject GetFirstSelected { get; }
    }

    public abstract class GraphicalUserInterface : MonoBehaviour, IUserInterface
    {
        [SerializeField] private Canvas _Canvas = null;
        public Canvas Canvas => _Canvas;

        public GameObject GetFirstSelected { get; }

        protected GUIEngine _GuiEngine;
        
        protected List<GuiComponent> _SubGUIs = new List<GuiComponent>();
        
        public Action<GraphicalUserInterface> OnDispose;

        protected bool _IsReady;
        public bool IsReady => _IsReady;
        public void LinkEngine(GUIEngine guiEngine)
        {
            _GuiEngine = guiEngine;
        }

        protected void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void OnReady()
        {
            
        }
        
        public virtual void Dispose()
        {
            Type type = GetType();
            while (_SubGUIs.Count > 0)
            {
                _SubGUIs[0].OnDispose -= RemoveGuiComponent;
                _SubGUIs[0].Dispose();
                _SubGUIs.RemoveAt(0);
            }
            
            OnDispose?.Invoke(this);

            if (_GuiEngine == null)
                return;

            _IsReady = false;
            Destroy(gameObject);
        }

        protected void AddGuiComponent<T, TData>(string key, TData data, Transform parent = null, Action<T> onInstanciated = null) where T : DataGuiComponent<TData>
        {
            _GuiEngine.CreateDataGUIComponent<T, TData>(key, data, parent, (gor) =>
            {
                gor.OnDispose += RemoveGuiComponent;
                _SubGUIs.Add(gor);
                onInstanciated?.Invoke(gor);
            });
        }

        protected void RemoveGuiComponent(GuiComponent gor)
        {
            gor.OnDispose -= RemoveGuiComponent;
            _SubGUIs.Remove(gor);
        }
    }

    public abstract class DataGUI<TData> : GraphicalUserInterface
    {
        public TData Data { get; set; }
    }

    public abstract class GuiComponent : MonoBehaviour, IUserInterface
    {
        protected GUIEngine _GuiEngine;
        protected List<GuiComponent> _SubGUIs = new List<GuiComponent>();
        
        public Action<GuiComponent> OnDispose;

        protected bool _IsReady;
        public bool IsReady => _IsReady;
        public void LinkEngine(GUIEngine guiEngine)
        {
            _GuiEngine = guiEngine;
        }

        public virtual void Dispose()
        {
            Type type = GetType();
            while (_SubGUIs.Count > 0)
            {
                _SubGUIs[0].OnDispose -= RemoveGuiComponent;
                _SubGUIs[0].Dispose();
                _SubGUIs.RemoveAt(0);
            }
            
            OnDispose?.Invoke(this);

            if (_GuiEngine == null)
                return;

            _IsReady = false;
            Destroy(gameObject);
        }

        protected void AddGuiComponent<T, TData>(string key, TData data, Transform parent = null, Action<T> onInstanciated = null) where T : DataGuiComponent<TData>
        {
            _GuiEngine.CreateDataGUIComponent<T, TData>(key, data, parent, (gor) =>
            {
                gor.OnDispose += RemoveGuiComponent;
                _SubGUIs.Add(gor);
                onInstanciated?.Invoke(gor);
            });
        }

        protected void RemoveGuiComponent(GuiComponent gor)
        {
            gor.OnDispose -= RemoveGuiComponent;
            _SubGUIs.Remove(gor);
        }

        public GameObject GetFirstSelected { get; }
    }

    public abstract class DataGuiComponent<TData> : GuiComponent
    {
        public TData Data { get; set; }
    }
}