# if UNITY_ANDROID
using System;
using BloomLines.Helpers;
using GoogleMobileAds.Api;
using UnityEngine;

namespace BloomLines.Ads
{
    public class GooglePlayAdapter : IAdsAdapter
    {
        private GooglePlayBanner _banner;
        private GooglePlayInterstitial _interstitial;
        private GooglePlayRewarded _rewardedVideo;
        private static bool? _isInitialized;

        private Action<bool> _onComplete;

        public void Initialize()
        {
            var updateHelper = UpdateHelper.Get();
            
            if (_isInitialized.HasValue)
            {
                return;
            }

            _isInitialized = false;
            MobileAds.Initialize((InitializationStatus initstatus) => OnInit(initstatus));            
        }

        private void OnInit(InitializationStatus initstatus)
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                _isInitialized = null;
                return;
            }
            // [START_EXCLUDE silent]
            // If you use mediation, you can check the status of each adapter.
            var adapterStatusMap = initstatus.getAdapterStatusMap();
            if (adapterStatusMap != null)
            {
                foreach (var item in adapterStatusMap)
                {
                    Debug.Log(string.Format("Adapter {0} is {1}",
                        item.Key,
                        item.Value.InitializationState));
                }
            }
            // [END_EXCLUDE]

            Debug.Log("Google Mobile Ads initialization complete.");
            _isInitialized = true;

            // Google Mobile Ads events are raised off the Unity Main thread. If you need to
            // access UnityEngine objects after initialization,
            // use MobileAdsEventExecutor.ExecuteInUpdate(). For more information, see:
            // https://developers.google.com/admob/unity/global-settings#raise_ad_events_on_the_unity_main_thread
            _banner = new GooglePlayBanner();
            _interstitial = new GooglePlayInterstitial();
            _rewardedVideo = new GooglePlayRewarded(this);
        }

        public void ShowInterstitial()
        {
            Debug.Log("GooglePlayAdsAdapter: ShowAd");
            _interstitial.ShowAd();
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            Debug.Log("GamePushAdsAdapter: ShowRewarded");

            _onComplete = onComplete;
            _rewardedVideo.ShowAd();            
        }

        public void OnReward(bool success)
        {
            _onComplete?.Invoke(success);
            _onComplete = null;
        }

        public void CloseSticky()
        {
            _banner.HideAd();
        }
    }
}
#endif