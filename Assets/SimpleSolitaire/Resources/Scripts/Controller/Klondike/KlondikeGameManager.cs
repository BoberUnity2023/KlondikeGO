using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class KlondikeGameManager : GameManager
    {
        private KlondikeCardLogic _klondikeCardLogic => _cardLogic as KlondikeCardLogic;

        protected override void InitCardLogic()
        {
            _klondikeCardLogic.InitRuleToggles();
        }

        protected override void OnStatisticsLayerClosed()
        {
            StatisticsController.Instance.InitRuleToggle(_klondikeCardLogic.CurrentRule);

            base.OnStatisticsLayerClosed();
        }

        public override void SetLevel(int level)
        {
            base.SetLevel(level);
            switch (level)
            {
                case 0:
                    {
                        _klondikeCardLogic.CurrentDifficultyType = KlondikeDifficultyType.Random;
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[0];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 1:
                    {
                        _klondikeCardLogic.CurrentDifficultyType = KlondikeDifficultyType.Random;
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[1];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[1];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 2:
                    {
                        _klondikeCardLogic.CurrentDifficultyType = KlondikeDifficultyType.Random;
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[2];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[2];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Error 1265. Unknown level");
                        _klondikeCardLogic.CurrentDifficultyType = KlondikeDifficultyType.Easy;
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[0];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        _buttonHint.SetActive(true);
                        break;
                    }
            }
        }
    }
}