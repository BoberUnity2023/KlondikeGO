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
            Debug.Log("BloomLines v." + Application.version + " started success");
        }

        private async void Start()
        {
            var audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            audioMixer.SetFloat("MusicVolume", -80f);
            audioMixer.SetFloat("SoundVolume", -80f);
            AnalyticsController.SendEvent("applicaton_start");            

#if GAME_PUSH
            await GP_Init.Ready;
            AnalyticsController.SendEvent("applicaton_gp_inited");
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
            SceneManager.LoadScene("Launcher");
        }

        private void OnPluginInited()
        {
#if CRAZY_GAMES
            Debug.Log("PreLauncherController.OnPluginInited()");
            AnalyticsController.SendEvent("applicaton_inited");
            LoadGame();
#endif
        }        
    }    
}