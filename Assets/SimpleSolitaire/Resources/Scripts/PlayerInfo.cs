using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Controller;
#if Yandex
using YG;
using YG.Utils.Lang;
#endif

#if GAME_PUSH
using GamePush;
#endif

namespace BloomLines
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager; 
        [SerializeField] private Text _title;
        [SerializeField] private Image _avatar;
        [SerializeField] private GameObject _yandexPlayerInfo;

        private void Start()
        {
#if GAME_PUSH && UNITY_WEBGL && !UNITY_EDITOR
            _yandexPlayerInfo.SetActive(false);
            _title.text = GP_Player.GetName();
            
            if (_gameManager.Device == Device.Desktop)
                GP_Player.GetAvatar(_avatar);
#endif

#if Yandex
            _yandexPlayerInfo.SetActive(true);
            _title.gameObject.SetActive(false);
            _avatar.gameObject.SetActive(false);
#endif
        }
    }
}
