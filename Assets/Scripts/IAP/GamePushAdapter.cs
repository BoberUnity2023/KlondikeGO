#if GAME_PUSH
using System;
using System.Linq;
using GamePush;
using UnityEngine;

namespace BloomLines.IAP
{
    // Адаптер покупок под GamePush
    public class GamePushAdapter : IIAPAdapter
    {
        private Action<bool> _result;

        public void Initialize()
        {
        }

        public bool CanPurchase(string id)
        {
            if (!GP_Payments.IsPaymentsAvailable())
                return false;

            var product = GP_Payments.Products.FirstOrDefault(e => e.tag == id);
            var purchase = GP_Payments.Purchases.FirstOrDefault(e => e.tag == id);
            return product != null && purchase == null;
        }

        public bool IsPurchased(string id)
        {
            var purchase = GP_Payments.Purchases.FirstOrDefault(e => e.tag == id);
            return purchase != null;
        }

        public string GetPurchasePrice(string id)
        {
            var product = GP_Payments.Products.FirstOrDefault(e => e.tag == id);
            return product == null ? "-" : $"{product.price} {product.currencySymbol}";
        }

        public void Purchase(string id, Action<bool> result)
        {
            _result = result;

            if (CanPurchase(id))
            {
                GP_Payments.Purchase(id, OnPurchaseSuccess, OnPurchaseFailed);
            }
            else
            {
                Debug.Log($"Can't purchase product with id: {id}");
                OnPurchaseFailed();
                return;
            }
        }

        private void OnPurchaseSuccess(string id)
        {
            _result?.Invoke(true);
            _result = null;
        }

        private void OnPurchaseFailed()
        {
            _result?.Invoke(false);
            _result = null;
        }

        public void Consume(string id)
        {
            GP_Payments.Consume(id);
        }
    }
}
#endif