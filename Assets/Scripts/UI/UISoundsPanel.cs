using BloomLines.Controllers;
using BloomLines.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UISoundsPanel : UIPanelBase
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private Toggle _vibrationToggle;
        [SerializeField] private Button _musicButton;
        [SerializeField] private Image _musicIcon;
        [SerializeField] private Sprite _musicActive;
        [SerializeField] private Sprite _musicUnactive;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Image _soundIcon;
        [SerializeField] private Sprite _soundActive;
        [SerializeField] private Sprite _soundUnactive;
        [SerializeField] private Sprite _buttonActiveIcon;
        [SerializeField] private Sprite _buttonUnactiveIcon;

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

            _vibrationToggle.onValueChanged.AddListener((value) =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent($"click_vibration_button");
            });

            _musicSlider.onValueChanged.AddListener(OnMusicValueChanged);
            _soundSlider.onValueChanged.AddListener(OnSoundValueChanged);

            _musicButton.onClick.AddListener(() =>
            {
                var value = _musicSlider.value <= 0f ? 1f : 0f;
                _musicSlider.SetValueWithoutNotify(value);
                OnMusicValueChanged(value);

                AudioController.Play("click_button");
                AnalyticsController.SendEvent($"click_music_button");
            });

            _soundButton.onClick.AddListener(() =>
            {
                var value = _soundSlider.value <= 0f ? 1f : 0f;
                _soundSlider.SetValueWithoutNotify(value);
                OnSoundValueChanged(value);

                AudioController.Play("click_button");
                AnalyticsController.SendEvent($"click_sound_button");
            });
        }

        private void CloseWithoutSave()
        {
            Close();
            ResetValue();
        }

        private void CloseWithSave()
        {
            Close();

            var gameState = SaveManager.GameState;
            gameState.MusicVolume = _musicSlider.value;
            gameState.SoundVolume = _soundSlider.value;
            gameState.Vibration = _vibrationToggle.isOn;

            SaveManager.Save(SaveType.Game);

            AudioController.Play("click_button");
            AnalyticsController.SendEvent($"click_sounds_save_button");
        }

        private void OnMusicValueChanged(float value)
        {
            _musicButton.image.sprite = value <= 0f ? _buttonUnactiveIcon : _buttonActiveIcon;
            _musicIcon.sprite = value <= 0f ? _musicUnactive : _musicActive;
            _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, value));
        }

        private void OnSoundValueChanged(float value)
        {
            _soundButton.image.sprite = value <= 0f ? _buttonUnactiveIcon : _buttonActiveIcon;
            _soundIcon.sprite = value <= 0f ? _soundUnactive : _soundActive;
            _audioMixer.SetFloat("SoundVolume", Mathf.Lerp(-80f, 0f, value));
        }

        private void ResetValue()
        {
            var gameState = SaveManager.GameState;

            _musicSlider.SetValueWithoutNotify(gameState.MusicVolume);
            OnMusicValueChanged(gameState.MusicVolume);

            _soundSlider.SetValueWithoutNotify(gameState.SoundVolume);
            OnSoundValueChanged(gameState.SoundVolume);

            _vibrationToggle.SetIsOnWithoutNotify(gameState.Vibration);
        }

        protected override void Open()
        {
            base.Open();

            ResetValue();
        }
    }
}