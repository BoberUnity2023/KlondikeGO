using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class BoardObjectsFactory : MonoBehaviour
    {
        [SerializeField] private int _initializeCount;

        private Dictionary<string, ObjectPoolController<BoardObjectBase>> _pool;

        private void Awake()
        {
            var objects = Resources.LoadAll<BoardObjectBase>("Prefabs/BoardObjects");
            _pool = new Dictionary<string, ObjectPoolController<BoardObjectBase>>();

            foreach (var obj in objects)
            {
                _pool.Add(obj.Data.Id, new ObjectPoolController<BoardObjectBase>(obj));

                for (int i = 0; i < _initializeCount; i++)
                    _pool[obj.Data.Id].GetObject();

                _pool[obj.Data.Id].ResetAllObjects();
            }
        }

        public BoardObjectBase CreateObject(string id)
        {
            return _pool[id].GetObject();
        }
    }
}