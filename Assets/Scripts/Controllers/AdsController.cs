using System;
using BloomLines.Ads;
using BloomLines.Helpers;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Controllers
{
    public static class AdsController
    {
        private static IAdsAdapter _adsAdapter; // Текущий рекламный адаптер
        private static float _timeToInterstitial;

        // Инициализируем контроллер при загрузке сцены
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var balanceData = BalanceManager.Get();
            _timeToInterstitial = balanceData.SecondsToInterstitial;
            var updateHelper = UpdateHelper.Get();
            updateHelper.OnUpdate += Update;

            EventsManager.Subscribe<MakeMoveEvent>(OnMakeMove);

#if UNITY_EDITOR
            return;
#endif
            
            // Загружаем нужный рекламный адаптер и инициализируем её

#if Yandex
            _adsAdapter = new YandexAdapter();
#endif

#if GAME_PUSH
            _adsAdapter = new GamePushAdapter();
#endif

#if RuStore
            _adsAdapter = new YandexMobileAdapter();
#endif

#if UNITY_ANDROID && GooglePlay
            _adsAdapter = new GooglePlayAdapter();
#endif

            _adsAdapter?.Initialize();
        }

        private static void Update()
        {
            _timeToInterstitial -= Time.deltaTime;
        }

        private static void OnMakeMove(MakeMoveEvent eventData)
        {
            if (_timeToInterstitial <= 0)
            {
                var balanceData = BalanceManager.Get();
                _timeToInterstitial = balanceData.SecondsToInterstitial;
                ShowInterstitial();
            }
        }

        public static void ShowInterstitial()
        {
            Debug.Log("AdsController: Show Interstitial");

            var gameState = SaveManager.GameState;
            if (gameState.Purchased.Contains(IAPController.NO_ADS)) // Если купленно отключение рекламы
                return;

            AnalyticsController.SendEvent("ads_show_interstitial");
#if UNITY_EDITOR
            return;
#endif

            _adsAdapter?.ShowInterstitial(); // Показываем рекламу из текущего адаптера
        }

        public static void ShowRewarded(Action<bool> onComplete)
        {
            Debug.Log("AdsController: Show Rewarded");

            //var gameState = SaveManager.GameState;//TODO: !!!
            //if (gameState.Purchased.Contains(IAPController.NO_ADS)) // Если купленно отключение рекламы
            //{
                //onComplete?.Invoke(true);
                //AnalyticsController.SendEvent("ads_show_rewarded");
                //return;
            //}
            
#if UNITY_EDITOR
            onComplete?.Invoke(true);
            return;
#endif

            if (_adsAdapter == null)
            {
                Debug.LogError("_adsAdapter == null");
                onComplete?.Invoke(true);
                return;
            }

            _adsAdapter.ShowRewarded(onComplete); // Показываем рекламу из текущего адаптера
        }

        public static void CloseSticky()
        {
            if (_adsAdapter != null)
            {
                _adsAdapter.CloseSticky();
            }
#if Yandex
            YG.YG2.StickyAdActivity(false);
#endif

#if GAME_PUSH
            GamePush.GP_Ads.CloseSticky();
#endif

#if GooglePlay
            //BloomLines.Ads.GooglePlayAdapter.
#endif
        }
    }
}