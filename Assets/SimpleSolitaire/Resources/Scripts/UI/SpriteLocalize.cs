using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class SpriteLocalize : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _spriteRus;
        [SerializeField] private Sprite _spriteEng;

        private void Start()
        {
#if Yandex
            if (!YG.YG2.isSDKEnabled)
                YG.YG2.StartInit();

            YG.YG2.onSwitchLang += OnChangeLanguage;
            OnChangeLanguage(YG.YG2.envir.language);
#endif

#if VK || OK
            OnChangeLanguage("ru");            
#endif

#if GAME_PUSH && !VK && !OK
            GamePush.GP_Language.OnChangeLanguage += GPOnChangeLanguage;
            OnChangeLanguage(GamePush.GP_Language.CurrentISO());
#endif

#if UNITY_EDITOR
            //OnChangeLanguage("en");
#endif
        }

        private void OnDestroy()
        {
#if Yandex && !UNITY_EDITOR
        YG.YG2.onSwitchLang -= OnChangeLanguage;        
#endif

#if GAME_PUSH && !VK && !OK
            GamePush.GP_Language.OnChangeLanguage -= GPOnChangeLanguage;
#endif
        }

        private void OnChangeLanguage(string lang)
        {
            Debug.Log(gameObject.name + ".OnChangeLanguage(" + lang + ")");
            _image.sprite = lang == "ru" ? _spriteRus : _spriteEng;
        }

#if GAME_PUSH
        private void GPOnChangeLanguage(GamePush.Language language)
        {
            OnChangeLanguage(GamePush.GP_Language.CurrentISO());
        }
#endif
    }
}
