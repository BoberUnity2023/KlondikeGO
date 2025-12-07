using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class SpriteLocalize : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _spriteRus;
        [SerializeField] private Sprite _spriteEng;
        
        void Start()
        {
#if Yandex && !UNITY_EDITOR
        YG2.onSwitchLang += OnChangeLanguage;
        OnChangeLanguage(YG2.envir.language);
#endif

#if GAME_PUSH            
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
        YG2.onSwitchLang -= OnChangeLanguage;        
#endif

#if GAME_PUSH
            GamePush.GP_Language.OnChangeLanguage -= GPOnChangeLanguage;
#endif
        }

        private void OnChangeLanguage(string lang)
        {
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
