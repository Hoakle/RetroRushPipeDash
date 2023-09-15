using UnityEditor;
using UnityEngine;

namespace HoakleEngine.Editors.Utils
{
    public class Utils : MonoBehaviour
    {
        [MenuItem("UI/Anchor Around Object &q")]
        static void uGUIAnchorAroundObject()
        {
            var selected = Selection.gameObjects;

            foreach (var o in selected)
            {
                if (o != null && o.GetComponent<RectTransform>() != null)
                {
                    var r = o.GetComponent<RectTransform>();
                    Undo.RecordObject(r, "Adjust anchor to rect size");
                    var p = o.transform.parent.GetComponent<RectTransform>();

                    var offsetMin = r.offsetMin;
                    var offsetMax = r.offsetMax;
                    var _anchorMin = r.anchorMin;
                    var _anchorMax = r.anchorMax;

                    var parent_width = p.rect.width;
                    var parent_height = p.rect.height;

                    var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                        _anchorMin.y + (offsetMin.y / parent_height));
                    var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                        _anchorMax.y + (offsetMax.y / parent_height));

                    r.anchorMin = anchorMin;
                    r.anchorMax = anchorMax;

                    r.offsetMin = new Vector2(0, 0);
                    r.offsetMax = new Vector2(0, 0);
                    r.pivot = new Vector2(0.5f, 0.5f);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(r);
                }
            }
        }
    }
}
