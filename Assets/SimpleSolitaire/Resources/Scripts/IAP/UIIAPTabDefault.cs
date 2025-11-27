using System.Collections;
using System.Collections.Generic;
using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.UI
{
    public class UIIAPTabDefault : UIIAPTabBase
    {
        protected override void UpdatePurchase()
        {
            string price = IAPController.GetPurchasePrice(_purchaseId);
            if (_price != null)
                _price.text = price;

            if (_priceLegacy != null)
                _priceLegacy.text = price;

            gameObject.SetActive(!IAPController.IsPurchased(_purchaseId));
        }
    }
}