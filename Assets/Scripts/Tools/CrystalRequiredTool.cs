using System.Collections;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Cursor;
using BloomLines.Helpers;
using BloomLines.Managers;
using TMPro;
using UnityEngine;

namespace BloomLines.Tools
{
    public abstract class CrystalRequiredTool : ToolBase
    {
        [SerializeField] private TextMeshProUGUI _crystalCountText;
        [SerializeField] private GameObject _crystalIcon;
        [SerializeField] private CrystalRequiredTool _prevTool;
        [SerializeField] private CrystalRequiredTool _nextTool;

        private int _currentCrystalCount;
        private CrystalRequiredToolData _crystalRequiredData => _data as CrystalRequiredToolData;

        public bool IsPrevCollected
        {
            get
            {
                return _prevTool != null ? (_prevTool.IsCollected && _prevTool.IsPrevCollected) : true;
            }
        }
        public bool IsCollected
        {
            get
            {
                var crystalCountLeft = Mathf.Clamp(_crystalRequiredData.CrystalCountRequired - _currentCrystalCount, 0, int.MaxValue);
                return crystalCountLeft <= 0;
            }
        }

        public override void UpdateVisual(bool withAnimation)
        {
            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;
            var tutorialCompleted = gameState.CompletedTutorials.Contains($"first_show_{_data.ToolType.ToString().ToLowerInvariant()}");

            float value = (float)_currentCrystalCount / _crystalRequiredData.CrystalCountRequired;
            var crystalCountLeft = Mathf.Clamp(_crystalRequiredData.CrystalCountRequired - _currentCrystalCount, 0, int.MaxValue);

            _crystalCountText.text = crystalCountLeft.ToString();
            _icon.sprite = _currentCrystalCount >= _crystalRequiredData.CrystalCountRequired ? _data.ActiveIcon : _data.UnactiveIcon;

            _isActive = crystalCountLeft <= 0;

            _crystalIcon.SetActive(tutorialCompleted && crystalCountLeft > 0 && gameModeState.Type != GameModeType.Classic);
            _icon.gameObject.SetActive(tutorialCompleted || gameModeState.Type != GameModeType.Adventure);

            if(_nextTool != null && crystalCountLeft <= 0)
            {
                var nextTutorialId = $"first_show_{_nextTool._data.ToolType.ToString().ToLowerInvariant()}";
                var nextTutorialCompleted = gameState.CompletedTutorials.Contains(nextTutorialId);
                if (!nextTutorialCompleted)
                {
                    gameState.CompletedTutorials.Add(nextTutorialId);
                    _nextTool.UpdateVisual(false);

                    SaveManager.Save(SaveType.Game);
                }
            }
        }

        private void OnCutPlantSpawner(CutPlantSpawnedEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.Type == GameModeType.Classic || eventData.CutPlant.PlantData.PlantType != PlantType.Flower_Crystal)
                return;

            if(!eventData.CutPlant.IsCollected)
                StartCoroutine(CollectCrystalFlower(eventData.CutPlant));
        }
        
        private IEnumerator CollectCrystalFlower(CutPlant cutPlant)
        {
            bool collect = false;

            if (_prevTool == null)
            {
                if (!IsCollected)
                {
                    collect = true;
                }
            }
            else
            {
                if (_prevTool.IsCollected && _prevTool.IsPrevCollected && !IsCollected)
                {
                    collect = true;
                }
            }

            if (!collect)
                yield break;

            var lastCount = _currentCrystalCount;
            _currentCrystalCount = Mathf.Clamp(_currentCrystalCount++ + 1, 0, _crystalRequiredData.CrystalCountRequired);

            cutPlant.FlyCollect(_crystalIcon.transform.position, true, () =>
            {
                if (lastCount < _crystalRequiredData.CrystalCountRequired && _currentCrystalCount >= _crystalRequiredData.CrystalCountRequired)
                {
                    Vibration.Vibrate(50);
                    _anim.SetTrigger("OnActive");
                }
                else
                {
                    Vibration.Vibrate(30);
                }

                UpdateVisual(true);
            });
        }

        protected override void OnResetTool(ResetToolEvent eventData)
        {
            if (eventData.ToolType != _data.ToolType)
                return;

            _currentCrystalCount = 0;
            UpdateVisual(false);
        }

        private void OnCheckFreeCrystalSlot(CheckFreeCrystalSlotEvent eventData)
        {
            if (!IsCollected)
                eventData.HaveFreeSlot = true;
        }

        #region ISaveable
        public override void LoadSaveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                _currentCrystalCount = 0;
            else
                _currentCrystalCount = int.Parse(data);

            base.LoadSaveData(data);
        }

        protected override void OnSave(SaveEvent eventData)
        {
            if (eventData.Type != SaveType.GameMode || eventData.Phase != SavePhase.Prepare)
                return;

            var gameModeState = SaveManager.GameModeState;
            var toolState = gameModeState.GetToolState(_data.ToolType);

            toolState.Data = _currentCrystalCount.ToString();
        }
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null)
            {
                var toolState = gameModeState.GetToolState(_data.ToolType);
                LoadSaveData(toolState.Data);
            }

            EventsManager.Subscribe<CutPlantSpawnedEvent>(OnCutPlantSpawner);
            EventsManager.Subscribe<CheckFreeCrystalSlotEvent>(OnCheckFreeCrystalSlot);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventsManager.Unsubscribe<CutPlantSpawnedEvent>(OnCutPlantSpawner);
            EventsManager.Unsubscribe<CheckFreeCrystalSlotEvent>(OnCheckFreeCrystalSlot);
        }
    }
}