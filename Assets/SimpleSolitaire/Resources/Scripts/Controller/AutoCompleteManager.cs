using System.Collections;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public enum AutoCompleteMode
    {
        FullGameSession = 0,
        OnlyWhenAllDecksClear
    }

    public class AutoCompleteManager : MonoBehaviour
    {
        [Tooltip("The mode of auto complete actions. Activates for full game session and only when all decks clear.")]
        public AutoCompleteMode Mode;
        
        [Tooltip("The state of auto complete actions.")]
        public bool IsAutoCompleteActive = false;

        [Tooltip("Time between cards sets on correct place. (Transition)")]
        public float HintSetTransitionTime = 0.2f;

        [Header("Components")]
        [SerializeField] private GameManager _gameManager;
        public HintManager HintComponent;
        public CardLogic CardLogicComponent;
        public GameObject AutoCompleteHintButtonObj;
        [SerializeField] private BottomMenu _bottomMenu;

        private IEnumerator _doubleClickAutoCompleteCoroutine;
        private IEnumerator _autoCompleteCoroutine;
        private bool _isCanComplete = true;

        private bool _autoCompleteFeatureEnable = true;
        [SerializeField] private bool _autoCompletePressed;

        
        public void SetEnableAutoCompleteFeature(bool state)
        {
            _autoCompleteFeatureEnable = state;
            AutoCompleteHintButtonObj.SetActive(_isCanComplete && state && CheckAvailabilityByMode() && !_autoCompletePressed);
        }

        /// <summary>
        /// Activate autocomplete availability with button.
        /// </summary>
        public void ActivateAutoCompleteAvailability()
        {
            _isCanComplete = true;

            if (!_autoCompleteFeatureEnable)
            {
                return;
            }

            bool isAvailable = CheckAvailabilityByMode() && !_autoCompletePressed;

            AutoCompleteHintButtonObj.SetActive(isAvailable);
        }

        private bool CheckAvailabilityByMode()
        {
            if (Mode == AutoCompleteMode.OnlyWhenAllDecksClear)
            {
                bool isAllDecksClear = true;

                for (int i = 0; i < CardLogicComponent.BottomDeckArray.Length; i++)
                {
                    var deck = CardLogicComponent.BottomDeckArray[i];
                    for (int j = 0; j < deck.CardsArray.Count; j++)
                    {
                        var card = deck.CardsArray[j];
                        if (!card.IsDraggable)
                        {
                            isAllDecksClear = false;
                        }
                    }
                }
                
                if (!isAllDecksClear)
                {
                    return false;
                }

                isAllDecksClear = (CardLogicComponent.PackDeck != null && CardLogicComponent.PackDeck.CardsCount == 0) 
                                  &&  (CardLogicComponent.WasteDeck == null || CardLogicComponent.WasteDeck.CardsArray.Count <= 1);
                
                if (!isAllDecksClear)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deactivate autocomplete availability with button.
        /// </summary>
        public void DeactivateAutoCompleteAvailability()
        {
            _isCanComplete = false;

            if (!_autoCompleteFeatureEnable)
            {
                return;
            }
            AutoCompleteHintButtonObj.SetActive(false);
        }

        /// <summary>
        /// Call auto complete action.
        /// </summary>
        public void CompleteGame()
        {
            if (_isCanComplete)
            {
                _autoCompletePressed = true;
                _isCanComplete = false;
                StopAutoComplete();
                AutoCompleteHintButtonObj.SetActive(false);
                _autoCompleteCoroutine = CompleteCoroutine();                
                StartCoroutine(_autoCompleteCoroutine);
                _bottomMenu.PressDown();
                _gameManager.DisableButtons.Deactivate();
            }
        }

        /// <summary>
        /// Auto complete actions in coroutine.
        /// </summary>
        private IEnumerator CompleteCoroutine()
        {
            IsAutoCompleteActive = true;
            _gameManager.LogoCorner.Show();
            HintComponent.UpdateAvailableForAutoCompleteCards();

            while (HintComponent.IsHasHint())
            {
                HintComponent.HintAndSet(HintSetTransitionTime);

                yield return new WaitWhile(() => HintComponent.IsHintProcess);
            }
            _gameManager.LogoCorner.Hide();
            IsAutoCompleteActive = false;
            HintComponent.UpdateAvailableForDragCards();
        }

        /// <summary>
        /// Deactivate auto complete coroutine.
        /// </summary>
        private void StopAutoComplete()
        {
            if (_autoCompleteCoroutine != null)
            {
                StopCoroutine(_autoCompleteCoroutine);
            }
        }
    }
}