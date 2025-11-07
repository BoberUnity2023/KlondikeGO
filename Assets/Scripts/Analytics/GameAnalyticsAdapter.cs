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
                gameKey = "970617db8eacf09170567b1c180c4968";
                secretKey = "365cc1792ee9d0c0a3c3664350abf42916cbd519";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if VK
                gameKey = "cd9def4da21bff28a13db415c771506e";
                secretKey = "54cd0a30db50a64b3b87fdb187ed64247d45e1ca";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if OK
                gameKey = "553999ddf52825606c2456b08239e7b0";
                secretKey = "e4a3fa580510579bc6692e669093f59ab79e3db6";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if CRAZY_GAMES
                gameKey = "f1dbfcc40612f142a2383d8ccbda4aaa";
                secretKey = "54e22532e1fd76c9f8fca8ad2f7530c25a278724";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if GD
                gameKey = "cc5c7b2e855080d1c0567d975b41e125";
                secretKey = "88ce1f0af064cf19c769d2ef0fba973d2b12127a";
                platform = RuntimePlatform.WebGLPlayer;
#endif

#if Poki
                gameKey = "952a54409a2b405582914d9ea0a5dfaf";
                secretKey = "ea48178f126dab6df68793dddea7705e6b3c210f";
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