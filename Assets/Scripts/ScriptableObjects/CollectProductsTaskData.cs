using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "CollectProductsTaskData", menuName = "BloomLines/Tasks/CollectProducts")]
    public class CollectProductsTaskData : TaskData
    {
        [SerializeField] private int _productsTypeCount;

        public int ProductsTypeCount => _productsTypeCount;
    }
}