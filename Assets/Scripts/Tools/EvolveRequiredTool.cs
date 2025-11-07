using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Tools
{
    public abstract class EvolveRequiredTool : ToolBase
    {
        [SerializeField] private GameObject _progressBarObj;
        [SerializeField] private Image _progressBarFill;

        private int _currentEvolveCount;
        private EvolveRequiredToolData _evolveRequiredData => _data as EvolveRequiredToolData;

        public override void UpdateVisual(bool withAnimation)
        {
            var gameModeState = SaveManager.GameModeState;
            float value = (float)_currentEvolveCount / _evolveRequiredData.EvolveCountRequired;

            if (withAnimation)
                _progressBarFill.DOFillAmount(value, 0.3f).SetEase(Ease.OutSine);
            else
                _progressBarFill.fillAmount = value;

            var active = _currentEvolveCount >= _evolveRequiredData.EvolveCountRequired;
            _icon.sprite = active ? _data.ActiveIcon : _data.UnactiveIcon;
            _isActive = active;

            _progressBarObj.SetActive(gameModeState.Type != GameModeType.Classic);
        }

        protected virtual void OnEvolve(BoardEvolveEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.Type == GameModeType.Classic)
                return;

            var lastCount = _currentEvolveCount;
            _currentEvolveCount = Mathf.Clamp(_currentEvolveCount + 1, 0, _evolveRequiredData.EvolveCountRequired);

            if (lastCount < _evolveRequiredData.EvolveCountRequired && _currentEvolveCount >= _evolveRequiredData.EvolveCountRequired)
            {
                Vibration.Vibrate(30);
                AudioController.Play("fill_pickaxe");

                _anim.SetTrigger("OnActive");
            }

            UpdateVisual(true);
        }

        protected override void OnResetTool(ResetToolEvent eventData)
        {
            if (eventData.ToolType != _data.ToolType)
                return;

            _currentEvolveCount = 0;
            UpdateVisual(false);
        }

        public override void LoadSaveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                _currentEvolveCount = 0;
            else
                _currentEvolveCount = int.Parse(data);

            base.LoadSaveData(data);
        }

        protected override void OnSave(SaveEvent eventData)
        {
            if (eventData.Type != SaveType.GameMode || eventData.Phase != SavePhase.Prepare)
                return;

            var gameModeState = SaveManager.GameModeState;
            var toolState = gameModeState.GetToolState(_data.ToolType);

            toolState.Data = _currentEvolveCount.ToString();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null)
            {
                var toolState = gameModeState.GetToolState(_data.ToolType);
                LoadSaveData(toolState.Data);
            }

            EventsManager.Subscribe<BoardEvolveEvent>(OnEvolve);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventsManager.Unsubscribe<BoardEvolveEvent>(OnEvolve);
        }
    }
}