using System;

namespace BloomLines.IAP
{
    // Адаптер покупок
    public interface IIAPAdapter
    {
        void Initialize();
        bool CanPurchase(string id, bool consumable);
        bool IsPurchased(string id);
        void Purchase(string id, bool consumable, Action<bool> result);
        void Consume(string id);
        string GetPurchasePrice(string id);
    }
}