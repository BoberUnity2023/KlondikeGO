using System;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SimpleSolitaire.Controller
{
    
    public enum DeckSpacesTypes
    {
        DECK_SPACE_VERTICAL_BOTTOM_OPENED = 1,
        DECK_SPACE_VERTICAL_BOTTOM_CLOSED = 2,
        DECK_SPACE_HORIONTAL_WASTE = 3,
        DECK_SPACE_VERTICAL_ACE = 4,
        DECK_SPACE_VERTICAL_FREECELL = 5,
        DECK_PACK_HORIZONTAL = 6,
    }
    
    public abstract class CardLogic : MonoBehaviour
    {
        protected Vector3[] Corners;
        [SerializeField]
        protected RectTransform CorrectlyDeck;
        
        public Color DraggableColor;
        public Color NondraggableColor;
        
        public List<HandOrientationElement> DeckElements;

        public Card[] CardsArray;
        public int[] CardNumberArray = new int[52];
        public Deck[] BottomDeckArray = new Deck[7];
        public Deck[] AceDeckArray = new Deck[4];
        public Deck[] AllDeckArray = new Deck[13];
        public Deck WasteDeck;
        public Deck PackDeck;
        public GameManager GameManagerComponent;
        public HintManager HintManagerComponent;
        public AutoCompleteManager AutoCompleteComponent;
        public UndoPerformer UndoPerformerComponent;

        public ParticleSystem ParticleStars;
        public CanvasScaler CanvasScaler;

        private readonly string _packNone = "pack_deck_none";
        private readonly string _packRotate = "pack_deck_rotate";
        private bool _isUserMadeFirstMove;
        private bool _isGameStarted;

        public bool HighlightDraggable { get; set; }
        public bool IsGameStarted { get; protected set; }
        public bool IsNeedResetPack { get; set; }
        public HandOrientation Orientation { get; set; }

        protected abstract int CardNums { get; }
        
        protected AudioController AudioCtrl;

        protected Dictionary<DeckSpacesTypes, float> SpacesDict;
        protected Dictionary<string, Sprite> CachedSprtesDict;
        
        public float DeckWidth => _deckWidth;
        public float DeckHeight => _deckHeight;

        private float _deckWidth;
        private float _deckHeight;

        protected void Awake()
        {
            CachedSprtesDict = new Dictionary<string, Sprite>();
            HighlightDraggable = true;
            InitializeSpacesDictionary();
        }

        protected void Start()
        {
            AudioCtrl = AudioController.Instance;
        }

        /// <summary>
        /// Initialize logic structure of game session.
        /// </summary>
        public virtual void InitCardLogic()
        {
            InitCardNodes();
            InitAllDeckArray();
            //SetHandOrientation();
            UndoPerformerComponent.ResetUndoStates();
            //ParticleStars.Stop();
        }

        public virtual void InitializeSpacesDictionary()
        {
            SpacesDict = new Dictionary<DeckSpacesTypes, float>();
            Corners = new Vector3[4];
            CorrectlyDeck.GetWorldCorners(Corners);

            _deckHeight = Corners[2].y - Corners[0].y;
            _deckWidth = Corners[2].x - Corners[0].x;
        }

        public virtual float GetSpaceFromDictionary(DeckSpacesTypes type)
        {
            if (SpacesDict == null || !SpacesDict.ContainsKey(type))
            {
                return 0f;
            }            

            return SpacesDict[type];
        }

        public abstract void SubscribeEvents();
        public abstract void UnsubscribeEvents();
        
        public abstract void OnNewGameStart();

        private void OnDisable()
        {
            if (HintManagerComponent != null)
            {
                HintManagerComponent.ResetHint();
            }
        }

        /// <summary>
        /// Randomize cards.
        /// </summary>
        protected virtual void GenerateRandomCardNums()
        {
            GenerateRandomCardNums1();
        }

        private void GenerateRandomCardNums1()
        {
            int cardNums = CardNums;
            int[] tagArray = new int[cardNums];
            int i = 0;
            for (i = 0; i < cardNums; i++)
            {
                tagArray[i] = 0;
            }

            for (i = 0; i < cardNums; i++)
            {
                int rand = Random.Range(0, cardNums);
                while (rand < cardNums && tagArray[rand] == 1)
                {
                    rand = Random.Range(0, cardNums);
                }

                tagArray[rand] = 1;
                CardNumberArray[i] = rand;
            }
        }

        private void GenerateRandomCardNums2()
        {
            int cardNums = CardNums;
            int[] tagArray = new int[cardNums];
            int i = 0;
            for (i = 0; i < cardNums; i++)
            {
                tagArray[i] = 0;
            }

            for (i = 0; i < cardNums; i++)
            {
                int rand = Random.Range(0, cardNums);
                while (rand < cardNums && tagArray[rand] == 1)
                {
                    rand = Random.Range(0, cardNums);
                }

                tagArray[rand] = 1;
                CardNumberArray[i] = i;
                CardNumberArray[51] = 39;
                CardNumberArray[39] = 51;

                CardNumberArray[49] = 26;
                CardNumberArray[26] = 49;

                CardNumberArray[46] = 13;
                CardNumberArray[13] = 46;
            }
        }

        private void GenerateRandomCardNums3()
        {
            //Open Layer
            CardNumberArray[51] = 44;//T diamond
            CardNumberArray[49] = 39;
            CardNumberArray[46] = 26; //T club
            CardNumberArray[42] = 48;
            CardNumberArray[37] = 47;
            CardNumberArray[31] = 46;
            CardNumberArray[24] = 32;
            //Closed 1 Layer
            CardNumberArray[50] = 40;
            CardNumberArray[47] = 25;
            CardNumberArray[43] = 49;//J diamond
            CardNumberArray[38] = 41;
            CardNumberArray[32] = 51;
            CardNumberArray[25] = 29;//4 club
            //Closed 2 Layer
            CardNumberArray[48] = 38;
            CardNumberArray[44] = 42;//4 diamond
            CardNumberArray[39] = 28;
            CardNumberArray[33] = 35;
            CardNumberArray[26] = 34;
            //Closed 3 Layer
            CardNumberArray[45] = 33;
            CardNumberArray[40] = 45;
            CardNumberArray[34] = 31;
            CardNumberArray[27] = 17;
            //Closed 4 Layer
            CardNumberArray[41] = 43;//5 diamond
            CardNumberArray[35] = 36;//J club
            CardNumberArray[28] = 10;//D spade
            //Closed 5 Layer
            CardNumberArray[36] = 24; //D heart
            CardNumberArray[29] = 50; //D diamond
            //Closed 5 Layer
            CardNumberArray[30] = 37;//D diamond
            //Open Deck
            CardNumberArray[0] = 0;
            CardNumberArray[1] = 22;
            CardNumberArray[2] = 12;
            CardNumberArray[3] = 16;
            CardNumberArray[4] = 4;
            CardNumberArray[5] = 14;
            CardNumberArray[6] = 19;
            CardNumberArray[7] = 21;
            CardNumberArray[8] = 8;
            CardNumberArray[9] = 9;            
            CardNumberArray[10] = 27; //2 club
            CardNumberArray[11] = 11;
            CardNumberArray[12] = 2;
            CardNumberArray[13] = 13;
            CardNumberArray[14] = 5;
            CardNumberArray[15] = 15;
            CardNumberArray[16] = 3;
            CardNumberArray[17] = 30;
            CardNumberArray[18] = 18;
            CardNumberArray[19] = 6;
            CardNumberArray[20] = 20;
            CardNumberArray[21] = 7;
            CardNumberArray[22] = 1;
            CardNumberArray[23] = 23;
        }

        //Open cards 51, 49, 46, 42, 37, 31, 24
        //51 50 48 45 41 36 30
        //   49 47 44 40 35 29
        //      46 43 39 34 28
        //         42 38 33 27
        //            37 32 26
        //               31 25
        //                  24

        //51 - K diamond
        //50 - D diamond
        //49 - J diamond
        //48 - 10 diamond
        //47 - 9 diamond
        //46 - 8 diamond
        //45 - 7 diamond
        //44 - 6 diamond
        //43 - 5 diamond
        //42 - 4 diamond
        //41 - 3 diamond
        //40 - 2 diamond
        //39 - T diamond

        //38 - K club
        //37 - D club
        //36 - J club        
        //35 - 10 club
        //34 - 9 club
        //33 - 8 club
        //32 - 7 club
        //31 - 6 club
        //30 - 5 club
        //27 - 2 club
        //26 - T club

        //25 - K heart
        //24 - D heart
        //23 - J heart
        //22 - 10 heart
        //21 - 9 heart
        //19 - 7 heart
        //18 - 6 heart
        //17 - 5 heart
        //16 - 4 heart
        //15 - 3 heart
        //14 - 2 heart
        //13 - T heart

        //12 - K spade
        //11 - D spade
        //10 - J spade
        //1 - 2 spade
        //0 - T spade

        /// <summary>
        /// Randomize cards.
        /// </summary>
        public void InitSpecificCardNums(int[] numsArray)
        {
            CardNumberArray = numsArray;
        }

        public void SetHandOrientation()
        {
            DeckElements.ForEach(x =>
            {
                switch (Orientation)
                {
                    case HandOrientation.LEFT:
                        x.RectRoot.anchorMin = x.LeftTransformRef.anchorMin;
                        x.RectRoot.anchorMax = x.LeftTransformRef.anchorMax;
                        x.RectRoot.pivot = x.LeftTransformRef.pivot;
                        x.RectRoot.localScale = x.LeftTransformRef.localScale;
                        x.RectRoot.anchoredPosition = x.LeftTransformRef.anchoredPosition;
                        x.RectRoot.sizeDelta = x.LeftTransformRef.sizeDelta;
                        break;
                    case HandOrientation.RIGHT:
                        x.RectRoot.anchorMin = x.RightTransformRef.anchorMin;
                        x.RectRoot.anchorMax = x.RightTransformRef.anchorMax;
                        x.RectRoot.pivot = x.RightTransformRef.pivot;
                        x.RectRoot.localScale = x.RightTransformRef.localScale;
                        x.RectRoot.anchoredPosition = x.RightTransformRef.anchoredPosition;
                        x.RectRoot.sizeDelta = x.RightTransformRef.sizeDelta;
                        break;
                }
            });

            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                Deck targetDeck = AllDeckArray[i];
                targetDeck.UpdateCardsPosition(false);
            }
            HintManagerComponent.UpdateAvailableForDragCards();
        }

        /// <summary>
        /// Initialize cards in the game.
        /// </summary>
        private void InitCardNodes()
        {
            int cardNums = CardNums;
            for (int i = 0; i < cardNums; i++)
            {
                CardsArray[i].transform.SetParent(transform);
                CardsArray[i].InitWithNumber(i);
                CardsArray[i].CardLogicComponent = this;
            }
        }

        /// <summary>
        /// Initialize deck of cards.
        /// </summary>
        protected abstract void InitDeckCards();

        /// <summary>
        /// Initialize deck array.
        /// </summary>
        protected virtual void InitAllDeckArray()
        {
            int j = 0;
            for (int i = 0; i < AceDeckArray.Length; i++)
            {
                AceDeckArray[i].Type = DeckType.DECK_TYPE_ACE;
                AllDeckArray[j++] = AceDeckArray[i];
            }

            for (int i = 0; i < BottomDeckArray.Length; i++)
            {
                BottomDeckArray[i].Type = DeckType.DECK_TYPE_BOTTOM;
                AllDeckArray[j++] = BottomDeckArray[i];
            }

            if (WasteDeck != null)
            {
                WasteDeck.Type = DeckType.DECK_TYPE_WASTE;
                AllDeckArray[j++] = WasteDeck;
            }

            if (PackDeck != null)
            {
                PackDeck.Type = DeckType.DECK_TYPE_PACK;
                AllDeckArray[j++] = PackDeck;
            }

            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                AllDeckArray[i].DeckNum = i;
            }   
        }

        /// <summary>
        /// Call when we drop card.
        /// </summary>
        /// <param name="card">Dropped card</param>
        public abstract void OnDragEnd(Card card);

        /// <summary>
        /// Call when we click on pack with cards.
        /// </summary>
        public abstract void OnClickPack();

        /// <summary>
        /// Hide all cards from waste to pack.
        /// </summary>
        protected void MoveFromWasteToPack()
        {
            int cardNums = WasteDeck.CardsCount;
            for (int i = 0; i < cardNums; i++)
            {
                PackDeck.PushCard(WasteDeck.Pop());
            }

            PackDeck.UpdateCardsPosition(false);
            WasteDeck.UpdateCardsPosition(false);
            
            if (AudioCtrl != null)
            {
                AudioCtrl.Play(AudioController.AudioType.MoveToPack);
            }
        }

        /// <summary>
        /// Check for player win or no.
        /// </summary>
        protected virtual void CheckWinGame()
        {
            bool hasWin = true;
            for (int i = 0; i < AceDeckArray.Length; i++)
            {
                if (AceDeckArray[i].CardsCount != Public.CARD_NUMS_OF_SUIT)
                {
                    hasWin = false;
                    break;
                }
            }

            if (hasWin)
            {
                GameManagerComponent.HasWinGame();
                IsGameStarted = false;
            }
        }

        /// <summary>
        /// Call after each step.
        /// </summary>
        public void ActionAfterEachStep()
        {
            if (!_isUserMadeFirstMove)
            {
                _isUserMadeFirstMove = true;
            }

            SetPackDeckBg();
            GameManagerComponent.CardMove();

            if (AutoCompleteComponent.IsAutoCompleteActive)
            {
                HintManagerComponent.UpdateAvailableForAutoCompleteCards();
            }
            else
            {
                HintManagerComponent.UpdateAvailableForDragCards();
            }

            CheckWinGame();
        }

        /// <summary>
        /// Shuffle cards by state type.
        /// </summary>
        /// <param name="bReplay">Replay game or start new</param>
        public virtual void Shuffle(bool bReplay)
        {
            HintManagerComponent.IsHintWasUsed = false;
            IsNeedResetPack = false;
            GameManagerComponent.RestoreInitialState();
            RestoreInitialState();

            if (!bReplay)
            {
                GenerateRandomCardNums();
            }

            int cardNums = CardNums;
            for (int i = 0; i < cardNums; i++)
            {
                Card card = CardsArray[i];
                card.transform.position = card.StartPosition;
                card.InitWithNumber(CardNumberArray[i]);
#if UNITY_EDITOR
                string cardName = $"{card.GetTypeName()}_{card.Number} Index: ({card.CardNumber})"; 
                card.gameObject.name = $"CardHolder ({cardName})";
                card.BackgroundImage.gameObject.name = $"Card_{cardName}";
#endif
                PackDeck.PushCard(card);
            }

            InitDeckCards();
            SetPackDeckBg();
            HintManagerComponent.UpdateAvailableForDragCards();
            IsGameStarted = true;
        }

        /// <summary>
        /// Initialize default state of game.
        /// </summary>
        public void RestoreInitialState()
        {
            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                AllDeckArray[i].RestoreInitialState();
            }
        }

        /// <summary>
        /// Set up background of pack.
        /// </summary>
        protected virtual void SetPackDeckBg()
        {
            string name = _packNone;
            if (PackDeck != null && PackDeck.HasCards || WasteDeck != null && WasteDeck.HasCards)
            {
                name = _packRotate;
            }

            PackDeck.SetBackgroundImg(name);
        }

        /// <summary>
        /// Write state of current decks and cards.
        /// </summary>
        public void WriteUndoState(bool isTemp = false)
        {
            UndoPerformerComponent.AddUndoState(AllDeckArray, isTemp);
            UndoPerformerComponent.ActivateUndoButton();
        }

        /// <summary>
        /// If user made any changes with cards.
        /// </summary>
        public bool IsMoveWasMadeByUser()
        {
            return _isUserMadeFirstMove;
        }

        public Sprite LoadSprite(string path)
        {
            if(CachedSprtesDict.ContainsKey(path))
            {
                return CachedSprtesDict[path];
            }
            else
            {
                Sprite sprite = Resources.Load<Sprite>(path);
                
                CachedSprtesDict.Add(path, sprite);

                return sprite;
            }
        }

        public void SaveGameState(bool isTempState)
        {
            WriteUndoState(isTempState);
            UndoPerformerComponent.SaveGame(
                time: GameManagerComponent.TimeCount,
                steps: GameManagerComponent.StepCount,
                score: GameManagerComponent.ScoreCount
                );
        }
    }
}