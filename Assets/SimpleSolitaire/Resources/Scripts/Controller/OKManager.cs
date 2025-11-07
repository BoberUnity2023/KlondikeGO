using SimpleSolitaire.Screen;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OKManager : MonoBehaviour
{
    public const string AppId = "512002355669";
    public const string AppKey = "CGNCPILGDIHBABABA";

    public UnityEvent IOSInitialized = new UnityEvent();
    [SerializeField] private ScreenController _screenController;
    [SerializeField] private RewardedVideoController _rewardedVideoController;

    public struct InitOptions
    {
        public string app_id;
        public string app_key;
    }

    [Serializable]
    public struct GetPageInfoStruct
    {
        public string method;
        public string result;
        
        public static GetPageInfoStruct CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<GetPageInfoStruct>(jsonString);
        }
    }

    [Serializable]
    public struct GetPageInfoHeightStruct
    {
        public int innerHeight;
        
        public static GetPageInfoHeightStruct CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<GetPageInfoHeightStruct>(jsonString);
        }
    }
    
    private void Awake()
    {
        Debug.Log("v.: " + Application.version);
        Init();
        LoadAd();
        //GetPageInfoCallback(GetPageInfoCallbackFunc);
        GetPageInfo();
        StartCoroutine(nameof(UpdatingInformationAboutPage));
    }

    private IEnumerator UpdatingInformationAboutPage()
    {
        while (true)
        {
            GetPageInfo();
            yield return new WaitForSeconds(3f);
        }
    }

    public static void Init()
    {
        Application.ExternalEval(
            "FAPI.init();"
        );
    }

    public static void LoadAd()
    {
        Application.ExternalEval(
            "FAPI.UI.loadAd();"
        );
    }

    public static void ShowLoadedAd()
    {
        LoadAd();
        Application.ExternalEval(
            "FAPI.UI.showLoadedAd();"
        );
    }
    
    public static void ShowInterstitial()
    {
        Application.ExternalEval(
            "FAPI.UI.showAd();"
        );
    }
    
    public static void SetWindowSize(int width, int height)
    {
        Application.ExternalEval(
            $"FAPI.UI.setWindowSize({width}, {height});"
        );
    }
    
    public static void GetPageInfo()
    {
         Application.ExternalEval(
            $"FAPI.UI.getPageInfo();"
        );
    }

    public void ApiCallback(string arg1)
    {        
        if (string.IsNullOrEmpty(arg1)) return;

        Debug.LogWarning("API OK: " + arg1);

        if (arg1.Contains("showLoadedAd"))
            Debug.Log("API OK: " + arg1);

        if (arg1.Contains("showLoadedAd") &&
            arg1.Contains("ok") &&
            arg1.Contains("complete"))        
        {            
            _rewardedVideoController.Reward();
            return;
        }        
        
        try
        {
            var pageInfo = GetPageInfoStruct.CreateFromJSON(arg1);            

            if (pageInfo.method == "getPageInfo")
            {
                if (pageInfo.result == "ok")
                {
                    var innerHeightValue = arg1.Split("innerHeight")[1];
                    var innerHeightValueTrim = innerHeightValue.Split(":")[1];
                    var innerHeightValueTrim2 = innerHeightValue.Split(",")[0];
                    var parseInt = int.Parse(innerHeightValueTrim2.Substring(2)) - 48;
                    SetWindowSize(0, parseInt);                    
                    _screenController.SetFapiWindowHeight(parseInt);
                }

                //int innerHeight = GetPageInfoHeightStruct.CreateFromJSON(parseData.Substring(1, parseData.Length - 1))
                //   .innerHeight;
            }            
        }
        catch (Exception e)
        {
            //Debug.LogWarning("FAPI ApiCallback Catch: " + e);
            // ignored
        }
    }

    public void TargetPlatform(string arg1)
    {
        if (string.IsNullOrEmpty(arg1)) return;

        if (arg1 == "ios" | arg1 == "iosweb")
        {
            IOSInitialized?.Invoke();
        }
    }
}