using System.Collections;
using SimpleSolitaire.Model;
using SimpleSolitaire.Model.Enum;
using System.Collections.Generic;
using UnityEngine;
using SimpleSolitaire.Model.Config;
using DG.Tweening;

namespace SimpleSolitaire.Controller
{
    public class KlondikeHintManager : HintManager
    {
        protected override IEnumerator HintTranslate(HintData data)
        {
            IsHintProcess = true;         

            //List<HintElement> hints = data.Type == HintType.AutoComplete ? AutoCompleteHints : Hints;
            List<HintElement> hints = AutoCompleteHints;
            if (data.Type == HintType.AutoComplete) CurrentHintIndex = 0;
            if (data.Card != null) CurrentHintIndex = hints.FindIndex(x => x.HintCard == data.Card);

            if (data.Card != null && CurrentHintIndex == -1)
            {
                AudioController audioCtrl = AudioController.Instance;
                if (audioCtrl != null)
                {
                    audioCtrl.Play(AudioController.AudioType.Error);
                }

                //Debug.LogWarning("After double tap! This Card: " + data.Card.CardNumber +
                //                 " is not available for complete to ace pack.");
                IsHintProcess = false;
                CurrentHintIndex = 0;
                yield break;
            }
            
            HintElement hint = hints[CurrentHintIndex];
            Card hintCard = hint.HintCard;
            hintCard.Deck.UpdateCardsPosition(false);

            CurrentHintSiblingIndex = hintCard.transform.GetSiblingIndex();

            hintCard.Deck.SetCardsToTop(hintCard);

            Vector3 fromPosition = hint.FromPosition;
            Vector3 toPosition = hint.ToPosition;

            float distance = Vector3.Distance(fromPosition, toPosition);
            float moveTime = distance / Public.CardSpeed + 0.20f;
            if (_gameManager.Save.Speed == 0)
                moveTime *= 1.5f;
            if (_gameManager.Save.Speed == 2)
                moveTime *= 0.5f;

            //if (data.Type == HintType.AutoComplete)
            //    moveTime *= 0.3f;

            float jumpPower = Random.Range(-200, 200);

            hintCard.transform.DOLocalJump(toPosition, jumpPower, 1, moveTime).SetEase(Ease.InOutQuad).OnUpdate(
                    () => hint.HintCard.Deck.SetPositionFromCard(hintCard,
                    hintCard.transform.localPosition.x,
                    hintCard.transform.localPosition.y)
                    );

            List<Card> cardsToTop = hintCard.Deck.SetCardsToTop(hintCard);//Движение стопки карт

            hintCard.DragEffect("On", cardsToTop);

            float addTime = _gameManager.Save.Speed == 2 ? 0.05f : 0.1f;
            yield return new WaitForSeconds(moveTime + addTime);

            hintCard.DragEffect("Off");

            if (IsHasHint() && data.Type == HintType.Hint)
            {
                hintCard.Deck.UpdateCardsPosition(false);
                hintCard.transform.localPosition = fromPosition;
                hintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
                CurrentHintIndex = CurrentHintIndex == hints.Count - 1 ? CurrentHintIndex = 0 : CurrentHintIndex + 1;
            }

            if (data.Type != HintType.Hint)
            {
                _cardLogicComponent.OnDragEnd(hintCard);
            }
            //float time = _gameManager.Speed * 0.1f;
            float t = 0.18f;
            if (data.Type != HintType.AutoComplete)
                t = 0.05f;
            yield return new WaitForSeconds(t);//wait 0.15sec from hintCard.Deck.UpdateCardsPosition(false)
            UpdateAvailableForDragCards();
            IsHintProcess = false;
        }

        /// <summary>
        /// Generate new hint depending on available for move cards.
        /// </summary>
        public override void GenerateHints(bool isAutoComplete = false)
        {
            //Debug.Log("Generate Hints");
            CurrentHintIndex = 0;
            AutoCompleteHints = new List<HintElement>();
            Hints = new List<HintElement>();
            bool isHasAutoCompleteHints;

            if (IsAvailableForMoveCardArray.Count > 0)
            {
                foreach (var card in IsAvailableForMoveCardArray)
                {
                    for (int i = 0; i < _cardLogicComponent.AllDeckArray.Length; i++)
                    {
                        isHasAutoCompleteHints = true;
                        Deck targetDeck = _cardLogicComponent.AllDeckArray[i];
                        if (targetDeck.Type == DeckType.DECK_TYPE_BOTTOM || targetDeck.Type == DeckType.DECK_TYPE_ACE)
                        {
                            if (card != null)
                            {
                                Card topTargetDeckCard = targetDeck.GetTopCard();
                                Card topDeckCard = card.Deck.GetPreviousFromCard(card);

                                if (card.Deck.Type == DeckType.DECK_TYPE_ACE)
                                {
                                    continue;
                                }

                                if (topDeckCard == null && topTargetDeckCard == null &&
                                    targetDeck.Type != DeckType.DECK_TYPE_ACE)
                                {
                                    if (card.Deck.Type != DeckType.DECK_TYPE_WASTE)
                                    {
                                        isHasAutoCompleteHints = false;

                                        if (isAutoComplete)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                if (topDeckCard != null && topTargetDeckCard != null &&
                                    topDeckCard.Number == topTargetDeckCard.Number && topDeckCard.CardStatus == 1 &&
                                    card.Deck.Type != DeckType.DECK_TYPE_WASTE)
                                {
                                    isHasAutoCompleteHints = false;

                                    if (isAutoComplete)
                                    {
                                        continue;
                                    }
                                }

                                if (targetDeck.AcceptCard(card))
                                {
                                    var offset = GetHintSpace(topTargetDeckCard);
                                    if (isHasAutoCompleteHints)
                                    {
                                        AutoCompleteHints.Add(new HintElement(card, card.transform.localPosition,
                                            topTargetDeckCard != null
                                                ? topTargetDeckCard.transform.localPosition - offset
                                                : targetDeck.transform.localPosition, targetDeck));
                                    }

                                    Hints.Add(new HintElement(card, card.transform.localPosition,
                                        topTargetDeckCard != null
                                            ? topTargetDeckCard.transform.localPosition - offset
                                            : targetDeck.transform.localPosition, targetDeck));
                                }
                            }
                        }
                    }
                }
            }

            ActivateHintButton(IsHasAutoCompleteHint());
            ActivateAutoCompleteHintButton(IsHasAutoCompleteHint());
            _magicWand.SetButtonSprite();
        }        
    }
}