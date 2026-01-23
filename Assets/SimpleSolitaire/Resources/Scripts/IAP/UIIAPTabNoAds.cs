using BloomLines.Controllers;
using I2.Loc;
using UnityEngine;
using SimpleSolitaire.Controller;

namespace BloomLines.UI
{
    public class UIIAPTabNoAds : UIIAPTabBase
    {
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
            }    
        }

        protected override void OnPurchaseComplete(bool result)
        {
            base.OnPurchaseComplete(result);

            if (result)
            {
                Debug.Log("NoAds was purchased Success!");
                Consume();
            }
        }

        protected override void UpdatePurchase()
        {
            var isPurchased = IAPController.IsPurchased(_purchaseId) || _gameManager.Save.NoAds;
            var canPurchase = IAPController.CanPurchase(_purchaseId, _consumable);

            if (isPurchased)
            {
                Consume();
            }
            else if (canPurchase)
            {                              
                _priceLegacy.text = IAPController.GetPurchasePrice(_purchaseId);
            }
        }

        protected override void Consume()
        {
            Debug.Log("NoAds was consumed Success!");
            _gameManager.Save.NoAds = true;
            _adsController.CloseSticky();
            _priceLegacy.text = LocalizationManager.GetTranslation("bought");// Куплено
            _btn.interactable = false;            
        }
    }
}