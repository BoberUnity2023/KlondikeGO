using BloomLines.Boards;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class BoardObjectsSpawner : MonoBehaviour
    {
        private BoardObjectsFactory _factory;

        private void Awake()
        {
            _factory = GetComponent<BoardObjectsFactory>();

           SpawnerManager.RegisterBoardObjectsSpawner(this);
        }

        public BoardObjectBase SpawnObject(string id)
        {
            return _factory.CreateObject(id);
        }
    }
}