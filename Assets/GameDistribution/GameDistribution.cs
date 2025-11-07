using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using BloomLines.Assets;

public class GameDistribution : MonoBehaviour
{    
    public static GameDistribution Instance;

    public string GAME_KEY = "886bf84505244ee182cc7af59d4b9ca8";

    public static Action<string> OnEvent;
    public static Action OnResumeGame;
    public static Action OnPauseGame;
    public static Action OnRewardGame;
    public static Action OnRewardedVideoSuccess;
    public static Action OnRewardedVideoFailure;
    public static Action<int> OnPreloadRewardedVideo;
#if GD
    [DllImport("__Internal")]
    private static extern void SDK_Init(string gameKey);

    [DllImport("__Internal")]
    private static extern void SDK_PreloadAd();

    [DllImport("__Internal")]
    private static extern void SDK_ShowAd(string adType);
    [DllImport("__Internal")]
    private static extern void SDK_SendEvent(string options);
#endif
    private bool _isRewardedVideoLoaded = false;

    private void Awake()
    {
        BuildData buildData = Resources.Load<BuildData>("BuildData");
        if (buildData == null)
            Debug.LogError("buildData == null");

        if (buildData.BuildPlatform != BuildPlatform.GD)
        {
            Destroy(gameObject);
        }
#if GD
        if (GameDistribution.Instance == null)
            GameDistribution.Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);

        Init();
#endif
    }

    void Init()
    {
        try 
        { 
#if GD
            Debug.Log("GD SDK Init");
            SDK_Init(GAME_KEY);
            StartCoroutine(AfterInit());
#endif
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD initialization failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }
    internal void ShowAd()
    {
        try
        {
#if GD
            SDK_ShowAd(null);
#endif
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void ShowRewardedAd()
    {
        try
        {
#if GD
            SDK_ShowAd("rewarded");
#endif
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void PreloadRewardedAd()
    {
        try
        {
#if GD
            SDK_PreloadAd();
#endif
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD Preload failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void SendEvent(string options)
    {
        try
        {
#if GD
            SDK_SendEvent(options);
#endif
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("GD SendEvent failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    private IEnumerator AfterInit()
    {
        yield return new WaitForSeconds(3);
        PreloadRewardedAd();
    }
    /// <summary>
    /// It is being called by HTML5 SDK when the game should start.
    /// </summary>
    void ResumeGameCallback()
    {
        if (OnResumeGame != null) OnResumeGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the game should pause.
    /// </summary>
    void PauseGameCallback()
    {
        if (OnPauseGame != null) OnPauseGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the game should should give reward.
    /// </summary>
    void RewardedCompleteCallback()
    {
        if (OnRewardGame != null) OnRewardGame();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the rewarded video succeeded.
    /// </summary>
    void RewardedVideoSuccessCallback()
    {
        _isRewardedVideoLoaded = false;

        if (OnRewardedVideoSuccess != null) OnRewardedVideoSuccess();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when the rewarded video failed.
    /// </summary>
    void RewardedVideoFailureCallback()
    {
        _isRewardedVideoLoaded = false;

        if (OnRewardedVideoFailure != null) OnRewardedVideoFailure();
    }

    /// <summary>
    /// It is being called by HTML5 SDK when it preloaded rewarded video
    /// </summary>
    void PreloadRewardedVideoCallback(int loaded)
    {
        _isRewardedVideoLoaded = (loaded == 1);

        if (OnPreloadRewardedVideo != null) OnPreloadRewardedVideo(loaded);
    }

    /// <summary>
    /// It is being called by HTML5 SDK when it any event triggered
    /// </summary>
    void EventCallback(string eventData)
    {
        if (OnEvent != null) OnEvent(eventData);
    }

    public bool IsRewardedVideoLoaded()
    {
        return _isRewardedVideoLoaded;
    }
}