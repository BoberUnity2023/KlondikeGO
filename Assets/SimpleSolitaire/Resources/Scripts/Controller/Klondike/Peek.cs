using DG.Tweening;
using SimpleSolitaire.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class Peek : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private KlondikeCardLogic _klondikeCardLogic;
        [SerializeField] private HintManager _hintManager;
        [SerializeField] private SimpleSolitaire.Controller.AudioController _audioController;
        [SerializeField] private GameObject _backgroundBlocker;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Sprite _spriteActive;
        [SerializeField] private Sprite _spriteInActive;
        [SerializeField] private Text _counterIndicator;
        [SerializeField] private Image _imageCounter;
        [SerializeField] private Sprite _spriteCounterActive;
        [SerializeField] private Sprite _spriteCounterInActive;
        [SerializeField] private List<Card> _closeCards;
        [SerializeField] private bool _isProcess;
        [SerializeField] private bool _isCardsRotateCloseing;
        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                _counterIndicator.text = value.ToString();
                _imageCounter.sprite = _count > 0 ? _spriteCounterActive : _spriteCounterInActive;
                SetButtonSprite();
            }
        }

        public void StartParty()
        {
            Count = 3;
        }

        public void SetButtonSprite()
        {
            //Debug.Log("MagicWand: SetButtonSprite()");
            bool hasClosedCard = HasCloseCard;
            _buttonImage.sprite = hasClosedCard && Count > 0 ? _spriteActive : _spriteInActive;
        }

        public void OnClickButton()
        {
            if (Count <= 0)
            {
                _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Error);
                _gameManager.ShowAdsLayerPeek();
                return;
            }

            if (_isProcess || _hintManager.IsHintProcess || _gameManager.MagicWand.IsProcess)
                return;

            Count--;
            _isProcess = true;
            _backgroundBlocker.SetActive(true);            
            _closeCards = CloseCards;

            foreach (Card card in _closeCards)
            {
                card.transform.DOScaleX(0, 0.15f);
            }
            StartCoroutine(CardsRotateOpen());            
        }

        private IEnumerator CardsRotateOpen()
        {
            yield return new WaitForSeconds(0.15f);
            foreach (Card card in _closeCards)
            {
                card.transform.DOScaleX(1, 0.15f);
                card.CardStatus = 1;
            }

            UpdateCardsPositionInDecks();
        }

        public void OnClickButtonClose()
        {
            if (_isCardsRotateCloseing)
                return;

            foreach (Card card in _closeCards)
            {
                card.transform.DOScaleX(0, 0.15f);
            }

            _isCardsRotateCloseing = true;
            StartCoroutine(CardsRotateClose());
        }

        private IEnumerator CardsRotateClose()
        {
            yield return new WaitForSeconds(0.15f);
            foreach (Card card in _closeCards)
            {
                card.transform.DOScaleX(1, 0.15f);
                card.CardStatus = 0;
            }

            UpdateCardsPositionInDecks();
            _backgroundBlocker.SetActive(false);            
            _isProcess = false;
            _isCardsRotateCloseing = false;
        }

        private void UpdateCardsPositionInDecks()
        {
            for (int d = 4; d < 11; d++)
            {
                Deck deck = _klondikeCardLogic.AllDeckArray[d];
                deck.UpdateCardsPosition(false);
            }
        }

        public void OnReward()
        {
            Count += 3;
        }

        private List<Card> CloseCards
        {
            get
            {
                List<Card> output = new List<Card>();
                for (int d = 4; d < 11; d++)
                {
                    Deck deck = _klondikeCardLogic.AllDeckArray[d];

                    for (int i = 0; i < deck.CardsCount; i++)
                    {
                        Card card = deck.CardsArray[i];
                        if (card.CardStatus == 0)
                        {
                            output.Add(card);
                        }
                    }
                }
                return output;
            }            
        }

        private bool HasCloseCard
        {
            get
            {
                for (int d = 4; d < 11; d++)
                {
                    Deck deck = _klondikeCardLogic.AllDeckArray[d];

                    for (int i = 0; i < deck.CardsCount; i++)
                    {
                        Card card = deck.CardsArray[i];
                        if (card.CardStatus == 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
