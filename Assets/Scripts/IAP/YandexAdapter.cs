#if Yandex
using System;
using UnityEngine;
using YG;

namespace BloomLines.IAP
{
    // Адаптер яндекс покупок
    public class YandexAdapter : IIAPAdapter
    {
        private Action<bool> _result;

        public void Initialize()
        {
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
            YG2.onPurchaseFailed += OnPurchaseFailed;
        }

        public bool CanPurchase(string id)
        {
            var purchase = YG2.PurchaseByID(id);
            return purchase != null && purchase.consumed;
        }

        public bool IsPurchased(string id)
        {
            var purchase = YG2.PurchaseByID(id);
            return !purchase.consumed;
        }

        public string GetPurchasePrice(string id)
        {
            var purchase = YG2.PurchaseByID(id);
            return purchase.price;
        }

        public void Purchase(string id, Action<bool> result)
        {
            _result = result;

            if (CanPurchase(id))
            {
                YG2.BuyPayments(id);
            }
            else
            {
                Debug.Log($"Can't purchase product with id: {id}");
                OnPurchaseFailed(id);
                return;
            }
        }

        private void OnPurchaseSuccess(string id)
        {
            _result?.Invoke(true);
            _result = null;
        }

        private void OnPurchaseFailed(string id)
        {
            _result?.Invoke(false);
            _result = null;
        }

        public void Consume(string id)
        {
            YG2.ConsumePurchaseByID(id);
        }
    }
}
#endif