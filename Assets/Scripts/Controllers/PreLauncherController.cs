using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

#if GAME_PUSH
using GamePush;
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
        }

        private async void Start()
        {
            Debug.Log("GamePush initing...");
            //var audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            //audioMixer.SetFloat("MusicVolume", -80f);
            //audioMixer.SetFloat("SoundVolume", -80f);
            AnalyticsController.SendEvent("applicaton_start");
            Debug.Log("GamePush initing...(1)");

#if GAME_PUSH
            await GP_Init.Ready;
            Debug.Log("GamePush initing...(2)");
            AnalyticsController.SendEvent("applicaton_gp_inited");
            Debug.Log("GamePush inited success");
#endif

#if CRAZY_GAMES
            Debug.Log("PreLauncherController.Initialize()");
            CrazyGamesController.Initialize();
            CrazyGamesController.OnInit += OnPluginInited;
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
            CrazyGamesController.OnInit -= OnPluginInited;
#endif
        }

        private void LoadGame()
        {
            Debug.Log("LoadGame()");
            SceneManager.LoadScene("1_Launcher");
        }

        private void OnPluginInited()//Crazy
        {
#if CRAZY_GAMES
            Debug.Log("PreLauncherController.OnPluginInited()");
            AnalyticsController.SendEvent("applicaton_inited");
            LoadGame();
#endif
        }        
    }    
}