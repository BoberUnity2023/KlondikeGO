using DG.Tweening;
using SimpleSolitaire.Controller;
using SimpleSolitaire.Model.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomLines
{
    public class MagicWand : MonoBehaviour
    {
        [SerializeField] private KlondikeCardLogic _klondikeCardLogic;
        [SerializeField] private Transform _buttonField;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Sprite _spriteActive;
        [SerializeField] private Sprite _spriteInActive;
        [SerializeField] private MagicWangScreenStars _stars;
        private List<Card> _targetOpenCards = new List<Card>();
        private Card _cardOpen;
        private Card _cardClose;
        private bool IsProcess = false;

        public void SetState()
        {
            Debug.Log("MagicWand: SetState()");
            bool success = FindPair();
            _buttonImage.sprite = success ? _spriteActive : _spriteInActive;        
        }

        public void OnClickButton()
        {
            if (IsProcess)
            {
                Debug.Log("MagicWand: IsProcess");
                return;
            }
                
            bool success = FindPair();
            if (success)
            {
                Debug.Log("MagicWand: " + "OpenCard: " + _cardOpen.GetTypeName() + _cardOpen.Number);
                Debug.Log("MagicWand: " + "CloseCard: " + _cardClose.GetTypeName() + _cardClose.Number);

                StartCoroutine(MagicWindTranslate(_cardClose, _cardOpen, OnComplete));
            }
            else
            { 
                Debug.Log("MagicWand: card pair no found"); 
            }
        }

        void OnComplete()
        {
            Debug.Log("MagicWand: OnComplete()");
            SetState();
        }

        private bool FindPair()
        {            
            _targetOpenCards.Clear();
            for (int i = 0; i < 7; i++)
            {
                Card targetOpenCard = TargetOpenCard;
                if (targetOpenCard == null)
                {
                    Debug.Log("TargetOpenCard no found");
                    return false;
                }

                //Debug.Log("TargetOpenCard: " + targetOpenCard.GetTypeName() + targetOpenCard.Number);
                _targetOpenCards.Add(targetOpenCard);

                Card targetCloseCard = TargetCloseCard(targetOpenCard);
                if (targetCloseCard != null)
                {
                    //Debug.Log("TargetCloseCard: " + targetCloseCard.GetTypeName() + targetCloseCard.Number);
                    _cardClose = targetCloseCard;
                    _cardOpen = targetOpenCard;                    
                    return true;
                }                
            }
            return false;
        }
        
        private Card TargetOpenCard
        {
            get
            {
                for (int d = 4; d < 11; d++)
                {
                    Deck deck = _klondikeCardLogic.AllDeckArray[d];

                    if (deck.CardsCount > 0)
                    {
                        Card card = deck.CardsArray[deck.CardsCount - 1];
                        if (card.CardStatus == 1 && !_targetOpenCards.Contains(card))//Open
                            return card;
                    }                                 
                }
                return null;
            }            
        }

        private Card TargetCloseCard(Card targetOpenCard)
        {
            for (int d = 4; d < 11; d++)
            {
                Deck deck = _klondikeCardLogic.AllDeckArray[d];

                for (int i = 0; i < deck.CardsCount; i++)
                {
                    Card card = deck.CardsArray[i];
                    if (card.CardStatus == 0 && 
                        card.Number == targetOpenCard.Number - 1 &&
                        card.CardColor != targetOpenCard.CardColor &&//Close
                        card.Deck != targetOpenCard.Deck)
                    { 
                        return card;
                    }
                }
            }
            return null;
        }

        public IEnumerator MagicWindTranslate(Card cardClose, Card cardOpen, UnityAction onComplete)
        {
            IsProcess = true;
            cardClose.transform.SetAsLastSibling();

            Vector3 fromPosition = cardClose.transform.localPosition;

            var verticalSpace = _klondikeCardLogic.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED);

            Vector3 toPosition = cardOpen.transform.localPosition - Vector3.up * verticalSpace; 
            Deck deckCardClose = cardClose.Deck;
            deckCardClose.CardsArray.Remove(cardClose);


            float distance = Vector3.Distance(fromPosition, toPosition);
            
            float moveTime = distance / Public.CardSpeed + 0.2f;
            int numJumps = (int)(distance / 300);
            cardClose.transform.DOLocalJump(toPosition, 80, numJumps, moveTime).SetEase(Ease.InSine);
            cardClose.DragEffect("On");

            _stars.EffectStart(cardClose.transform);            

            yield return new WaitForSeconds(moveTime);

            deckCardClose.UpdateCardsPosition(false);
            cardClose.DragEffect("Off");
            cardClose.transform.DOScaleX(0, 0.15f);            
            
            yield return new WaitForSeconds(0.15f);            
            
            Deck deckCardOpen = cardOpen.Deck;
            deckCardOpen.CardsArray.Add(cardClose);
            cardClose.Deck = deckCardOpen;
            deckCardOpen.UpdateCardsPosition(false);
            cardClose.transform.DOScaleX(1, 0.15f);
            _stars.EffectStop();
            IsProcess = false;
            onComplete();
        }
    }
}
