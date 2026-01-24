using BloomLines.Controllers;
using BloomLines.UI;
using UnityEngine;

namespace BloomLines
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private UIIAPTabBase[] _uIIAPTabBases;

        public void Init()
        {
            int count = 0;
            foreach (UIIAPTabBase item in _uIIAPTabBases)
            {
                bool isPurchased = IAPController.IsPurchased(item.PurchaseId);
                if (isPurchased)
                {
                    Debug.Log("Product " + item.PurchaseId + " was consumed by Shop");                    
                    item.Consume();
                    if (item.Consumable)
                        IAPController.Consume(item.PurchaseId);
                    count++;
                }
            }
            Debug.Log("Shop. Consumed " + count.ToString() + " products");
        }
    }
}
