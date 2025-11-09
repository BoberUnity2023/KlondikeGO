using System.Linq;
using GameAnalyticsSDK;
using UnityEngine;

namespace BloomLines.Analytics
{
    // Адаптер аналитики под GameAnalytics
    public class GameAnalyticsAdapter : IAnalyticsAdapter, IGameAnalyticsATTListener
    {
        public void Initialize()
        {
            // Загружаем нужные ключи в зависимости от платформы под какую билдим
            {
                string gameKey = string.Empty;
                string secretKey = string.Empty;
                RuntimePlatform platform = RuntimePlatform.Android;

#if Yandex
                gameKey = "8b37f6f956c071cec054aaeb466b4482";
                secretKey = "0ee917e71aa5aec9433fc2e57a6cddda56ade9c0";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if VK
                gameKey = "ea65a1b6553a53cc1c88a997056be2e7";
                secretKey = "9960aa07a87489b7ba2c37ede4aca12dc09d9ff5";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if OK
                gameKey = "fe49849d07de4c6117ebe90edeb524c6";
                secretKey = "987d664e89c41c851ab4a8592172d638bd06cdc1";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if CRAZY_GAMES
                gameKey = "";
                secretKey = "";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if GD
                gameKey = "";
                secretKey = "";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if Poki
                gameKey = "";
                secretKey = "";
                platform = RuntimePlatform.WebGLPlayer;
#endif

                if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
                    GameAnalytics.SettingsGA.AddPlatform(platform);

                var item = GameAnalytics.SettingsGA.Platforms.FirstOrDefault(e => e == platform);
                int index = GameAnalytics.SettingsGA.Platforms.IndexOf(item);

                GameAnalytics.SettingsGA.UpdateGameKey(index, gameKey);
                GameAnalytics.SettingsGA.UpdateSecretKey(index, secretKey);
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
            }
        }

        public void SendEvent(string id)
        {
            GameAnalytics.NewDesignEvent(id);
        }

        #region GameAnalyticsCallbacks
        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }
        #endregion
    }
}