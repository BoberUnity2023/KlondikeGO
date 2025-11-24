using SimpleSolitaire.Controller;
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
        private List<Card> _closeCards;
        private bool _isProcess;
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
                card.CardStatus = 1;
            }

            for (int d = 4; d < 11; d++)
            {
                Deck deck = _klondikeCardLogic.AllDeckArray[d];
                deck.UpdateCardsPosition(false);
            }
        }

        public void OnClickButtonClose()
        {            
            foreach (Card card in _closeCards)
            {
                card.CardStatus = 0;
            }

            for (int d = 4; d < 11; d++)
            {
                Deck deck = _klondikeCardLogic.AllDeckArray[d];
                deck.UpdateCardsPosition(false);
            }
            _backgroundBlocker.SetActive(false);
            _isProcess = false;
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
