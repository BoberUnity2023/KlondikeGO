#if GAME_PUSH
using GamePush;
#endif
using Newtonsoft.Json.Linq;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Screen;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
//using YG;

namespace SimpleSolitaire.Controller
{
    public enum Game
    {
        Klondike,
        Spider,
        Solitaire
    }

    public enum Platform
    { 
        Ok,
        Yandex,
        VK,        
        GD
    }


    public abstract class GameManager : MonoBehaviour
    {
        [SerializeField] private Game _game;
        [SerializeField] private Platform _platform;
        [Header("Serialized fields:")]
        [SerializeField]
        private RectTransform _bottomPanel;
        [SerializeField]
        private Animator _settingsPanelAnimator;
        [SerializeField]
        protected GameObject _buttonHint;
        [SerializeField]
        private GameObject _buttonRewardedVideo;
        [SerializeField]
        private ToggleGroup _levelToggleGroup;
        [SerializeField]
        private GameObject _bonusGold;

        [Header("Ads Components:")]        
        [SerializeField]
        private GameObject _adsLayer;
        [SerializeField]
        private GameObject _adsLayerHints;
        [SerializeField]
        private GameObject _watchButton;
        [SerializeField]
        private Text _adsInfoText;
        [SerializeField]
        private Text _adsDidNotLoadText;
        [SerializeField]
        private Text _adsClosedTooEarlyText;

        public string NoAdsInfoText = "DO YOU WANNA TO DEACTIVATE ALL ADS FOR THIS GAME SESSION? JUST WATCH LAST REWARD VIDEO AND INSTALL APP. THEN ADS WON'T DISTURB YOU AGAIN!";
        public string GetUndoAdsInfoText = "DO YOU WANNA TO GET FREE UNDO COUNTS? JUST WATCH REWARD VIDEO AND INSTALL APP. THEN UNDO WILL ADDED TO YOUR GAME SESSION!";

        [Header("Components:")]
        [SerializeField]
        protected CardLogic _cardLogic;
        [SerializeField]        
        private KlondikeCardLogic _klondikeCardLogic;
        [SerializeField]
        private InterVideoAds _interVideoAdsComponent;
        [SerializeField]
        private CongratulationManager _congratManagerComponent;
        [SerializeField]
        protected UndoPerformer _undoPerformComponent;
        [SerializeField]
        private TutorialManager _tutorialComponent;
        [SerializeField]
        private AutoCompleteManager _autoCompleteComponent;
        //[SerializeField]
        //public LeaderboardYG _leaderboardYG;
        [SerializeField]
        protected HintManager _hintManager;

        public ScreenController ScreenController;

        public TabLeaderboard TabLeaderboard;

        [SerializeField]
        private AdsController _adsController;

        [Header("Layers:")]
        [SerializeField]
        protected GameObject _newGameLayer;
        [SerializeField]
        protected GameObject _newGameLayerBlocker;
        [SerializeField]
        protected GameObject _cardLayer;
        [SerializeField]
        private GameObject _winLayer;
        [SerializeField]
        private GameObject _settingLayer;
        [SerializeField]
        private GameObject _visualLayer;
        [SerializeField]
        private GameObject _shopLayer;        
        [SerializeField]
        private GameObject _statisticLayer;
        [SerializeField]
        private GameObject _exitLayer;
        [SerializeField]
        private GameObject _continueLayer;
        [SerializeField]
        private GameObject _tutorialLayer;
        [SerializeField]
        private GameObject _leaderboardLayer;
        [SerializeField]
        private BuyWindow _buyWindow;
        [SerializeField]
        private GameObject _restartWindow;
        [SerializeField]
        private GameObject _clickBlocker;
        [SerializeField]
        private GameObject _clickBlockerPanels;
        [SerializeField]
        private GameObject _everyDayBonusWindow;

        public Text DebugLayer;

        [Header("Labels:")]
        [SerializeField]
        private Text _timeLabel;
        [SerializeField]
        private Text _scoreLabel;
        [SerializeField]
        private Text _goldLabel;        
        [SerializeField]
        private Text _stepsLabel;
        [SerializeField]
        private Text _timeWinLabel;
        [SerializeField]
        private Text _scoreWinLabel;
        [SerializeField]
        private Text _stepsWinLabel;

        [Header("Switchers:")]
        [SerializeField]
        private SwitchSpriteComponent _soundSwitcher;        

