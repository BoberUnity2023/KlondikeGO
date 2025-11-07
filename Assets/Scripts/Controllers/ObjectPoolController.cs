using System.Collections.Generic;
using BloomLines.ObjectPool;
using UnityEngine;

namespace BloomLines.Controllers
{
    // Контроллер пула обьектов
    public class ObjectPoolController<T> where T : PoolObject
    {
        private Stack<T> _allObjects = new Stack<T>();
        private List<T> _globalAllObjects = new List<T>();

        private T _objectPrefab;

        public ObjectPoolController(T mainObject)
        {
            _objectPrefab = mainObject;
        }

        public T GetObject(bool active = true)
        {
            T obj;

            if (_allObjects.Count > 0)
            {
                obj = _allObjects.Pop();

                if (obj == null)
                    obj = GetObject();
            }
            else
            {
                var poolObject = GameObject.Instantiate(_objectPrefab);
                obj = poolObject.GetComponent<T>();

                _globalAllObjects.Add(obj);
                poolObject.onMoveToPool += OnMoveToPool;
            }

            obj.gameObject.SetActive(active);
            return obj;
        }

        public void ResetAllObjects()
        {
            foreach (T obj in _globalAllObjects)
            {
                if (obj == null)
                    continue;

                obj.gameObject.SetActive(false);
                obj.transform.SetParent(null);
            }
        }

        public void OnMoveToPool(PoolObject obj)
        {
            _allObjects.Push(obj as T);
        }
    }
}