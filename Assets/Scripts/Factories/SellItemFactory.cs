using BloomLines.Boards;
using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class SellItemFactory : MonoBehaviour
    {
        [SerializeField] private int _initializeCount;

        private ObjectPoolController<SellItem> _pool;

        private void Awake()
        {
            var sellItem = Resources.Load<SellItem>("Prefabs/SellItem");
            _pool = new ObjectPoolController<SellItem>(sellItem);

            for (int i = 0; i < _initializeCount; i++)
                _pool.GetObject();

            _pool.ResetAllObjects();
        }

        public SellItem CreateObject()
        {
            return _pool.GetObject();
        }
    }
}