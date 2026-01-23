using BloomLines.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public abstract class UIIAPTabBase : MonoBehaviour
    {
        [SerializeField] protected string _purchaseId;
        [SerializeField] protected bool _consumable;
        [SerializeField] protected TextMeshProUGUI _price;
        [SerializeField] protected Text _priceLegacy;
        [SerializeField] protected Button _btn;

        private void Awake()
        {
            _btn.onClick.AddListener(OnClick);
            bool isPurchased = IAPController.IsPurchased(_purchaseId);
            if (!_consumable && isPurchased)
            {
                Debug.Log("Product " + _purchaseId + " was consumed");
                Consume(); 
            }
        }

        private void OnEnable()
        {
            UpdatePurchase();
        }

        protected virtual void OnClick()
        {
            IAPController.Purchase(_purchaseId, _consumable, OnPurchaseComplete);
        }

        protected virtual void OnPurchaseComplete(bool result)
        {
            UpdatePurchase();
        }

        protected virtual void UpdatePurchase()
        {
        }

        protected virtual void Consume()
        {
        }
    }
}