using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public enum KlondikeDifficultyType
    {
        Random = 0,
        Easy = 1,
    }

    public class KlondikeCardLogic : CardLogic
    {
        protected override int CardNums => Public.KLONDIKE_CARD_NUMS;

        public DeckRule TempRule { get; set; }
        public DeckRule CurrentRule;

        public KlondikeDifficultyType CurrentDifficultyType;
        public int DifficultyReplaceAmount = 16;

        [Header("Rule toggles:")] [SerializeField]
        private Toggle _oneDrawRuleToggle;

        [SerializeField] private Toggle _threeDrawRuleToggle;

        private void ChangeRuleTypeByToggle(DeckRule rule)
        {
            if (CurrentRule == rule) return;

            TempRule = rule;
        }

        public void InitRuleToggles()
        {
            TempRule = CurrentRule;

            _oneDrawRuleToggle.SetIsOnWithoutNotify(CurrentRule == DeckRule.ONE_RULE);
            _threeDrawRuleToggle.SetIsOnWithoutNotify(CurrentRule == DeckRule.THREE_RULE);
        }

        public override void InitCardLogic()
        {
            InitRuleToggles();

            base.InitCardLogic();
        }

        protected override void GenerateRandomCardNums()
        {
            switch (CurrentDifficultyType)
            {
                case KlondikeDifficultyType.Random:
                {
                    base.GenerateRandomCardNums();
                    break;
                }
                case KlondikeDifficultyType.Easy:
                {
                    base.GenerateRandomCardNums();
                    int replaceAmount = DifficultyReplaceAmount;
                    int lastReplaceIndex = 0;
                    int bottomDeckCardsCounter = 28;
                    for (int i = CardNumberArray.Length - 1; i > 0; i--)
                    {
                        if (replaceAmount <= 0 || bottomDeckCardsCounter <= 0)
                        {
                            break;
                        }

                        bottomDeckCardsCounter--;

                        if (CardNumberArray[i] % 13 < 6 || CardNumberArray[i] % 13 == 12)
                        {
                            replaceAmount--;

                            while (CardNumberArray[lastReplaceIndex] % 13 < 6 || CardNumberArray[lastReplaceIndex] % 13 == 12)
                            {
                                if (lastReplaceIndex > CardNums)
                                {
                                    break;
                                }

                                lastReplaceIndex++;
                                replaceAmount--;
                            }

                            if (replaceAmount <= 0)
                            {
                                break;
                            }

                            int currentCardValue = CardNumberArray[i];
                            int cardForReplaceValue = CardNumberArray[lastReplaceIndex];

                            CardNumberArray[lastReplaceIndex] = currentCardValue;
                            CardNumberArray[i] = cardForReplaceValue;

                            /* Test debug.
                            Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
                            */
                        }
                    }
                        int card = CardNumberArray[51];
                        CardNumberArray[51] = CardNumberArray[25];
                        CardNumberArray[25] = card;

                        card = CardNumberArray[49];
                        CardNumberArray[49] = CardNumberArray[26];
                        CardNumberArray[26] = card;

                        card = CardNumberArray[46];
                        CardNumberArray[46] = CardNumberArray[27];
                        CardNumberArray[27] = card;

                        card = CardNumberArray[42];
                        CardNumberArray[42] = CardNumberArray[28];
                        CardNumberArray[28] = card;

                        break;
                }
            }
        }

        public override void InitializeSpacesDictionary()
        {
            base.InitializeSpacesDictionary();

            SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED, DeckHeight / 3.5f);
            SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_CLOSED, DeckHeight / 3.5f / 2);
            SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE, DeckWidth / 2.3f);
        }

        public override void SubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeRuleTypeByToggle(DeckRule.ONE_RULE); });
            _threeDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeRuleTypeByToggle(DeckRule.THREE_RULE); });
        }

        public override void UnsubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.RemoveAllListeners();
            _threeDrawRuleToggle.onValueChanged.RemoveAllListeners();
        }

        public override void OnNewGameStart()
        {
            CurrentRule = TempRule;
            StatisticsController.Instance.InitRuleToggle(CurrentRule);
            IsGameStarted = true;
        }

        public void SetRuleImmediately(DeckRule rule)
        {
            TempRule = rule;
            CurrentRule = TempRule;
        }

        /// <summary>
        /// Initialize deck of cards.
        /// </summary>
        protected override void InitDeckCards()
        {
            for (int i = 0; i < BottomDeckArray.Length; i++)
            {
                Deck bottomDeck = BottomDeckArray[i];

                for (int j = 0; j < i + 1; j++)
                {
                    bottomDeck.PushCard(PackDeck.Pop());
                }

                bottomDeck.UpdateCardsPosition(true);
                bottomDeck.UpdateDraggableStatus();
            }

            PackDeck.UpdateCardsPosition(true);
            PackDeck.UpdateDraggableStatus();

            WasteDeck.UpdateCardsPosition(true);
        }

        /// <summary>
        /// Call when we drop card.
        /// </summary>
        /// <param name="card">Dropped card</param>
        public override void OnDragEnd(Card card)
        {
            bool isPackWasteNotFound = false;
            bool isHasTarget = false;
            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                Deck targetDeck = AllDeckArray[i];
                if (targetDeck.Type == DeckType.DECK_TYPE_BOTTOM || targetDeck.Type == DeckType.DECK_TYPE_ACE)
                {
                    if (targetDeck.OverlapWithCard(card))
                    {
                        isHasTarget = true;
                        Deck srcDeck = card.Deck;

                        if (targetDeck.AcceptCard(card))
                        {
                            WriteUndoState();
                            Card[] popCards = srcDeck.PopFromCard(card);
                            targetDeck.PushCardArray(popCards);
                            targetDeck.UpdateCardsPosition(false);
                            srcDeck.UpdateCardsPosition(false);

                            ActionAfterEachStep();

                            if (targetDeck.Type == DeckType.DECK_TYPE_ACE)
                            {
                                GameManagerComponent.AddScoreValue(Public.SCORE_MOVE_TO_ACE * 3);
                                if (AudioCtrl != null)
                                {
                                    AudioCtrl.Play(AudioController.AudioType.MoveToAce);
                                }
                            }
                            else
                            {
                                if (AudioCtrl != null)
                                {
                                    AudioCtrl.Play(AudioController.AudioType.Move);
                                }
                            }

                            return;
                        }
                    }
                }
                else
                {
                    isPackWasteNotFound = true;
                }
            }

            if (isPackWasteNotFound &&
                (card.Deck.Type != DeckType.DECK_TYPE_PACK && card.Deck.Type != DeckType.DECK_TYPE_WASTE) ||
                isHasTarget)
            {
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Error);
                }
            }
        }

        /// <summary>
        /// Call when we click on pack with cards.
        /// </summary>
        public override void OnClickPack()
        {
            if (!PackDeck.HasCards && !WasteDeck.HasCards)
            {
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Error);
                }

                return;
            }

            WriteUndoState();

            switch (CurrentRule)
            {
                case DeckRule.ONE_RULE:
                    IsNeedResetPack = PackDeck.CardsCount == 1;

                    if (PackDeck.HasCards)
                    {
                        WasteDeck.PushCard(PackDeck.Pop());
                        PackDeck.UpdateCardsPosition(false);
                        WasteDeck.UpdateCardsPosition(false);
                        if (AudioCtrl != null)
                        {
                            AudioCtrl.Play(AudioController.AudioType.MoveToWaste);
                        }
                    }
                    else
                    {
                        if (WasteDeck.HasCards)
                        {
                            MoveFromWasteToPack();
                        }
                    }

                    break;
                case DeckRule.THREE_RULE:
                    for (int i = 0; i < 3; i++)
                    {
                        if (IsNeedResetPack)
                        {
                            MoveFromWasteToPack();
                            IsNeedResetPack = false;
                            break;
                        }

                        if (PackDeck.HasCards)
                        {
                            WasteDeck.PushCard(PackDeck.Pop());
                            PackDeck.UpdateCardsPosition(false);
                            WasteDeck.UpdateCardsPosition(false);
                            IsNeedResetPack = !PackDeck.HasCards;
                            if (IsNeedResetPack) break;
                            if (AudioCtrl != null)
                            {
                                AudioCtrl.Play(AudioController.AudioType.MoveToWaste);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
            }

            ActionAfterEachStep();
        }

        public override float GetSpaceFromDictionary(DeckSpacesTypes type)
        {
            if (SpacesDict == null || !SpacesDict.ContainsKey(type))
            {
                return 0f;
            }

            float canvasScale = CanvasScaler.referenceResolution.y / 1080;

            if (type == DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED)
            {
                float output = (float)UnityEngine.Screen.height / 20 / canvasScale;
                //Debug.LogWarning("GetSpaceFromDictionary[" + type.ToString() + "]: " + output);
                return output;
            }

            if (type == DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_CLOSED)
            {
                float output = (float)UnityEngine.Screen.height / 40 / canvasScale;
                //Debug.LogWarning("GetSpaceFromDictionary[" + type.ToString() + "]: " + output);
                return output;
            }

            if (type == DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE)
            {
                float output = (float)UnityEngine.Screen.height / 22 / canvasScale;
                //Debug.LogWarning("GetSpaceFromDictionary[" + type.ToString() + "]: " + output);
                return output;
            }

            return SpacesDict[type];
        }

        public void OnToggleChangeRule(int rule)
        {
            if (rule == 1)            
                CurrentRule = DeckRule.ONE_RULE;

            if (rule == 3)
                CurrentRule = DeckRule.THREE_RULE;

        }
    }
}