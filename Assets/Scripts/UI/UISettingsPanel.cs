using BloomLines.Controllers;
using BloomLines.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if Yandex
using YG;
#endif

namespace BloomLines.UI
{
    public class UISettingsPanel : UIPanelBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _languageButton;
        [SerializeField] private Button _ourGamesButton;
        [SerializeField] private TMP_Text _appVersionText;

        private GameModeController _gameModeController;

        protected override void Awake()
        {
            base.Awake();

            _gameModeController = FindAnyObjectByType<GameModeController>();

            _closeButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                Close();
            });

            _backgroundButton.onClick.AddListener(Close);

            _restartButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_restart_button");

                var gameModeState = SaveManager.GameModeState;
                _gameModeController.StartGame(gameModeState.Type, true);
                Close();
            });

            _homeButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_home_button");
                OpenAnotherPanel(UIPanelType.GamemodeChoice);
            });

            _shopButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_shop_button");
                OpenAnotherPanel(UIPanelType.Shop);
            });

            _soundButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_sound_button");
                OpenAnotherPanel(UIPanelType.Sounds);
            });

            _languageButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_language_button");
                OpenAnotherPanel(UIPanelType.SelectLanguage);
            });

            _ourGamesButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_our_games_button");

#if Yandex
                YG2.OnDeveloperURL();
#endif

#if VK
                Application.OpenURL("https://vk.com/dakagames");
#endif

#if OK
                Application.OpenURL("https://ok.ru/group/70000039166661");
#endif

#if RuStore
                Application.OpenURL("https://www.rustore.ru/catalog/developer/up04w6gq");
#endif
            });

#if RuStore || CRAZY_GAMES || GD || Poki
            _shopButton.gameObject.SetActive(false);
#endif

#if CRAZY_GAMES || GD || Poki
            _ourGamesButton.gameObject.SetActive(false);
#endif
            _appVersionText.text = "v:" + Application.version;
        }

        private void OpenAnotherPanel(UIPanelType type)
        {
            EventsManager.Publish(new ClosePanelEvent(UIPanelType.Settings));
            EventsManager.Publish(new OpenPanelEvent(type));
        }

        protected override void Open()
        {
            base.Open();
#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStop();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStop();
#endif
        }

        protected override void Close()
        {
            base.Close();
#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStart();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStart();
#endif
        }
    }
}