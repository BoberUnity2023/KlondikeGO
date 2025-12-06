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
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private KlondikeCardLogic _klondikeCardLogic;
        [SerializeField] private HintManager _hintManager;
        [SerializeField] private SimpleSolitaire.Controller.AudioController _audioController;
        [SerializeField] private Transform _buttonField;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Sprite _spriteActive;
        [SerializeField] private Sprite _spriteInActive;
        [SerializeField] private MagicWangScreenStars _stars;
        [SerializeField] private Text _counterIndicator;
        [SerializeField] private Image _imageCounter;
        [SerializeField] private Sprite _spriteCounterActive;
        [SerializeField] private Sprite _spriteCounterInActive;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private ParticleSystem _particlesRed;

        private List<Card> _targetOpenCards = new List<Card>();
        private Card _cardOpen;
        private Card _cardClose;
        private int _count;
        private bool _isProcess = false;  
        public bool IsProcess => _isProcess;

        public int Count
        {
            get { return _count; }
            set 
            {
                if (_count > value)
                    _particles.Play();

                if (value > _count)
                    _particlesRed.Play();

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
            bool hasClosedCard = AnyCloseCard != null;
            _buttonImage.sprite = hasClosedCard && Count > 0 ? _spriteActive : _spriteInActive;        
        }

        public void OnClickButton()
        {
            if (Count <= 0)
            {
                _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Error);
                _gameManager.ShowAdsLayerMagicWand();
                return; 
            }

            if (_isProcess || _hintManager.IsHintProcess)
            {
                Debug.Log("MagicWand: _isProcess");
                _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Error);
                return;
            }
                
            bool hasPair = FindPair();
            if (hasPair)
            {
                Debug.Log("MagicWand: " + "OpenCard: " + _cardOpen.GetTypeName() + _cardOpen.Number);
                Debug.Log("MagicWand: " + "CloseCard: " + _cardClose.GetTypeName() + _cardClose.Number);
                Count--;
                StartCoroutine(MagicWindTranslate(_cardClose, _cardOpen, OnComplete));
            }
            else
            { 
                Card _anyCloseCard = AnyCloseCard;
                if (_anyCloseCard != null)
                {
                    Debug.Log("MagicWand: card move to Pack");
                    Count--;
                    StartCoroutine(MagicWindTranslate(_anyCloseCard, null, OnComplete));
                }
                else
                { 
                    Debug.Log("MagicWand: card pair no found");
                    _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Error);
                }
            }
        }

        private void OnComplete()
        {
            Debug.Log("MagicWand: OnComplete()");
            SetButtonSprite();
            _hintManager.GenerateHints();
            _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Bonus);
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

        public void OnReward()
        {
            Count += 3;
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
                        card.CardColor != targetOpenCard.CardColor &&
                        card.Deck != targetOpenCard.Deck)
                    { 
                        return card;
                    }
                }
            }
            return null;
        }

        private Card AnyCloseCard
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
                            return card;
                        }
                    }
                }
                return null;
            }
        }

        public IEnumerator MagicWindTranslate(Card cardClose, Card cardOpen, UnityAction onComplete)
        {
            bool isMoveToPack = cardOpen == null;
            _isProcess = true;
            cardClose.transform.SetAsLastSibling();//Карта которую двигаем

            Vector3 fromPosition = cardClose.transform.localPosition;

            float verticalSpace = _klondikeCardLogic.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED);

            Vector3 toPosition = isMoveToPack ?
                _klondikeCardLogic.PackDeck.transform.localPosition :
                cardOpen.transform.localPosition - Vector3.up * verticalSpace; 
            
            
            Deck deckCardClose = cardClose.Deck;
            deckCardClose.CardsArray.Remove(cardClose);

            float distance = Vector3.Distance(fromPosition, toPosition);
            
            float moveTime = distance / Public.CardSpeed + 0.2f;
            int numJumps = (int)(distance / 300);
            cardClose.DragEffect("On");
            _stars.EffectStart(cardClose.transform);

            cardClose.transform.DOLocalJump(toPosition, 80, numJumps, moveTime).SetEase(Ease.InSine);

            yield return new WaitForSeconds(moveTime);

            deckCardClose.UpdateCardsPosition(false);
            cardClose.DragEffect("Off");
            if (!isMoveToPack)
            {
                cardClose.transform.DOScaleX(0, 0.2f);
                yield return new WaitForSeconds(0.2f);
            }           
            
            Deck deckFinish = isMoveToPack ? _klondikeCardLogic.PackDeck : cardOpen.Deck;
            deckFinish.CardsArray.Add(cardClose);
            cardClose.Deck = deckFinish;
            if (!isMoveToPack)
            {
                deckFinish.UpdateCardsPosition(false, true);
                cardClose.transform.DOScaleX(1, 0.2f);
            }
            _stars.EffectStop();
            _isProcess = false;
            onComplete();
        }        
    }
}
