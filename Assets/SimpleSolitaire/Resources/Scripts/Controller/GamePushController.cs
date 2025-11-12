#if GAME_PUSH
using GamePush;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSolitaire.Controller;

public class GamePushController : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
#if GAME_PUSH
        Debug.Log("GamePush Awake()");
        GamePushController[] _gPCs = FindObjectsOfType<GamePushController>();

        GP_Init[] _GP_Inits = FindObjectsOfType<GP_Init>();

        if (_GP_Inits.Length > 1)
            Debug.LogError("_GP_Inits.Length > 1");

        if (_gPCs.Length > 1)
            Destroy(gameObject);

        if (_gameManager == null)
            _gameManager = FindObjectOfType<GameManager>();

        _gameManager.GamePush = this;
        DontDestroyOnLoad(gameObject);
#endif
    }

    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
#if GAME_PUSH
        GP_Init.OnReady -= OnPluginReady;
        GP_Leaderboard.OnFetchSuccess -= OnFetchLeaderboardSuccess;
        GP_Leaderboard.OnFetchPlayerRatingSuccess -= OnFetchPlayerRatingSuccess;
        //GP_Ads.OnAdsStart -= OnAdsStart;
        //GP_Ads.OnAdsClose -= OnAdsClose;
#endif
    }

    private /*async*/ void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("GamePush starting...");

# if UNITY_EDITOR
        OnPluginReady();
#endif

#if GAME_PUSH
        //await GP_Init.Ready;

        if (GP_Init.isReady)
            Debug.Log("GamePush isReady!");
        else
            Debug.Log("GamePush failed");

        GP_Init.OnReady += OnPluginReady;
        GP_Leaderboard.OnFetchSuccess += OnFetchLeaderboardSuccess;
        GP_Leaderboard.OnFetchPlayerRatingSuccess += OnFetchPlayerRatingSuccess;

        OnPluginReady();
#endif
    }

    private void OnPluginReady()
    {
        Init();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (GP_Init.isReady)
        //    Init();
    }

    private void Init()
    {        
        /*
        if (_gameManager.Platform == Platform.VK||
            _gameManager.Platform == Platform.Ok)
        {
            _gameManager.Saves.OnGamePushInit();
            _gameManager.UI.WindowMainMenu.OnGamePushInit();            
        }*/
    }

    //LB
    public void FetchLeaderboard()
    {
#if GAME_PUSH
        Debug.Log("GamePush. Fetch Leaderboard...");
        GP_Leaderboard.Fetch("score", "score", Order.DESC, 8, 4, WithMe.none, "");
#endif
    }

    public void FetchPlayerRating()
    {
#if GAME_PUSH
        Debug.Log("GamePush. Fetch Player Rating...");
        GP_Leaderboard.FetchPlayerRating("score", "score", Order.DESC);
#endif
    }
#if GAME_PUSH
    private void OnFetchLeaderboardSuccess(string fetchTag, GP_Data data)
    {
        Debug.Log("GamePush. Fetch Leaderboard success");
        _gameManager.TabLeaderboard.OnFetchSuccess(fetchTag, data);
    }


    private void OnFetchPlayerRatingSuccess(string fetchTag, int position)
    {
        Debug.Log("GamePush. Fetch Player Rating success");
        _gameManager.TabLeaderboard.OnFetchPlayerRatingSuccess(fetchTag, position);
    }
#endif

    /*private void OnAdsStart()//GamePush
    {
        _hub.Sound.Lock();
        _hub.RewardedVideo.OpenVideoAd();
    }
    private void OnAdsClose(bool success)//GamePush
    {
        Debug.LogWarning("OnAdsClose(" + success + ")");
        _hub.Sound.Unlock();
        _hub.RewardedVideo.CloseVideoAd();
    }*/
}
