using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoakleEngine.Core.Graphics
{
    public class GraphicsPool : MonoBehaviour
    {
        private Dictionary<Type, List<GameObject>> _Pool = new Dictionary<Type, List<GameObject>>();

        public GameObject GetGraphics<T>()
        {
            Type type = typeof(T);
            if (_Pool.ContainsKey(type))
            {
                var graphics = _Pool[type][0];
                _Pool[type].RemoveAt(0);
                if (_Pool[type].Count == 0)
                    _Pool.Remove(type);

                return graphics;
            }

            return null;
        }

        public bool AddToPool(Type type, GameObject gameObject)
        {
            if (!_Pool.ContainsKey(type))
            {
                _Pool.Add(type, new List<GameObject>());
            }
            
            _Pool[type].Add(gameObject);

            MoveAndResetObject(gameObject);
            return true;
        }

        private void MoveAndResetObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.parent = this.transform;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localRotation = Quaternion.identity;
        }
    }
}
