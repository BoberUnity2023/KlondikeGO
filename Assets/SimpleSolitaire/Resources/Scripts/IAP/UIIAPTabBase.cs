using BloomLines.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public abstract class UIIAPTabBase : MonoBehaviour
    {
        [SerializeField] protected string _purchaseId;
        [SerializeField] protected TextMeshProUGUI _price;
        [SerializeField] protected Text _priceLegacy;
        [SerializeField] private Button _btn;

        private void Awake()
        {
            _btn.onClick.AddListener(OnClick);
        }

        protected virtual void OnClick()
        {
            IAPController.Purchase(_purchaseId, OnPurchaseComplete);
        }

        protected virtual void OnPurchaseComplete(bool result)
        {
            UpdatePurchase();
        }

        protected virtual void UpdatePurchase()
        {
        }

        private void OnEnable()
        {
            UpdatePurchase();
        }
    }
}