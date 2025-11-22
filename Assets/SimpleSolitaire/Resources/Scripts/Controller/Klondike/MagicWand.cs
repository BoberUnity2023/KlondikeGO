using SimpleSolitaire.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class MagicWand : MonoBehaviour
    {
        [SerializeField] private KlondikeCardLogic _klondikeCardLogic;
        [SerializeField] private HintManager _hintManager;
        [SerializeField] private Transform _buttonField;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Sprite _spriteActive;
        [SerializeField] private Sprite _spriteInActive;
        private List<Card> _targetOpenCards = new List<Card>();
        private Card _cardOpen;
        private Card _cardClose;

        public void SetState()
        {
            Debug.Log("MagicWand: SetState()");
            bool success = FindPair();
            _buttonImage.sprite = success ? _spriteActive : _spriteInActive;        
        }

        public void OnClickButton()
        {
            if (_hintManager.IsMagicWangProcess)
            {
                Debug.Log("MagicWand: _hintManager.IsHintProcess");
                return;
            }
                //ShowDebug();
            bool success = FindPair();
            if (success)
            {
                Debug.Log("MagicWand: " + "OpenCard: " + _cardOpen.GetTypeName() + _cardOpen.Number);
                Debug.Log("MagicWand: " + "CloseCard: " + _cardClose.GetTypeName() + _cardClose.Number);
             
                StartCoroutine(_hintManager.MagicWindTranslate(_cardClose, _cardOpen, OnComplete));
            }
            else
                Debug.Log("MagicWand: card pair no found");
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

        //private void ShowDebug()
        //{            
        //    for (int d = 4; d < 11; d++)
        //    {
        //        Debug.Log("Deck " + (d-3).ToString());
        //        Deck deck = _klondikeCardLogic.AllDeckArray[d];

        //        for (int i = deck.CardsCount - 1; i >= 0; i--)
        //        {
        //            Card card = deck.CardsArray[i];
        //            string n = card.GetTypeName();
        //            string color = card.CardColor == 0 ? "black" : "red";
        //            Debug.Log(card.Number + n + "; open: " + card.CardStatus + "; color: " + color);
        //        }
        //        Debug.Log("--");
        //    }
        //}
        
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
                    //for (int i = deck.CardsCount - 1; i >= 0 ; i--)
                    //{
                    //    Card card = deck.CardsArray[i];
                    //    if (card.CardStatus == 1 && !_targetOpenCards.Contains(card))//Open
                    //        return card;                        
                    //}                    
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
                        card.CardColor != targetOpenCard.CardColor)//Close
                    { 
                        return card;
                    }
                }
            }
            return null;
        }
    }
}
