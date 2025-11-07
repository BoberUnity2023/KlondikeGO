using System;

namespace BloomLines.Ads
{
    // Адаптер под разную рекламную логику
    public interface IAdsAdapter
    {
        void Initialize();
        void ShowInterstitial();
        void ShowRewarded(Action<bool> onComplete);
        void CloseSticky();
    }
}