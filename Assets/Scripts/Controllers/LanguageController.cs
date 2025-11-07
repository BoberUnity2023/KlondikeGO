using UnityEngine;
using System;
using BloomLines.UI;
using BloomLines.Managers;

#if Yandex
using YG;
#endif

#if GAME_PUSH
using GamePush;
#endif

namespace BloomLines.Controllers
{
    public static class LanguageManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void Initialize()
        {
#if UNITY_EDITOR
            OnChangeLanguage("ru");
#endif

#if Yandex
            YG2.onSwitchLang += OnChangeLanguage;
            OnChangeLanguage(YG2.envir.language);
#endif

#if GAME_PUSH
            await GP_Init.Ready;
            GP_Language.OnChangeLanguage += GPOnChangeLanguage;
            OnChangeLanguage(GP_Language.CurrentISO());
#endif
        }

#if GAME_PUSH
        private static void GPOnChangeLanguage(Language language)
        {
            OnChangeLanguage(GP_Language.CurrentISO());
        }
#endif

        private static void OnChangeLanguage(string lang)
        {
            Debug.Log("Language: " + lang);
            I2.Loc.LocalizationManager.CurrentLanguageCode = lang;

            var gameState = SaveManager.GameState;
            if (gameState == null)
                return;

            if (Enum.TryParse(typeof(LanguageCode), lang.ToUpperInvariant(), true, out object result))
            {
                gameState.LanguageCode = (LanguageCode)result;
            }
        }
    }
}
