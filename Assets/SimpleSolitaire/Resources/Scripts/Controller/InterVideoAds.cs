//using GoogleMobileAds.Api;
using SimpleSolitaire.Model.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [Serializable]
    public class AdsIds
    {
        public string InterId;
        public string RewardId;
        public string BannerId;
    }

    public class AdsData
    {
        public AdsIds Ids;

        public bool IsTestADS;
        public bool IsBanner;
        public bool IsIntersitial;
        public bool IsReward;
        public bool IsHandeldAction;
    }

    public class InterVideoAds : MonoBehaviour
    {
        public static Action<RewardAdsState, RewardAdsType> RewardAction { get; set; }

        [SerializeField]
        private GameManager _gameManagerComponent;
        [SerializeField]
        private UndoPerformer _undoPerformerComponent;

        [SerializeField]
        private AdsIds _androidIds;
        [SerializeField]
        private AdsIds _iosIds;

        //private string _interId;
        //private string _rewardId;
        //private string _bannerId;

        //private readonly string _testBannerId = "ca-app-pub-3940256099942544/6300978111";
        //private readonly string _testIntersitialId = "ca-app-pub-3940256099942544/1033173712";
        //private readonly string _testRewardId = "ca-app-pub-3940256099942544/5224354917";

        [Space(5f)]
        [SerializeField]
        private bool _intersitialRepeatCall;
        [SerializeField]
        private int _intersitialCallsBorder = 3;
        [SerializeField]
        private int _firstCallIntersitialTime;
        [SerializeField]
        private int _repeatIntersitialTime;

        private int _intersitialCallsCounter = 0;

        /*private AdRequest _requestAdmob;
        private BannerView _bannerView;
        private InterstitialAd _interstitial;

        private RewardedAd _rewardVideo;*/

        [SerializeField]
        private bool _isTestADS;
        [SerializeField]
        private bool _isBanner;
        [SerializeField]
        private bool _isIntersitial;
        [SerializeField]
        private bool _isReward;
        [SerializeField]
        private bool _isHandeldAction;

        //private AdSize _currentBannerSize = AdSize.Banner;

        public readonly string NoAdsKey = "NoAds";

        private RewardAdsType _lastShowingType = RewardAdsType.None;
        //private RewardVideoStatus _lastRewardVideoStatus = RewardVideoStatus.None;

        //private bool _isRewarded = false;

        private void Start()
        {
            //InitializeADS();
        }

        private void OnDestroy()
        {
            HideBanner();
        }

        /// <summary>
        /// Initialize admob requests variable.
        /// </summary>
        private void AdMobRequest()
        {
            /*if (_isTestADS)
            {
                List<string> deviceIds = new List<string>()
                {
                    SystemInfo.deviceUniqueIdentifier
                };

                RequestConfiguration requestConfiguration = new RequestConfiguration
                    .Builder()
                    .SetTestDeviceIds(deviceIds)
                    .build();
                MobileAds.SetRequestConfiguration(requestConfiguration);
            }

            _requestAdmob = new AdRequest.Builder().Build();*/
        }

        #region Requests ADS
        /// <summary>
        /// Banned ad request.
        /// </summary>
        private void ShowBanner()
        {
            /*if (IsHasKeyNoAds())
                return;
            _bannerView = new BannerView((_isTestADS) ? _testBannerId : _bannerId, _currentBannerSize, AdPosition.Bottom);

            if (_bannerView != null)
            {
                AdMobRequest();
                _bannerView.LoadAd(_requestAdmob);
            }*/
        }

        /// <summary>
        /// Intersitial video request.
        /// </summary>
        public void RequestInterstitial()
        {
            if (IsHasKeyNoAds())
                return;

            /*_interstitial = new InterstitialAd((_isTestADS) ? _testIntersitialId : _interId);
            AdMobRequest();
            _interstitial.LoadAd(_requestAdmob);*/
        }

        /// <summary>
        /// Reward video request.
        /// </summary>
        private void RequestRewardBasedVideo(bool isRequiredRequest = false)
        {
            /*if (IsHasKeyNoAds() && !isRequiredRequest)
                return;

            var adUnitId = _isTestADS ? _testRewardId : _rewardId;
            _rewardVideo = new RewardedAd(adUnitId);

            AdMobRequest();
            _rewardVideo.LoadAd(_requestAdmob);*/
        }

        #endregion

        #region Show/Hide ADS
 
        public void TryShowIntersitialByCounter()
        {
            if (++_intersitialCallsCounter >= _intersitialCallsBorder)
            {
                _intersitialCallsCounter = 0;
                ShowInterstitial();
            }
        }

        /// <summary>
        /// Show intersitial ads <see cref="_interstitial"/> if ads available for watch.
        /// </summary>
        public void ShowInterstitial()
        {
            if (IsHasKeyNoAds())
                return;

            /*if (_interstitial.IsLoaded())
            {
                _interstitial.Show();

                _interstitial = new InterstitialAd((_isTestADS) ? _testIntersitialId : _interId);
                AdMobRequest();
                _interstitial.LoadAd(_requestAdmob);
            }*/
        }

        /// <summary>
        /// Show reward video <see cref="_rewardVideo"/> if ads available for watch.
        /// </summary>
        public void ShowRewardBasedVideo()
        {
            if (IsHasKeyNoAds())
                return;

            /*if (_rewardVideo.IsLoaded())
            {
                _rewardVideo.Show();

                var adUnitId = _isTestADS && !string.IsNullOrEmpty(_rewardId) ? _testRewardId : _rewardId;
                _rewardVideo = new RewardedAd(adUnitId);

                AdMobRequest();
                _rewardVideo.LoadAd(_requestAdmob);
            }*/
        }

        /// <summary>
        /// This method hide Smart banner from bottom of screen.
        /// </summary>
        public void HideBanner()
        {
            /*if (_bannerView != null)
                _bannerView.Hide();*/
        }
        /// <summary>
        /// Show reward video. If user watch it the ads will disappear for current game session.
        /// </summary>
        public void NoAdsAction()
        {
            /*_lastShowingType = RewardAdsType.NoAds;
            StartCoroutine(LoadRewardedVideo(_rewardVideo, _lastShowingType));*/
        }

        /// <summary>
        /// Show reward video. If user watch it the free undo tries will be add for user.
        /// </summary>
        public void ShowGetUndoAction()
        {
            /*_lastShowingType = RewardAdsType.GetUndo;
            StartCoroutine(LoadRewardedVideo(_rewardVideo, _lastShowingType));*/
        }

//        private IEnumerator LoadRewardedVideo(RewardedAd ads, RewardAdsType type)
//        {
//            _lastRewardVideoStatus = RewardVideoStatus.None;

//            ads.OnAdFailedToLoad += RewardedVideoFailedToLoad;
//            ads.OnAdLoaded += RewardedVideoLoaded;

//            if (_isHandeldAction)
//            {
//#if UNITY_IPHONE
//                Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
//#elif UNITY_ANDROID
//                Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
//#endif 
//                Handheld.StartActivityIndicator();
//            }

//            yield return new WaitUntil(() => _lastRewardVideoStatus == RewardVideoStatus.Loaded
//                                          || _lastRewardVideoStatus == RewardVideoStatus.FailedToLoad
//                                          || _isTestADS
//                                          || ads.IsLoaded()
//                                          //Used for old Admob sdk Before version 6.1.2
//                                          /* || Application.isEditor*/);
//            if (_isHandeldAction)
//            {
//                Handheld.StopActivityIndicator();
//            }

//            _lastRewardVideoStatus = ads.IsLoaded() || _isTestADS ? _lastRewardVideoStatus = RewardVideoStatus.Loaded : _lastRewardVideoStatus;

//            ads.OnAdFailedToLoad -= RewardedVideoFailedToLoad;
//            ads.OnAdLoaded -= RewardedVideoLoaded;
            
//            //Used for old Admob sdk Before version 6.1.2
//            // if (Application.isEditor)
//            // {
//            //     OnRewardedUser();
//            //     yield break;
//            // }

//            _isRewarded = false;

//            switch (_lastRewardVideoStatus)
//            {
//                case RewardVideoStatus.None:
//                case RewardVideoStatus.FailedToLoad:
//                        RewardAction?.Invoke(RewardAdsState.DID_NOT_LOADED, type);
//                    break;
//                case RewardVideoStatus.Loaded:
//                    ads.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
//                    ads.OnAdClosed += HandleClosedBasedVideoRewarded;
//                    ads.Show();
//                    break;
//            }
//        }   
        #endregion

        #region EventsHandlers

        /// <summary>
        /// On reward loaded video event.
        /// </summary>
        private void RewardedVideoLoaded(object sender, EventArgs e)
        {
            /*Debug.Log("RewardedVideoLoaded");
            _lastRewardVideoStatus = RewardVideoStatus.Loaded;*/
        }

        /// <summary>
        /// On reward failed load video event.
        /// </summary>
        /*private void RewardedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("RewardedVideoFailedToLoad " + e.LoadAdError);
            _lastRewardVideoStatus = RewardVideoStatus.FailedToLoad;
        }*/

        /// <summary>
        /// On close reward video event.
        /// </summary>
        private void HandleClosedBasedVideoRewarded(object sender, EventArgs eventArgs)
        {
            /*Debug.LogError($"HandleRewardBasedVideoRewarded {_isRewarded}");

            if (_isRewarded)
            {
                return;
            }

            switch (_lastShowingType)
            {
                case RewardAdsType.NoAds:
                    _rewardVideo.OnAdClosed -= HandleClosedBasedVideoRewarded;
                    RequestRewardBasedVideo();
                    break;
                case RewardAdsType.GetUndo:
                    _rewardVideo.OnAdClosed -= HandleClosedBasedVideoRewarded;
                    RequestRewardBasedVideo(true);
                    break;
            }

                RewardAction?.Invoke(RewardAdsState.TOO_EARLY_CLOSE, _lastShowingType);*/
        }

        /// <summary>
        /// On full watch reward video event.
        /// </summary>
        /*public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            _isRewarded = true;

            Debug.LogError($"HandleRewardBasedVideoRewarded {_isRewarded}");

            OnRewardedUser();
        }*/
        /*
        /// <summary>
        /// Reward actions by type of reward ads.
        /// </summary>
        public void OnRewardedUser()
        {
            switch (_lastShowingType)
            {
                case RewardAdsType.NoAds:
                    PlayerPrefs.SetInt(NoAdsKey, 1);
                    HideBanner();
                    _gameManagerComponent.OnNoAdsRewardedUser();
                    RequestRewardBasedVideo(true);
                    break;
                case RewardAdsType.GetUndo:
                    _gameManagerComponent.OnClickAdsCloseBtn();
                    _undoPerformerComponent.UpdateUndoCounts();
                    RequestRewardBasedVideo(true);
                    break;
            }

            //if (_rewardVideo != null)
            //{
            //    _rewardVideo.OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
            //    _rewardVideo.OnAdClosed -= HandleClosedBasedVideoRewarded;
            //}/
        }*/
        #endregion

        /// <summary>
        /// Initialize all active advertisment.
        /// </summary>
        public void InitializeADS()
        {
            /*if (IsHasKeyNoAds())
                PlayerPrefs.DeleteKey(NoAdsKey);

            //Uncomment for old versions of admob sdk
            //MobileAds.Initialize(initStatus => { Debug.Log("Sdk init status:" + initStatus); });
            var ids = Application.platform == RuntimePlatform.Android ? _androidIds : Application.platform == RuntimePlatform.IPhonePlayer ? _iosIds : null;

            if (ids != null)
            {
                _interId = ids.InterId;
                _rewardId = ids.RewardId;
                _bannerId = ids.BannerId;
            }

            if (_isBanner)
            {
                ShowBanner();
                _gameManagerComponent.InitializeBottomPanel(_currentBannerSize.Height * GetAdmobBannerScaleBasedOnDPI());
            }*/

            if (_isReward)
            {
                RequestRewardBasedVideo(true);
            }

            if (_isIntersitial)
            {
                RequestInterstitial();
                if (_intersitialRepeatCall)
                {
                    // First call after _firstCallIntersitialTime seconds. Repeating intersitial video every _repeatIntersitialTime seconds.
                    InvokeRepeating("ShowInterstitial", _firstCallIntersitialTime, _repeatIntersitialTime);
                }
            }
        }

        /// <summary>
        /// Check for exist in player prefs key <see cref="NoAdsKey"/>
        /// </summary>
        /// <returns></returns>
        private bool IsHasKeyNoAds()
        {
            return PlayerPrefs.HasKey(NoAdsKey);
        }

        private float GetAdmobBannerScaleBasedOnDPI()
        {
            //By default banner has no scaling.
            float scale = 1f;

            //All information about scaling has provided on Google Admob API
            //Low Density Screens, around 120 DPI, scaling factor 0.75, e.g. 320×50 becomes 240×37.
            //Medium Density Screens, around 160 DPI, no scaling, e.g. 320×50 stays at 320×50.
            //High Density Screens, around 240 DPI, scaling factor 1.5, e.g. 320×50 becomes 480×75.
            //Extra High Density Screens, around 320 DPI, scaling factor 2, e.g. 320×50 becomes 640×100.
            //Extra Extra High Density Screens, around 480 DPI, scaling factor 3, e.g. 320×50 becomes 960×150.

            if (UnityEngine.Screen.dpi > 480)
            {
                scale = 3f;
            }
            else if (UnityEngine.Screen.dpi > 320)
            {
                scale = 2f;
            }
            else if (UnityEngine.Screen.dpi > 240)
            {
                scale = 1.5f;
            }
            else if (UnityEngine.Screen.dpi > 160)
            {
                scale = 1f;
            }
            else if (UnityEngine.Screen.dpi > 120)
            {
                scale = 0.75f;
            }

            return scale;
        }
    }
}