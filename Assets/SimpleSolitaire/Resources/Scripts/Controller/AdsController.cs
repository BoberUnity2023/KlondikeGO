using SimpleSolitaire.Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdsController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    //[SerializeField] private VKManager _vkManager;
    [SerializeField] private float _interval;
    private float _lastAdsTime;

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
}
