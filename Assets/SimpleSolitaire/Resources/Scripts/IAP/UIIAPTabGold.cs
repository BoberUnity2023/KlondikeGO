using System;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.Skins;
using I2.Loc;
using TMPro;
using UnityEngine;
using SimpleSolitaire.Controller;
using BloomLines.UI;

namespace BloomLines.UI
{
    public class UIIAPTabGold : UIIAPTabBase
    {
        //[SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private int _gold;

        protected override void OnClick()
        {
            if (IAPController.CanPurchase(_purchaseId, _consumable))
            {
                base.OnClick();
            }
            else
            {
                Debug.Log("Product: " + _purchaseId + " you can not purchase");
            }
        }

        /*protected override void OnPurchaseComplete(bool result)
        {
            base.OnPurchaseComplete(result);

            if (result)
            {
                Consume();
            }
        }*/

        protected override void UpdatePurchase()
        {
            //var isPurchased = IAPController.IsPurchased(_purchaseId);
            var canPurchase = IAPController.CanPurchase(_purchaseId, _consumable);
            
            if (canPurchase)                                         
                _priceLegacy.text = IAPController.GetPurchasePrice(_purchaseId);
            else
                _priceLegacy.text = "---";
        }

        public override void Consume()
        {
            _gameManager.Gold += _gold;
        }
    }
}