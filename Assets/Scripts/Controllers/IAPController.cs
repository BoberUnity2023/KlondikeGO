using System;
//using BloomLines.Ads;
using BloomLines.IAP;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Controllers
{
    // Контроллер игровых покупок
    public static class IAPController
    {
        private static IIAPAdapter _iapAdapter; // Текущий адаптер покупок

        public const string NO_ADS = "no_ads";
        public const string START_OFFER = "start_offer";
        public const string GOLD_1 = "gold_1";
        public const string GOLD_2 = "gold_2";
        public const string GOLD_3 = "gold_3";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            return;
#endif

            // Загружаем текущий адаптер в зависимости от платформы на которой билдимся

#if Yandex
            _iapAdapter = new YandexAdapter();
#endif

#if GAME_PUSH
            _iapAdapter = new GamePushAdapter();
#endif

            _iapAdapter?.Initialize();
        }

        // Загрузить все покупки
        public static void LoadPurchases()
        {
            if (IsPurchased(NO_ADS))
                PurchaseCompleted(NO_ADS);
        }

        // Проверка куплен ли товар
        public static bool IsPurchased(string id)
        {
            //var gameState = SaveManager.GameState;//TODO

#if UNITY_EDITOR
            BloomLines.Saving.GameState gameState = SaveManager.GameState;
            if (gameState == null)
            {
                Debug.LogWarning("gameState == null");
                return false;
            }

            if (gameState.Purchased == null)
            {
                Debug.LogWarning("gameState.Purchased == null");
                return false;
            }
            return gameState.Purchased.Contains(id);
#endif

            if (_iapAdapter == null)
                return false;

            bool isPurchased = _iapAdapter.IsPurchased(id);
            return /*gameState.Purchased.Contains(id) ||*/ isPurchased;
        }

        // Проверка можно ли купить товар
        public static bool CanPurchase(string id)
        {
#if UNITY_EDITOR
            return !IsPurchased(id);
#endif

            if (_iapAdapter == null)
                return false;

            return !IsPurchased(id) && _iapAdapter.CanPurchase(id);
        }

        // Получить цену товара
        public static string GetPurchasePrice(string id)
        {
#if UNITY_EDITOR
            return "0$";
#endif

            if (_iapAdapter == null)
                return "-";

            return _iapAdapter.GetPurchasePrice(id);
        }

        // Купить товар
        public static void Purchase(string id, Action<bool> result)
        {
            var gameState = SaveManager.GameState;

#if UNITY_EDITOR
            PurchaseCompleted(id);
            result?.Invoke(true);
            return;
#endif

            if (_iapAdapter == null)
                return;

            _iapAdapter.Purchase(id, (success) =>
            {
                if(success)
                    PurchaseCompleted(id);

                result?.Invoke(success);

                if (success)
                    _iapAdapter.Consume(id);
            });
        }

        // Покупка удалась
        private static void PurchaseCompleted(string id)
        {
            var gameState = SaveManager.GameState;

            if (!gameState.Purchased.Contains(id))
                gameState.Purchased.Add(id);

            switch (id)
            {
                case NO_ADS:
                    AdsController.CloseSticky();
                    break;
                case START_OFFER:
                    break;
                case GOLD_1:
                    break;
                case GOLD_2:
                    break;
                case GOLD_3:
                    break;
            }

            SaveManager.Save(SaveType.Game);
            SaveManager.Sync();
        }
    }
}