        [Space(5f)]
        [SerializeField]
        private TableLayoutGroup _settingsRef;

        [Header("Settings:")]
        public bool UseLoadLastGameOption;

        [SerializeField]
        private int[] MoveFromWasteToPackPrices;//Штраф за переворот колоды

        public GamePushController GamePush { get; set; }

        public int TimeCount => _timeCount;
        public int StepCount => _stepCount;
        public int ScoreCount => _scoreCount;

        public int TimeBonus => _timeBonus;
        public int Gold
        {
            get
            {
                //_gold = PlayerPrefs.GetInt(_goldKey);
                //if (Platform == Platform.Yandex) 
                //    _gold = YandexGame.savesData.Gold;

                if (Platform == Platform.Ok ||
                    Platform == Platform.VK ||
                    Platform == Platform.GD)
                    _gold = PlayerPrefs.GetInt("Gold");                

                return _gold;
            }
            set
            {
                _gold = Mathf.Max(0, value);
                _goldLabel.text = _gold.ToString();

                if (Platform == Platform.Ok ||
                    Platform == Platform.VK ||
                    Platform == Platform.GD)
                { 
                    PlayerPrefs.SetInt("Gold", _gold);
                    PlayerPrefs.Save();
                }                

                if (Platform == Platform.Yandex)
                {
                    //YandexGame.savesData.Gold = _gold;
                    //YandexGame.SaveProgress();
                }                    
            }
        }

        public int GoldForParty { get; set; }

        public int Level => _level;

        public int MoveFromWasteToPackPrice
        {
            get
            {
                return MoveFromWasteToPackPrices[Level];
            }
        }        

        public Game Game => _game;

        public Platform Platform => _platform;

        private readonly string _appearTrigger = "Appear";
        private readonly string _disappearTrigger = "Disappear";
        //private readonly string _bestScoreKey = "WinBestScore";
        private readonly string _showBottomBarKey = "ShowBar";

        private int _timeCount;
        private int _stepCount;
        private int _scoreCount;
        private int _timeBonus;
        private int _gold;
        private int _level;
        private int _levelSelected;

        private Coroutine _timeCoroutine;
        private AudioController _audioController;

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        private bool _highlightDraggableEnable;
        private bool _soundEnable;
        private bool _autoCompleteEnable;
        private bool _isBarActive;

        protected float _windowAnimationTime = 0.42f;

        private void Awake()
        {
            InitializeGame();
        }

        /// <summary>
        /// Initialize game structure.
        /// </summary>
        protected virtual void InitializeGame()
        {
            Application.targetFrameRate = 300;

            _soundEnable = true;
            _autoCompleteEnable = true;
            _isBarActive = true;

            InterVideoAds.RewardAction += OnRewardActionState;

            _cardLogic.SubscribeEvents();
            _audioController = AudioController.Instance;
            //_settingsRef.StartCorner = _cardLogic.Orientation == HandOrientation.RIGHT ? TableLayoutGroup.Corner.UpperLeft : TableLayoutGroup.Corner.UpperRight;
            _goldLabel.text = Gold.ToString();
        }

        private void Start()
        {
            if (Game == Game.Klondike)
            {
                _levelSelected = 1;
                SetLevel(1);
            }

            if (Game == Game.Spider)
            {
                _levelSelected = 0;
                SetLevel(0);
            }

            StartCoroutine(InitGameState());
        }

        /// <summary>
        /// Appear window with animation.
        /// </summary>
        protected void AppearWindow(GameObject window)
        {
            if (window == null)
            {
                return;
            }

            var anim = window.GetComponent<Animator>();

            if (anim == null)
            {
                return;
            }

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.WindowOpen);
            }

