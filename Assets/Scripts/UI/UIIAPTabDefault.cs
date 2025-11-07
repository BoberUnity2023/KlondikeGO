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
            _price.text = IAPController.GetPurchasePrice(_purchaseId);
            gameObject.SetActive(!IAPController.IsPurchased(_purchaseId));
        }
    }
}