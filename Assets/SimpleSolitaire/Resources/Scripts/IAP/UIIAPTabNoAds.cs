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
    public class UIIAPTabNoAds : UIIAPTabBase
    {
        //[SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private AdsController _adsController;

        protected override void OnClick()
        {
            if (_gameManager.Save.NoAds)
            {
                Debug.Log("NoAds was purchased yet!");
                return; 
            }

            if (IAPController.CanPurchase(_purchaseId, _consumable))
            {
                base.OnClick();
            }
            else
            {
                Debug.Log("You can not purchase NoAds :(");
                //var gameState = SaveManager.GameState;
                //SetSkinPack(gameState.SkinPack == "skin_pack_1" ? "skin_pack_2" : "skin_pack_1");
            }    
        }

        protected override void OnPurchaseComplete(bool result)
        {
            base.OnPurchaseComplete(result);

            if (result)
            {
                Debug.Log("NoAds was purchased Success!");
                _gameManager.Save.NoAds = true;
                _adsController.CloseSticky();
                _priceLegacy.text = LocalizationManager.GetTranslation("bought");// Куплено
                _btn.interactable = false;
            }
        }

        protected override void UpdatePurchase()
        {
            var isPurchased = _gameManager.Save.NoAds;// IAPController.IsPurchased(_purchaseId);
            var canPurchase = IAPController.CanPurchase(_purchaseId, _consumable);

            if (isPurchased)
            {
                _priceLegacy.text = LocalizationManager.GetTranslation("bought");// Куплено
                _btn.interactable = false;
            }
            else if (canPurchase)
            {
                //_title.text = LocalizationManager.GetTranslation("Main/new_flowers_iap");                
                _priceLegacy.text = IAPController.GetPurchasePrice(_purchaseId); ;
            }

            gameObject.SetActive(isPurchased || canPurchase);
        }
    }
}