using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UISelectLanguagePanel : UIPanelBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private UILanguageTab[] _languageTabs;

        private LanguageCode _selectedLanguageCode;

        protected override void Awake()
        {
            base.Awake();

            _saveButton.onClick.AddListener(CloseWithSave);
            _closeButton.onClick.AddListener(() =>
            {
                CloseWithoutSave();
                AudioController.Play("click_button");
            });
            _backgroundButton.onClick.AddListener(CloseWithoutSave);

            foreach (var tab in _languageTabs)
                tab.Initialize((code) => 
                {
                    OnSelectLanguage(code);

                    AudioController.Play("click_button");
                    AnalyticsController.SendEvent($"select_language_{code.ToString().ToLowerInvariant()}");
                });
        }

        private void OnSelectLanguage(LanguageCode code)
        {
            foreach (var t in _languageTabs)
                t.Toggle.SetIsOnWithoutNotify(t.LanguageCode == code);

            _selectedLanguageCode = code;
            LocalizationManager.CurrentLanguageCode = code.ToString();
        }

        private void CloseWithSave()
        {
            Close();

            var gameState = SaveManager.GameState;
            gameState.LanguageCode = _selectedLanguageCode;

            SaveManager.Save(SaveType.Game);

            AudioController.Play("click_button");
            AnalyticsController.SendEvent("click_language_save_button");
        }

        private void CloseWithoutSave()
        {
            Close();
            ResetValue();
        }

        private void ResetValue()
        {
            var gameState = SaveManager.GameState;
            OnSelectLanguage(gameState.LanguageCode);

            _selectedLanguageCode = gameState.LanguageCode;
        }

        protected override void Open()
        {
            base.Open();

            ResetValue();
        }
    }
}