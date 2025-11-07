using BloomLines.Boards;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class SellItemSpawner : MonoBehaviour
    {
        private SellItemFactory _factory;

        private void Awake()
        {
            _factory = GetComponent<SellItemFactory>();

           SpawnerManager.RegisterSellItemSpawner(this);
        }

        public SellItem SpawnObject()
        {
            return _factory.CreateObject();
        }
    }
}