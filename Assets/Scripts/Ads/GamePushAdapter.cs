#if GAME_PUSH
using System;
using System.Collections;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using GamePush;
using UnityEngine;

namespace BloomLines.Ads
{
    // Рекламный адаптер под GamePush
    public class GamePushAdapter : IAdsAdapter
    {
        private Action<bool> _onComplete;

        public void Initialize()
        {
            var updateHelper = UpdateHelper.Get();                     
        }        

        public void ShowInterstitial()
        {
            Debug.Log("GamePushAdsAdapter: ShowAd");

            GP_Ads.ShowFullscreen();
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            Debug.Log("GamePushAdsAdapter: ShowRewarded");

            _onComplete = onComplete;
            GP_Ads.ShowRewarded(string.Empty, null, null, OnReward);
        }

        private void OnReward(bool success)
        {
            _onComplete?.Invoke(success);
            _onComplete = null;
        }

        public void CloseSticky()
        {
            GP_Ads.CloseSticky();
        }
    }
}
#endif