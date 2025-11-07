using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class FreecellGameManager : GameManager
    {
        private FreecellCardLogic _freecellCardLogic => _cardLogic as FreecellCardLogic;

        protected override void InitCardLogic()
        {
            _freecellCardLogic.InitFreecellToggles();
        }

        public override void SetLevel(int level)
        {
            base.SetLevel(level);
            switch (level)
            {
                case 0:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[0];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 1:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[1];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[1];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 2:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[2];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        //_buttonHint.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Error 1265. Unknown level");
                        break;
                    }
            }
        }
    }
}