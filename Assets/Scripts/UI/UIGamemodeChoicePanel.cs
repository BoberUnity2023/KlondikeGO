using BloomLines.Controllers;
using BloomLines.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UIGamemodeChoicePanel : UIPanelBase
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private UIGamemodeTab[] _tabs;
        [SerializeField] Sprite[] _saveButtonSprites;

        private GameModeType _selectedGameModeType;
        private GameModeController _gameModeController;

        protected override void Awake()
        {
            base.Awake();

            _gameModeController = FindAnyObjectByType<GameModeController>();

            _saveButton.onClick.AddListener(CloseWithSave);
            _closeButton.onClick.AddListener(() =>
            {
                CloseWithoutSave();
                AudioController.Play("click_button");
            });
            _backgroundButton.onClick.AddListener(CloseWithoutSave);

            foreach (var tab in _tabs)
                tab.Initialize((gameMode) =>
                {
                    OnSelect(gameMode);

                    AudioController.Play("click_button");
                });

        }

        private void OnSelect(GameModeType type)
        {
            foreach(var tab in _tabs)
                tab.Toggle.SetIsOnWithoutNotify(type == tab.GameModeType);

            _selectedGameModeType = type;

            _saveButton.image.sprite = _saveButtonSprites[type == GameModeType.Classic ? 0 : 1];
            AnalyticsController.SendEvent(type == GameModeType.Classic ? "select_classic" : "select_adventure");
        }

        private void CloseWithoutSave()
        {
            Close();
            ResetValue();
        }

        private void CloseWithSave()
        {
            Close();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState == null)
                _gameModeController.StartGame(_selectedGameModeType, true);
            else
                _gameModeController.StartGame(_selectedGameModeType, gameModeState.Type != _selectedGameModeType);
            
            AudioController.Play("click_button");
            AnalyticsController.SendEvent("click_gamemode_choice_save_button");
        }

        private void ResetValue()
        {
            var gameModeState = SaveManager.GameModeState;
            OnSelect(gameModeState != null ? gameModeState.Type : GameModeType.None);
        }

        protected override void Open()
        {
            base.Open();

            ResetValue();

            var gameModeState = SaveManager.GameModeState;
            _closeButton.gameObject.SetActive(gameModeState != null);
            _backgroundButton.interactable = gameModeState != null;
            AnalyticsController.SendEvent("gamemode_open");

            if (gameModeState == null)
                OnSelect(GameModeType.Adventure);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState == null)
                Open();
        }
    }
}