using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using I2.Loc;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class ReviveEvent { }

    public class UIResultPanel : UIPanelBase
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _continueWithAdsButton;
        [SerializeField] private Button _continueWithoutAdsButton;
        [SerializeField] private GameObject _newRecordTitle;
        [SerializeField] private GameObject[] _stars;
        [SerializeField] private TextMeshProUGUI _recordText;
        [SerializeField] private TextMeshProUGUI _currentScoreText;

        private GameModeController _gameModeController;
        private bool _isWaitingAddScore;

        protected override void Awake()
        {
            base.Awake();

            _gameModeController = FindAnyObjectByType<GameModeController>();
            _restartButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_result_restart_button");

                AdsController.ShowInterstitial();
                Restart();
            });

            _continueWithAdsButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_result_revive_button");
                ContinueWithAds();
            });

            _continueWithoutAdsButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_result_restart_button");

                Restart();
            });
        }

        protected override void Open()
        {
            base.Open();

            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;

            bool newRecord = gameModeState.Score > gameState.Stats.MaxScore;
            foreach (var star in _stars)
                star.SetActive(newRecord);

            if (newRecord)
            {
                Vibration.Vibrate(70);
                AudioController.Play("result_positive");
                AnalyticsController.SendEvent("result_positive");
            }
            else
            {
                AudioController.Play("result_negative");
                AnalyticsController.SendEvent("result_negative");
            }

            _restartButton.gameObject.SetActive(gameModeState.ContinueGameCount <= 0);
            _continueWithAdsButton.gameObject.SetActive(gameModeState.ContinueGameCount <= 0);
            _continueWithoutAdsButton.gameObject.SetActive(gameModeState.ContinueGameCount > 0);

            _newRecordTitle.SetActive(newRecord);

            _recordText.text = $"{LocalizationManager.GetTranslation("Main/record_score")} {gameState.Stats.MaxScore}";
            _currentScoreText.text = $"{LocalizationManager.GetTranslation("Main/current_score")} {gameModeState.Score}";

#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStop();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStop();
#endif
        }


        private void Restart()
        {
            Close();

            var gameModeState = SaveManager.GameModeState;
            _gameModeController.StartGame(gameModeState.Type, true);
        }

        private void ContinueWithAds()
        {
            AdsController.ShowRewarded((success) =>
            {
                if (success)
                {
                    var gameModeState = SaveManager.GameModeState;
                    gameModeState.ContinueGameCount++;

                    Close();

                    EventsManager.Publish(new ReviveEvent());
                }
            });
        }

        private void ContinueWithoutAds()
        {
            Close();

            SaveManager.SetGameModeState(null);
            EventsManager.Publish(new OpenPanelEvent(UIPanelType.GamemodeChoice));
        }

        protected override void Close()
        {
            base.Close();

            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;

            if (gameState.Stats.MaxScore < gameModeState.Score)
            {
                NewRecord();
            }

#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStart();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStart();
#endif
        }

        private void NewRecord()
        {
            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;

            gameState.Stats.MaxScore = gameModeState.Score;
            LeaderboardController.SetScore(gameModeState.Score);

            SaveManager.Save(SaveType.Game);
            SaveManager.Sync();
        }

        private void OnAddedScore(AddedScoreEvent eventData)
        {
            if (_isWaitingAddScore)
                return;

            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;

            if (gameState.Stats.MaxScore <= gameModeState.Score)
            {
                _isWaitingAddScore = true;
                StartCoroutine(AfterAddedScore(3));//∆дЄм пока наберутс€ все монетки. 
            }                       
        }

        private IEnumerator AfterAddedScore(float time)
        {
            yield return new WaitForSeconds(time);
            NewRecord();
            _isWaitingAddScore = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventsManager.Subscribe<AddedScoreEvent>(OnAddedScore);            
        }

        protected override void OnDisable()
        {
            base.OnDisable();    
            EventsManager.Unsubscribe<AddedScoreEvent>(OnAddedScore);            
        }
    }
}