using SimpleSolitaire.Controller;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if RuStore
using YandexMobileAds;
using YandexMobileAds.Base;
#endif

public class AdsController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    //[SerializeField] private VKManager _vkManager;
    [SerializeField] private float _interval;
    private float _lastAdsTime;
#if RuStore
    public Action<bool> OnRewarded;

    private string _interstitialAdUnitId = "R-M-11808109-1";
    private string _rewardedAdUnitId = "R-M-11808109-3";
    private string _bannerAdUnitId = "R-M-11808109-2";

    private InterstitialAdLoader interstitialAdLoader;
    private Interstitial interstitial;

    private RewardedAdLoader rewardedAdLoader;
    private RewardedAd rewardedAd;

    private Banner banner;
#endif

    private bool IsReady
    {
        get => Time.time - _lastAdsTime > _interval;
    }

    private void Start()
    {
        StartCoroutine(AfterStart());
    }

    private IEnumerator AfterStart()
    {
        yield return new WaitForSeconds(5);
        if (_gameManager.Platform == Platform.VK)
        {
            //_vkManager.ShowBanners();
            //_vkManager.WebAppTrackEvent();
        }

        if (_gameManager.Platform == Platform.Yandex)
        {
#if Yandex
            if (!_gameManager.Save.NoAds)
                YG.YG2.StickyAdActivity(true);
#endif
#if RuStore
            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += InterstitialHandleAdLoaded;
            interstitialAdLoader.OnAdFailedToLoad += InterstitialHandleAdFailedToLoad;

            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += RewardedHandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += RewardedHandleAdFailedToLoad;

            RequestInterstitial();
            RequestRewardedAd();
#endif
        }
    }

    public void TryShowInterstitial()
    {
        if (!_gameManager.Save.NoAds && IsReady)
        {            
            _lastAdsTime = Time.time;
            ShowInterstitial();
        }        
    }

    private void ShowInterstitial()
    { 
        if (_gameManager.Platform == Platform.Yandex)
        {
            YG.YG2.InterstitialAdvShow();
        }

        if (_gameManager.Platform == Platform.Ok)
        {
            OKManager.ShowInterstitial();
            //OKManager.LoadAd();
        }

        if (_gameManager.Platform == Platform.VK)
        {
#if GAME_PUSH
            GamePush.GP_Ads.ShowFullscreen();
#endif
        }         
        
        if (_gameManager.Platform == Platform.GD)
        {
            GameDistribution.Instance.ShowAd();
        }
    }

    public void CloseSticky()
    {
#if Yandex
        YG.YG2.StickyAdActivity(false);
#endif

#if GAME_PUSH
        GamePush.GP_Ads.CloseSticky(); ;
#endif
    }


    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }

    #region Rewarded RuStore
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

    #region Interstitial RuStore
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
