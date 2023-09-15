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
    }

    public abstract class DataGUI<TData> : GraphicalUserInterface
    {
        public TData Data { get; set; }
    }

    public abstract class GuiComponent : MonoBehaviour
    {
        protected GUIEngine _GuiEngine;
        
        public void LinkEngine(GUIEngine guiEngine)
        {
            _GuiEngine = guiEngine;
        }

        protected void Destroy()
        {
            Destroy(gameObject);
        }
    }

    public abstract class DataGuiComponent<TData> : GuiComponent
    {
        public TData Data { get; set; }
    }
}