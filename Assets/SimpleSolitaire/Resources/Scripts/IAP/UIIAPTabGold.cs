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
                //var gameState = SaveManager.GameState;
                //SetSkinPack(gameState.SkinPack == "skin_pack_1" ? "skin_pack_2" : "skin_pack_1");
            }    
        }

        protected override void OnPurchaseComplete(bool result)
        {
            base.OnPurchaseComplete(result);

            if (result)
            {
                _gameManager.Gold += _gold;
            }
        }

        protected override void UpdatePurchase()
        {
            //var isPurchased = IAPController.IsPurchased(_purchaseId);
            var canPurchase = IAPController.CanPurchase(_purchaseId, _consumable);

            
            if (canPurchase)                                         
                _priceLegacy.text = IAPController.GetPurchasePrice(_purchaseId);
            else
                _priceLegacy.text = "---";

            //gameObject.SetActive(isPurchased || canPurchase);
        }
    }
}