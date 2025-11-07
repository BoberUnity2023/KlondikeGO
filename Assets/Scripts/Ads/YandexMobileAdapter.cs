#if RuStore
using System;
using System.Collections;
using BloomLines.Helpers;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace BloomLines.Ads
{
    public class YandexMobileAdapter : IAdsAdapter
    {
        public Action<bool> OnRewarded;

        private string _interstitialAdUnitId = "R-M-11808109-1";
        private string _rewardedAdUnitId = "R-M-11808109-3";
        private string _bannerAdUnitId = "R-M-11808109-2";

        private InterstitialAdLoader interstitialAdLoader;
        private Interstitial interstitial;

        private RewardedAdLoader rewardedAdLoader;
        private RewardedAd rewardedAd;

        private Banner banner;

        public void Initialize()
        {
            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += InterstitialHandleAdLoaded;
            interstitialAdLoader.OnAdFailedToLoad += InterstitialHandleAdFailedToLoad;

            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += RewardedHandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += RewardedHandleAdFailedToLoad;

            RequestInterstitial();
            RequestRewardedAd();
            UpdateHelper.Get().StartCoroutine(RequestBannerCoroutine());
        }

        public void ShowInterstitial()
        {
            if (interstitial == null)
            {
                Debug.Log("YandexMobileAdsAdapter: Interstitial is not ready yet");
                return;
            }

            Debug.Log("YandexMobileAdsAdapter: ShowAd");

            interstitial.OnAdFailedToShow += InterstitialHandleAdFailedToShow;
            interstitial.OnAdDismissed += InterstitialHandleAdDismissed;

            interstitial.Show();
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            if (rewardedAd == null)
            {
                Debug.Log("YandexMobileAdsAdapter: RewardedAd is not ready yet");
                OnRewarded?.Invoke(false);
                return;
            }

            Debug.Log("YandexMobileAdsAdapter: ShowRewarded");

            rewardedAd.OnAdFailedToShow += RewardedHandleAdFailedToShow;
            rewardedAd.OnAdDismissed += RewardedHandleAdDismissed;
            rewardedAd.OnRewarded += RewardedHandleRewarded;

            rewardedAd.Show();
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        #region Banner

        private float _bannerHeight;

        private IEnumerator RequestBannerCoroutine()
        {
            var delay = new WaitForSeconds(120f);

            while (true)
            {
                RequestBanner();

                yield return delay;
            }
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private void RequestBanner()
        {
            MobileAds.SetAgeRestrictedUser(true);

            if (banner != null)
            {
                _bannerHeight = 0;
                //Assets.GameAssets.BannerHeight = 0;
                banner.Destroy();
            }

            BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            banner = new Banner(_bannerAdUnitId, bannerSize, AdPosition.TopLeft);

            _bannerHeight = bannerSize.Height;

            banner.OnAdLoaded += BannerHandleAdLoaded;
            banner.OnAdFailedToLoad += BannerHandleAdFailedToLoad;

            banner.LoadAd(CreateAdRequest());
            Debug.Log("YandexMobileAdsAdapter: Banner is requested");
        }

        private int GetScreenWidthDp()
        {
            int screenWidth = (int)Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }

        public void BannerHandleAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("YandexMobileAdsAdapter: BannerHandleAdLoaded event received");

            banner.Show();
            //Assets.GameAssets.BannerHeight = (int)_bannerHeight;
        }

        public void BannerHandleAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            Debug.Log("YandexMobileAdsAdapter: BannerHandleAdFailedToLoad event received with message: " + args.Message);

            RequestBanner();
        }

        public void CloseSticky()
        {
            banner.Hide();
        }
        #endregion

        #region Rewarded
        private void RequestRewardedAd()
        {
            MobileAds.SetAgeRestrictedUser(true);

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            rewardedAdLoader.LoadAd(CreateAdRequest(_rewardedAdUnitId));
            Debug.Log("YandexMobileAdsAdapter: Rewarded Ad is requested");
        }

        private void RewardedHandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            Debug.Log("YandexMobileAdsAdapter: RewardedHandleAdLoaded event received");
            rewardedAd = args.RewardedAd;
        }

        private void RewardedHandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log($"YandexMobileAdsAdapter: RewardedHandleAdFailedToLoad event received with message: {args.Message}");

            RequestRewardedAd();
        }

        private void RewardedHandleAdDismissed(object sender, EventArgs args)
        {
            Debug.Log("YandexMobileAdsAdapter: RewardedHandleAdDismissed event received");

            rewardedAd.Destroy();
            rewardedAd = null;

            RequestRewardedAd();
        }

        private void RewardedHandleRewarded(object sender, Reward args)
        {
            Debug.Log($"YandexMobileAdsAdapter: RewardedHandleRewarded event received: amout = {args.amount}, type = {args.type}");

            OnRewarded?.Invoke(true);

            RequestRewardedAd();
        }

        private void RewardedHandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.Log($"YandexMobileAdsAdapter: RewardedHandleAdFailedToShow event received with message: {args.Message}");

            OnRewarded?.Invoke(false);
            RequestRewardedAd();
        }
        #endregion

        #region Interstitial
        private void RequestInterstitial()
        {
            MobileAds.SetAgeRestrictedUser(true);

            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            interstitialAdLoader.LoadAd(CreateAdRequest(_interstitialAdUnitId));

            Debug.Log("YandexMobileAdsAdapter: Interstitial is requested");
        }

        private void InterstitialHandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            Debug.Log("YandexMobileAdsAdapter: InterstitialHandleAdLoaded event received");

            interstitial = args.Interstitial;
        }

        private void InterstitialHandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log($"YandexMobileAdsAdapter: InterstitialHandleAdFailedToLoad event received with message: {args.Message}");
            RequestInterstitial();
        }

        private void InterstitialHandleAdDismissed(object sender, EventArgs args)
        {
            Debug.Log($"YandexMobileAdsAdapter: InterstitialHandleAdDismissed event received");

            interstitial.Destroy();
            interstitial = null;

            RequestInterstitial();
        }

        private void InterstitialHandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.Log($"YandexMobileAdsAdapter: InterstitialHandleAdFailedToShow event received with message: {args.Message}");
            RequestInterstitial();
        }        
        #endregion
    }
}
#endif