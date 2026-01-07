using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

#if GAME_PUSH
using GamePush;
using System.Collections;
#endif

#if CRAZY_GAMES
using CrazyGames;
#endif

namespace BloomLines.Controllers
{
    public class PreLauncherController : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("KlondikeGO v." + Application.version + " started success");
#if Yandex
            if (YG.YG2.isSDKEnabled)
                Debug.Log("YG SDK Inited success!");
            else
            {
                Debug.Log("YG SDK Init starting...");
                YG.YG2.StartInit(); 
            }
#endif
        }

        private void Start()
        {
            AnalyticsController.SendEvent("applicaton_start");
#if CRAZY_GAMES
            Debug.Log("PreLauncherController.Initialize()");
            CrazyGamesController.Initialize();
            CrazyGamesController.OnInit += OnCrazyGamesPluginInited;
#endif

#if Poki
            PokiUnitySDK.Instance.init();
#endif

#if !CRAZY_GAMES
            LoadGame();
#endif

#if UNITY_EDITOR
            LoadGame();
#endif
        }

        private void OnDestroy()
        {
#if CRAZY_GAMES            
            CrazyGamesController.OnInit -= OnCrazyGamesPluginInited;
#endif
        }

        private void LoadGame()
        {
            Debug.Log("LoadGame()");
            SceneManager.LoadScene("1_Launcher");            
        }

        private void OnCrazyGamesPluginInited()//Crazy
        {
#if CRAZY_GAMES
            Debug.Log("PreLauncherController.OnCrazyGamesPluginInited()");
            AnalyticsController.SendEvent("applicaton_inited");
            LoadGame();
#endif
        }        
    }    
}