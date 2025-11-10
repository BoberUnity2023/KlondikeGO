//using YG;
using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Controller;
#if GAME_PUSH
using GamePush;
#endif
using Platform = SimpleSolitaire.Controller.Platform;
using System.Security.Cryptography;

public class RewardedVideoController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioController _audioController;
    [SerializeField] private BuyWindow _buyWindow;
    [SerializeField] private UndoPerformer _undoPerformer;
    [SerializeField] private HintManager _hintManager;
    [SerializeField] private EveryDayBonus _everyDayBonus;    
    private int _reward = 300;    

    public int Id { get; set; }

    // Подписываемся на событие открытия рекламы в OnEnable
    //private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    // Отписываемся от события открытия рекламы в OnDisable
    //private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

    // Подписанный метод получения награды
    private void Start()
    {
        if (_gameManager.Platform == Platform.GD)
        {            
            GameDistribution.Instance.PreloadRewardedAd();
            GameDistribution.OnRewardedVideoSuccess += Reward;
            GameDistribution.OnRewardedVideoFailure += RewardError;
        }
    }

    private void OnDestroy()
    {
        if (_gameManager.Platform == Platform.GD)
        {            
            GameDistribution.OnRewardedVideoSuccess -= Reward;
            GameDistribution.OnRewardedVideoFailure -= RewardError;
        }
    }

    public void Rewarded(int id)
    {
        if (id == 0)
        {
            _gameManager.Gold += _reward;
            _buyWindow.OnRewardedShown();
            _audioController.Play(AudioController.AudioType.Buy);
        }

        if (id == 1)
        {
            _undoPerformer.UpdateUndoCounts();
            _gameManager.HideAdsLayer();
            _audioController.Play(AudioController.AudioType.Buy);
        }

        if (id == 2)
        {
            _hintManager.AvailableCountLevels += 3;
            _gameManager.HideAdsLayerHints();
            _audioController.Play(AudioController.AudioType.Buy);
        }

        if (id == 3)
        {
            _everyDayBonus.OnRewardGetX2();
            _audioController.Play(AudioController.AudioType.Buy);
        }
    }

    // Метод для вызова видео рекламы
    public void Reward()//OK
    {
        Rewarded(Id);
    }

    public void RewardError()//OK
    {
        
    }

    public void PressRewardedVideo(int id)
    {
        Debug.Log("PressRewardedVideo(" + id + ")");
        Id = id;
//        if (_gameManager.Platform == Platform.Ok)
//        {
//            //OKManager.ShowLoadedAd();
//#if GAME_PUSH
//            GP_Ads.ShowRewarded(id.ToString(), OnRewardedReward, OnRewardedStart, OnRewardedClose);
//#endif
//        }

        //if (_gameManager.Platform == Platform.VK)
        //{            
            BloomLines.Controllers.AdsController.ShowRewarded((success) =>
            {
                if (success)
                {
                    OnRewardedReward(id.ToString());
                    //var gameModeState = SaveManager.GameModeState;
                    //gameModeState.ContinueGameCount++;

                    //Close();

                    //EventsManager.Publish(new ReviveEvent());
                }
            });
        //}        

        //if (_gameManager.Platform == Platform.GD)
        //{
        //    GameDistribution.Instance.ShowRewardedAd();
        //    GameDistribution.Instance.PreloadRewardedAd();
        //}
    }

    public void OnRewardedReward(string id)
    {
        Debug.Log("OnRewardedReward: " + id);
        //_game.HasFocus = true;
        Rewarded(Id);
    }

    public void OnRewardedStart()//Editor YandexGame + GP
    {
        //_game.HasFocus = false;
        Debug.Log("GP.OnRewardedStart()");
    }

    public void OnRewardedClose(bool result)//Editor YandexGame + GP
    {
        Debug.Log("GP.OnRewardedClose(" + result + ")");
        //_game.HasFocus = true;
        //if (!result)
        //    RewardedError();
    }
}