            anim.SetTrigger(_appearTrigger);
        }

        /// <summary>
        /// Disappear window with animation.
        /// </summary>
        protected void DisappearWindow(GameObject window, Action onDisappear)
        {
            if (window == null)
            {
                return;
            }

            var anim = window.GetComponent<Animator>();

            if (anim == null)
            {
                return;
            }

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.WindowClose);
            }

            anim.SetTrigger(_disappearTrigger);

            StartCoroutine(InvokeAction(onDisappear, _windowAnimationTime));
        }

        /// <summary>
        /// Show tutorial window with animation.
        /// </summary>
        public void ShowTutorialLayer()
        {
            DisappearWindow(_newGameLayer, OnModalLayerDisappeared);

            void OnModalLayerDisappeared()
            {
                _newGameLayer.SetActive(false);
                _tutorialLayer.SetActive(true);
                AppearWindow(_tutorialLayer);
            }            
        }

        /// <summary>
        /// Hide tutorial layer with animation.
        /// </summary>
        public void HideTutorialLayer()
        {
            DisappearWindow(_tutorialLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _tutorialLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy);
                OnClickPlayBtn();
            }
        }

        /// <summary>
        /// Init new game state or show continue game panel.
        /// </summary>
        private IEnumerator InitGameState()
        {
            yield return new WaitForEndOfFrame();
            if (UseLoadLastGameOption && _tutorialComponent.IsHasKey() && _undoPerformComponent.IsHasGame())
            {
                _cardLayer.SetActive(false);
                _continueLayer.SetActive(true);
                AppearWindow(_continueLayer);
            }
            else
            {
                _cardLogic.InitCardLogic();
                _cardLogic.Shuffle(false);
                InitMenuView(false);
                _cardLayer.SetActive(false);
                _newGameLayer.SetActive(true);
                _newGameLayerBlocker.SetActive(true);
                AppearWindow(_newGameLayer);
            }
        }

        /// <summary>
        /// Change position of bottom panel. Used for ads banner.
        /// </summary>
        /// <param name="offset"></param>
        public void InitializeBottomPanel(float offset)
        {
            _bottomPanel.anchoredPosition = new Vector2(0, offset);
        }

        /*public void OnNoAdsRewardedUser()
        {
            InitializeBottomPanel(0);
            OnClickAdsCloseBtn();
            AdsBtn.SetActive(false);
        }*/

        private void OnDestroy()
        {
            InterVideoAds.RewardAction -= OnRewardActionState;
            _cardLogic.UnsubscribeEvents();
        }

        /// <summary>
        /// Initialize first state of UI elements. And first timer state.
        /// </summary>
        private void InitMenuView(bool isLoadGame)
        {
            _timeCount = isLoadGame ? _undoPerformComponent.StatesData.Time : 0;
            SetTimeLabel(_timeCount);
            _stepCount = isLoadGame ? _undoPerformComponent.StatesData.Steps : 0;
            _stepsLabel.text = _stepCount.ToString();
            _scoreCount = isLoadGame ? _undoPerformComponent.StatesData.Score : 0;
            _scoreLabel.text = _scoreCount.ToString();
            StopGameTimer();
        }        

        /// <summary>
        /// Update <see cref="_timeLabel"/> view text.
        /// </summary>
        /// <param name="seconds"></param>
        private void SetTimeLabel(int seconds)
        {
            int sec = seconds % 60;
            int min = (seconds % 3600) / 60;
            _timeLabel.text = string.Format("{0,2}:{1,2}", min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// Win game action.
        /// </summary>
        public void HasWinGame()
        {
            _timeBonus = Public.SCORE_NUMBER / _timeCount;

            var score = _scoreCount + _timeBonus;

            if (Game == Game.Klondike)
                GoldForParty = score * (Level + 1) / 10;
            if (Game == Game.Spider)
                GoldForParty = score * 3 * (Level + 1) / 10;
            if (Game == Game.Solitaire)
                GoldForParty = score * 3 * (Level + 1) / 10;

            Gold += GoldForParty;
            SetBestValuesToPrefs(score);

            StopGameTimer();
            _congratManagerComponent.CongratulationTextFill();

            _timeWinLabel.text = _timeLabel.text;//"YOUR TIME: "
            _scoreWinLabel.text = score.ToString();//"YOUR SCORE: "
            _stepsWinLabel.text = _stepCount.ToString();//"YOUR MOVES: "

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.Win);
            }

            StatisticsController statisticsController = StatisticsController.Instance;

            statisticsController.PlayedGamesTime?.Invoke(_timeCount);
            statisticsController.AverageTime?.Invoke();
            statisticsController.IncreaseScore?.Invoke(score);
            statisticsController.IncreaseWonGames?.Invoke();
            statisticsController.BestTime?.Invoke(_timeCount);
            statisticsController.BestMoves?.Invoke(_stepCount);

            int scoreForAllGames = PlayerPrefs.GetInt("ScoreForAllGames");
            //YandexGame.savesData.ScoreForAllGames += score;
            PlayerPrefs.SetInt("ScoreForAllGames", scoreForAllGames + score);
            PlayerPrefs.Save();
#if GAME_PUSH
            GP_Player.SetScore(scoreForAllGames + score);
            GP_Player.Sync(SyncStorageType.cloud);
            //Debug.Log("ScoreForAllGames: " + YandexGame.savesData.ScoreForAllGames);
            //YandexGame.NewLeaderboardScores("advanced", YandexGame.savesData.ScoreForAllGames);
#endif
            StartCoroutine(AfterWin(2.5f));

            //_interVideoAdsComponent.ShowInterstitial();
        }

        private IEnumerator AfterWin(float time)
        {
            yield return new WaitForSeconds(time);
            _cardLayer.SetActive(false);
            _winLayer.SetActive(true);            
            AppearWindow(_winLayer);            
        }

        /// <summary>
        /// Save to prefs best score if it need :)
        /// </summary>
        /// <param name="score">Score value</param>
        private void SetBestValuesToPrefs(int score)
        {
            if (score > PlayerPrefs.GetInt("BestScore")/*YandexGame.savesData.BestScore*/)
            {
                PlayerPrefs.SetInt("BestScore", score);
                PlayerPrefs.Save();
                //YandexGame.savesData.BestScore = score;
                //YandexGame.SaveProgress();
            }

            //if (!PlayerPrefs.HasKey(_bestScoreKey))
            //{
            //	PlayerPrefs.SetInt(_bestScoreKey, score);
            //}
            //else
            //{
            //	if (score > PlayerPrefs.GetInt(_bestScoreKey))
            //	{
            //		PlayerPrefs.SetInt(_bestScoreKey, score);					
            //  }
            //}
        }

        /// <summary>
        /// Click on new game button.
        /// </summary>
        public void OnClickWinNewGame()
        {
            _winLayer.SetActive(false);
            _cardLayer.SetActive(!_statisticLayer.activeInHierarchy && !_winLayer.activeInHierarchy);
            /*DisappearWindow(_winLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _winLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy && !_ruleLayer.activeInHierarchy && !_winLayer.activeInHierarchy);
            }*/

            _cardLogic.Shuffle(false);
            _undoPerformComponent.ResetUndoStates();
            _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[Level];
            StatisticsController.Instance.PlayedGames?.Invoke();            
        }

        /// <summary>
        /// Click on play button in bottom setting layer.
        /// </summary>
        public void OnClickPlayBtn()
        {
            _cardLayer.SetActive(false);            

            if (_cardLogic is KlondikeCardLogic klondikeLogic)
            {
                klondikeLogic.InitRuleToggles();
            }
            else if (_cardLogic is SpiderCardLogic spiderLogic)
            {
                spiderLogic.InitSuitsToggles();
            }
            else if (_cardLogic is FreecellCardLogic freecellLogic)
            {
                freecellLogic.InitFreecellToggles();
            }            

            _levelToggleGroup.SetAllTogglesOff();
            Toggle[] toggles = _levelToggleGroup.GetComponentsInChildren<Toggle>();
            toggles[Level].isOn = true;
            AppearGameLayer();            
        }

        public void OnClickLevel(int level)
        {
            _levelSelected = level;
        }

        protected void AppearGameLayer()
        {
            _newGameLayer.SetActive(true);
            _newGameLayerBlocker.SetActive(true);
            InitCardLogic();
            AppearWindow(_newGameLayer);
        }

        public void OnClickLeaderboardBtn()
        {
            _cardLayer.SetActive(false);
            _leaderboardLayer.SetActive(true);

            if (_cardLogic is KlondikeCardLogic klondikeLogic)
            {
                klondikeLogic.InitRuleToggles();
            }
            else if (_cardLogic is SpiderCardLogic spiderLogic)
            {
                spiderLogic.InitSuitsToggles();
            }
            else if (_cardLogic is FreecellCardLogic freecellLogic)
            {
                freecellLogic.InitFreecellToggles();
            }
            AppearWindow(_leaderboardLayer);
        }

        public void OnClickTryBuyBtn(VisualiseElement element)
        {
            Debug.Log("OnClickTryBuyBtn Name: " + element.ElementName + "; Price: " + element.Price);

            _buyWindow.gameObject.SetActive(true);
            _buttonRewardedVideo.gameObject.SetActive(false);
            _buyWindow.Set(element);
            AppearWindow(_buyWindow.gameObject);
        }

        public void OnClickTryBuyCloseBtn()
        {
            //DisappearWindow(_buyWindow.gameObject);
            DisappearWindow(_buyWindow.gameObject, OnWindowDisappeared);
            void OnWindowDisappeared()
            {
                //_winLayer.SetActive(false);
                //_cardLayer.SetActive(!_statisticLayer.activeInHierarchy && !_ruleLayer.activeInHierarchy && !_winLayer.activeInHierarchy);
            }
            StartCoroutine(InvokeAction(delegate
            {
                _buyWindow.gameObject.SetActive(false);
                _buttonRewardedVideo.gameObject.SetActive(true);
            }, 0.42f));
        }

        public void Buy(VisualiseElement element)
        {
            Gold -= element.Price;
            element.Buy();

            DisappearWindow(_buyWindow.gameObject, OnWindowDisappeared);
            void OnWindowDisappeared()
            {
                _winLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy && !_winLayer.activeInHierarchy);
            }
            StartCoroutine(InvokeAction(delegate
            {
                _buyWindow.gameObject.SetActive(false);
                _buttonRewardedVideo.gameObject.SetActive(true);
            }, 0.42f));
        }

        public void OnClickTryRestartBtn()
        {
            _restartWindow.SetActive(true);            
            AppearWindow(_restartWindow);
        }

        public void OnClickTryRestartCloseBtn()
        {
            DisappearWindow(_restartWindow, OnWindowDisappeared);
            void OnWindowDisappeared()
            {
                _winLayer.SetActive(false);                
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy && !_winLayer.activeInHierarchy);
            }
            StartCoroutine(InvokeAction(delegate
            {
                _restartWindow.SetActive(false);                
            }, 0.42f));
        }

        public void ShowEveryDayBonusWindow()
        {            
            _everyDayBonusWindow.SetActive(true);
            AppearWindow(_everyDayBonusWindow);
        }

        public void OnClickEveryDayBonusCloseBtn()
        {
            DisappearWindow(_everyDayBonusWindow, OnWindowDisappeared);
            void OnWindowDisappeared()
            {
                
            }
            StartCoroutine(InvokeAction(delegate
            {
                _everyDayBonusWindow.SetActive(false);
            }, 0.42f));
        }

        protected abstract void InitCardLogic();

        #region Continue Layer
        /// <summary>
        /// Click on play button in bottom setting layer.
        /// </summary>
        private void LoadGame()
        {  
            _cardLogic.InitCardLogic();

            _undoPerformComponent.LoadGame();

            _cardLogic.OnNewGameStart();

            InitMenuView(true);
        }

        /// <summary>
        /// Start new game.
        /// </summary>
        public void OnClickContinueNoBtn()
        {
            DisappearWindow(_continueLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                //Uncomment if you wanna clear last game when User click No button on Continue Layer.
                //_undoPerformComponent.DeleteLastGame();
                _cardLogic.InitCardLogic();
                _cardLogic.Shuffle(false);
                _continueLayer.SetActive(false);
                _cardLayer.SetActive(true);
            }
        }

        /// <summary>
        /// Continue last game.
        /// </summary>
        public void OnClickContinueYesBtn()
        {
            DisappearWindow(_continueLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                LoadGame();
                _continueLayer.SetActive(false);
                _cardLayer.SetActive(true);
            }
        }
        #endregion

        #region Exit Layer
        /// <summary>
        /// Click on Exit button.
        /// </summary>
        public void OnClickExitBtn()
        {
            _cardLayer.SetActive(false);
            _exitLayer.SetActive(true);
            AppearWindow(_exitLayer);
        }

        /// <summary>
        /// Close <see cref="_adsLayer"/>.
        /// </summary>
        public void OnClickExitNoBtn()
        {
            DisappearWindow(_exitLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _exitLayer.SetActive(false);
                _cardLayer.SetActive(true);
            }
        }

        /// <summary>
        /// Quit application. Exit game.
        /// </summary>
        public void OnClickExitYesBtn()
        {
            DisappearWindow(_exitLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
#if UNITY_EDITOR
                _cardLogic.SaveGameState(isTempState: true);
                EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
            }
        }

        #endregion

        #region Ads Layer

        /// <summary>
        /// Click on NoAds button.
        /// </summary>
        public void OnClickGetUndoAdsBtn()
        {
            _currentAdsType = RewardAdsType.GetUndo;
            ShowAdsLayer();
        }

        /// <summary>
        /// Click on NoAds button.
        /// </summary>
        public void OnClickNoAdsBtn()
        {
            _currentAdsType = RewardAdsType.NoAds;
            ShowAdsLayer();
        }

        /// <summary>
        /// Appearing ads layer with information about ads type.
        /// </summary>
        private void ShowAdsLayer()
        {
            UpdateAdsInfoText(_currentAdsType);

            //_cardLayer.SetActive(false);
            _adsLayer.SetActive(true);
            _adsInfoText.enabled = true;
            _adsDidNotLoadText.enabled = false;
            _adsClosedTooEarlyText.enabled = false;
            _watchButton.SetActive(true);
            AppearWindow(_adsLayer);
        }

        public void OnClickNoHinsButton()
        {
            ShowAdsLayerHint();
        }

        private void ShowAdsLayerHint()
        {
            _adsLayerHints.SetActive(true);  
            AppearWindow(_adsLayerHints);
        }

        /// <summary>
        /// Close <see cref="_adsLayer"/>.
        /// </summary>
        public void OnClickAdsCloseBtn()
        {
            DisappearWindow(_adsLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _adsLayer.SetActive(false);
                //_cardLayer.SetActive(true);
            }
        }

        public void OnClickAdsHintCloseBtn()
        {
            DisappearWindow(_adsLayerHints, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _adsLayerHints.SetActive(false);
                //_cardLayer.SetActive(true);
            }
        }        

        /// <summary>
        /// Close <see cref="_adsLayer"/>.
        /// </summary>
        public void OnWatchAdsBtnClick()
        {
            switch (_currentAdsType)
            {
                case RewardAdsType.GetUndo:
                    _interVideoAdsComponent.ShowGetUndoAction();
                    break;
                case RewardAdsType.NoAds:
                    _interVideoAdsComponent.NoAdsAction();
                    break;
            }
        }

        /// <summary>
        /// Call result of watched reward video.
        /// </summary>
        public void OnRewardActionState(RewardAdsState state, RewardAdsType type)
        {
            DisappearWindow(_adsLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                bool infoText = false;
                bool closedText = false;
                bool notLoadedText = false;
                switch (state)
                {
                    case RewardAdsState.TOO_EARLY_CLOSE:
                        closedText = true;
                        break;
                    case RewardAdsState.DID_NOT_LOADED:
                        notLoadedText = true;
                        break;
                }
                _adsLayer.SetActive(true);
                _adsInfoText.enabled = infoText;
                _adsDidNotLoadText.enabled = notLoadedText;
                _adsClosedTooEarlyText.enabled = closedText;
                _watchButton.SetActive(false);
                _cardLayer.SetActive(false);
                AppearWindow(_adsLayer);
            }
        }

        public void UpdateAdsInfoText(RewardAdsType type)
        {
            switch (type)
            {
                case RewardAdsType.NoAds:
                    _adsInfoText.text = NoAdsInfoText;
                    break;
                case RewardAdsType.GetUndo:
                    _adsInfoText.text = GetUndoAdsInfoText;
                    break;
            }
        }

        public void HideAdsLayer()
        {
            _adsLayer.SetActive(false);
        }

        public void HideAdsLayerHints()
        {
            _adsLayerHints.SetActive(false);
        }
        #endregion

        #region Rule Layer
        /// <summary>
        /// Click on rule button.
        /// </summary>
        public void OnClickSettingLayerRuleBtn()
        {
            //StartCoroutine(InvokeAction(delegate { OnClickSettingLayerCloseBtn(); Invoke(nameof(OnRuleAppearing), _windowAnimationTime); }, 0f));
        }        
        #endregion

        #region Settings Layer
        /// <summary>
        /// Click on settings button.
        /// </summary>
        public void OnClickSettingBtn()
        {
            DisappearWindow(_newGameLayer, OnModalLayerDisappeared);

            void OnModalLayerDisappeared()
            {
                _newGameLayer.SetActive(false);
                //_cardLayer.SetActive(true);

                //_cardLayer.SetActive(false);
                _settingLayer.SetActive(true);
                AppearWindow(_settingLayer);
            }                       
        }

        /// <summary>
        /// Close <see cref="_settingLayer"/>.
        /// </summary>
        public void OnClickSettingLayerCloseBtn()
        {
            DisappearWindow(_settingLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _settingLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy);
                OnClickPlayBtn();
            }
        }

        public void OnClickVisualBtn()
        {
            _cardLayer.SetActive(false);
            _visualLayer.SetActive(true);
            AppearWindow(_settingLayer);
        }        
        public void OnClickVisualLayerCloseBtn()
        {
            DisappearWindow(_visualLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _visualLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy);
            }
        }

        public void OnClickShoplBtn()
        {
            _cardLayer.SetActive(false);
            _shopLayer.SetActive(true);
            AppearWindow(_shopLayer);
        }
        public void OnClickShopLayerCloseBtn()
        {
            DisappearWindow(_shopLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _shopLayer.SetActive(false);
                _cardLayer.SetActive(!_statisticLayer.activeInHierarchy);
            }
        }
        #endregion

        #region Statistics Layer
        /// <summary>
        /// Click on statistics button.
        /// </summary>
        public void OnClickStatisticBtn()
        {
            StartCoroutine(InvokeAction(delegate { OnClickSettingLayerCloseBtn(); Invoke(nameof(OnStatisticAppearing), _windowAnimationTime); }, 0f));
        }

        /// <summary>
        /// Call animation which appear statistics popup.
        /// </summary>
        private void OnStatisticAppearing()
        {
            _statisticLayer.SetActive(true);
            AppearWindow(_statisticLayer);
        }

        /// <summary>
        /// Close <see cref="_statisticLayer"/>.
        /// </summary>
        public void OnClickStatisticLayerCloseBtn()
        {
            DisappearWindow(_statisticLayer, OnStatisticsLayerClosed);
        }

        protected virtual void OnStatisticsLayerClosed()
        {
            _statisticLayer.SetActive(false);
            OnClickSettingBtn();
        }
        #endregion

        #region Game Layer
        /// <summary>
        /// Click on random button.
        /// </summary>
        public void OnClickModalRandom()
        {
            DisappearWindow(_newGameLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                SetLevel(_levelSelected);
                StatisticsController.Instance.PlayedGames?.Invoke();                
                _cardLogic.OnNewGameStart();
                _newGameLayer.SetActive(false);
                _newGameLayerBlocker.SetActive(false);
                _cardLayer.SetActive(true);
                _cardLogic.Shuffle(false);
                _winLayer.SetActive(false);
                if (Game == Game.Spider)
                    StartCoroutine(AfterStartEffectPlayed(2.5f));
                if (Game == Game.Klondike || Game == Game.Solitaire)
                {
                    _hintManager.GenerateHints();
                    ScreenController.Active = true;
                }
                _undoPerformComponent.ResetUndoStates();
                _adsController.TryShowInterstitial();                
            }
        }

        private IEnumerator AfterStartEffectPlayed(float time)
        {            
            _clickBlocker.SetActive(true);
            _clickBlockerPanels.SetActive(true);
            ScreenController.Active = false;
            yield return new WaitForSeconds(time);
            ScreenController.Active = true;
            _clickBlocker.SetActive(false);
            _clickBlockerPanels.SetActive(false);
            _hintManager.GenerateHints();
            StartCoroutine(AfterStartEffectPlayedUpdate(0.2f));
        }

        private IEnumerator AfterStartEffectPlayedUpdate(float time)
        {            
            yield return new WaitForSeconds(time);
            ScreenController.NeedUpdate = true;
        }

        /// <summary>
        /// Click on replay button.
        /// </summary>
        public void OnClickModalReplay()
        {
            DisappearWindow(_newGameLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                StatisticsController.Instance.PlayedGames?.Invoke();
                _cardLogic.OnNewGameStart();
                _newGameLayer.SetActive(false);
                _newGameLayerBlocker.SetActive(false);
                _winLayer.SetActive(false);
                _cardLayer.SetActive(true);
                _cardLogic.Shuffle(true);
                //StartCoroutine(AfterStartEffectPlayed(2.5f));
                _undoPerformComponent.ResetUndoStates();
                _adsController.TryShowInterstitial();
            }
        }

        /// <summary>
        /// Close <see cref="_newGameLayer"/>.
        /// </summary>
        public void OnClickModalClose()
        {
            DisappearWindow(_newGameLayer, OnModalLayerDisappeared);

            void OnModalLayerDisappeared()
            {
                _newGameLayer.SetActive(false);
                _newGameLayerBlocker.SetActive(false);
                _cardLayer.SetActive(true);
            }
        }    

        public void OnClickLeaderboardClose()
        {
            DisappearWindow(_leaderboardLayer, OnLeaderboardLayerDisappeared);
            StartCoroutine(InvokeAction(delegate { _leaderboardLayer.SetActive(false); _cardLayer.SetActive(true); }, 0.42f));
        }

        protected virtual void OnLeaderboardLayerDisappeared()
        {
            _leaderboardLayer.SetActive(false);
            _cardLayer.SetActive(true);
        }
        #endregion

        /// <summary>
        /// Call action via _time.
        /// </summary>
        /// <param name="action">Delegate.</param>
        /// <param name="time">Time for invoke.</param>
        /// <returns></returns>
        protected IEnumerator InvokeAction(Action action, float time)
        {
            yield return new WaitForSeconds(time);

            action?.Invoke();
        }

        /// <summary>
        /// Increase <see cref="_stepCount"/> value and start timer <see cref="GameTimer"/> if count == 1.
        /// </summary>
        public void CardMove()
        {
            _stepCount++;
            StatisticsController.Instance.Moves?.Invoke();

            _stepsLabel.text = _stepCount.ToString();
            if (_stepCount >= 1 && _timeCoroutine == null)
            {
                _timeCoroutine = StartCoroutine(GameTimer());
            }
        }

        /// <summary>
        /// Reset all view and states.
        /// </summary>
        public void RestoreInitialState()
        {
            InitMenuView(false);
        }

        /// <summary>
        /// Update score value <see cref="_scoreCount"/> and view text <see cref="_scoreLabel"/> on UI. 
        /// </summary>
        /// <param name="value">Score</param>
        public void AddScoreValue(int value)
        {
            _scoreCount += value;
            if (_scoreCount < 0)
            {
                _scoreCount = 0;
            }
            _scoreLabel.text = _scoreCount.ToString();
        }

        /// <summary>
        /// Click on sound switch button.
        /// </summary>
        public void OnClickSoundSwitch()
        {
            _soundEnable = !_soundEnable;
            _soundSwitcher.UpdateSwitchImg(_soundEnable);

            if (_audioController != null)
            {
                _audioController.SetMute(!_soundEnable);
            }
        }   

        /// <summary>
        /// Start game timer.
        /// </summary>
        private IEnumerator GameTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                _timeCount++;
                if (_timeCount % 30 == 0)
                {
                    AddScoreValue(Public.SCORE_OVER_THIRTY_SECONDS_DECREASE);
                }
                SetTimeLabel(_timeCount);
            }
        }

        /// <summary>
        /// Stop game timer.
        /// </summary>
        private void StopGameTimer()
        {
            if (_timeCoroutine != null)
            {
                StopCoroutine(_timeCoroutine);
                _timeCoroutine = null;
            }
        }

        public void OnApplicationFocus(bool state)
        {
            if (!_cardLogic.IsGameStarted)
            {
                Debug.Log($"Game does not started.");
                return;
            }

            if (!state)
            {
                //_cardLogic.SaveGameState(isTempState: true);
            }
            else
            {
                //_undoPerformComponent.Undo(removeOnlyState: true);
            }
        }

        public void OnApplicationPause(bool state)
        {
            if (!_cardLogic.IsGameStarted)
            {
                Debug.Log($"Game does not started.");
                return;
            }

            if (state)
            {
                //_cardLogic.SaveGameState(isTempState: true);
            }
            else
            {
                //_undoPerformComponent.Undo(removeOnlyState: true);
            }
        }

        /// <summary>
        /// Show/hide bottoms bar with animation.
        /// </summary>
        public void TryShowBar()
        {
            _isBarActive = !_isBarActive;
            _settingsPanelAnimator.SetBool(_showBottomBarKey, _isBarActive);
        }

        public void PressAddGold()
        {
            Gold += 100;
        }

        public void PressClearPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public virtual void SetLevel(int level)
        {
            _level = level;
            _levelSelected = level;
            Debug.Log("Difficulty Level: " + level);            
        }

        public void PressClearDebug()
        {
            DebugLayer.text = "";
        }

        public void PressResetSaveProgress()
        {
            //YandexGame.ResetSaveProgress();
        }

        public void AddBonusGold()
        {
            GameObject bonusObject = Instantiate(_bonusGold, ScreenController.CanvasScaler.transform);
            Bonus bonus = bonusObject.GetComponent<Bonus>();
            bonus.SetIndicatorors(Level);
            StartCoroutine(AfterAddBonusGold(1));
            _audioController.Play(AudioController.AudioType.Bonus);
        }

        private IEnumerator AfterAddBonusGold(float time)
        {
            yield return new WaitForSeconds(time);
            Gold += 100 * (Level + 1);
            
        }
    }
}