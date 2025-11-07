#if Yandex
using System;
using UnityEngine;
using YG;

namespace BloomLines.Ads
{
    // –екламный адаптер под яндекс
    public class YandexAdapter : IAdsAdapter
    {
        private Action<bool> _onComplete;

        public void Initialize()
        {
            YG2.onRewardAdv += OnReward;
            YG2.onErrorRewardedAdv += OnFail;
        }

        public void ShowInterstitial()
        {
            Debug.Log("YandexAdsAdapter: ShowAd");

            YG2.InterstitialAdvShow();
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            Debug.Log("YandexAdsAdapter: ShowRewarded");

            _onComplete = onComplete;
            YG2.RewardedAdvShow(string.Empty);
        }

        private void OnReward(string id)
        {
            _onComplete?.Invoke(true);
            _onComplete = null;
        }

        private void OnFail()
        {
            _onComplete?.Invoke(false);
            _onComplete = null;
        }

        public void CloseSticky()
        {
            YG2.StickyAdActivity(false);
        }
    }
}
#endif