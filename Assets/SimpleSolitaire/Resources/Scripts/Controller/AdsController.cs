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
    }

    public void TryShowInterstitial()
    {
        if (IsReady)
        {            
            _lastAdsTime = Time.time;
            ShowInterstitial();
        }        
    }

    private void ShowInterstitial()
    { 
        if (_gameManager.Platform == Platform.Yandex)
        {
            //_yandexGame._FullscreenShow();
        }

        if (_gameManager.Platform == Platform.Ok)
        {
            OKManager.ShowInterstitial();
            //OKManager.LoadAd();
        }

        if (_gameManager.Platform == Platform.VK)
        {
            //VKManager.Instance.ShowInterstitial();
        }         
        
        if (_gameManager.Platform == Platform.GD)
        {
            GameDistribution.Instance.ShowAd();
        }
    }    
}
