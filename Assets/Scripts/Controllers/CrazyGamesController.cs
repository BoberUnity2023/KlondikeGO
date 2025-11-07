using UnityEngine;
using BloomLines.Helpers;
using System;

#if CRAZY_GAMES
using CrazyGames;
#endif

namespace BloomLines.Controllers
{
    [System.Serializable]
    public class CrazyGamesStorage
    {
        public string GameStateJson;
        public string GameModeStateJson;
    }

    public static class CrazyGamesController
    {
        public static CrazyGamesStorage Storage { get; private set; }
        public static bool SaveLoaded { get; private set; }

        public const string StorageKey = "save";

        public static event Action OnInit;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
#if CRAZY_GAMES
            Debug.Log("CrazySDK.Initializing...");
            if (CrazySDK.IsAvailable)
            {
                if (CrazySDK.IsInitialized)
                {
                    Debug.Log("CrazySDK.IsInitialized yet");
                }
                else
                {
                    CrazySDK.Init(() =>
                    {
                        OnInited();
                    });
                }                
            }
            else
            {
                Debug.Log("CrazySDK.IsAvailable: False");
            }
#endif
        }

        private static void OnInited()
        {
#if CRAZY_GAMES
            Debug.Log("CrazySDK initialized success");
            string text =
            $"SDK Version: {CrazySDK.Version} \n" +
            $"SDK Initialized: {CrazySDK.IsInitialized} \n" +
            $"Is QA Tool: {CrazySDK.IsQaTool} \n" +
            $"Is user account available: {CrazySDK.User.IsUserAccountAvailable} \n" +
            $"Environment: {CrazySDK.Environment} \n" +
            $"Is SDK Available: {CrazySDK.IsAvailable} \n";
            Debug.Log("Info:" + text);

            //_gameManager.Gold = CrazySDK.Data.GetInt("Gold");

            CrazySDK.User.GetUser(
                (
                    user =>
                    {
                        Debug.Log(("Got user" + user));
                    }
                )
            );
            CrazySDK.Ad.HasAdblock(
                (adblockPresent) =>
                {
                    Debug.Log("Has adblock: " + adblockPresent);
                }
            );

            OnInit?.Invoke();
#endif
        }
    }
}