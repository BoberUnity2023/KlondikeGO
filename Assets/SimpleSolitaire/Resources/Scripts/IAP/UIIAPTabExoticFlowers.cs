using System;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.Skins;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace BloomLines.UI
{
    public class UIIAPTabExoticFlowers : UIIAPTabBase
    {
        [SerializeField] private TextMeshProUGUI _title;

        protected override void OnClick()
        {
            if (IAPController.CanPurchase(_purchaseId))
            {
                base.OnClick();
            }
            else
            {
                var gameState = SaveManager.GameState;
                SetSkinPack(gameState.SkinPack == "skin_pack_1" ? "skin_pack_2" : "skin_pack_1");
            }    
        }

        protected override void OnPurchaseComplete(bool result)
        {
            base.OnPurchaseComplete(result);

            SetSkinPack("skin_pack_2");
        }

        private void SetSkinPack(string id)
        {
            var gameState = SaveManager.GameState;
            var skinPack = GameAssets.GetSkinPackData(id);
            gameState.SkinPack = id;

            EventsManager.Publish(new UpdateSkinPackEvent());

            SaveManager.Save(SaveType.Game);

            UpdatePurchase();
        }

        protected override void UpdatePurchase()
        {
            var isPurchased = IAPController.IsPurchased(_purchaseId);
            var canPurchase = IAPController.CanPurchase(_purchaseId);

            if (isPurchased)
            {
                var gameState = SaveManager.GameState;

                _price.text = LocalizationManager.GetTranslation("Main/change");

                if (gameState.SkinPack == "skin_pack_1")
                    _title.text = LocalizationManager.GetTranslation("Main/default_flowers");
                else if(gameState.SkinPack == "skin_pack_2")
                    _title.text = LocalizationManager.GetTranslation("Main/new_flowers_iap");
            }
            else if (canPurchase)
            {
                _title.text = LocalizationManager.GetTranslation("Main/new_flowers_iap");
                _price.text = IAPController.GetPurchasePrice(_purchaseId);
            }

            gameObject.SetActive(isPurchased || canPurchase);
        }
    }
}