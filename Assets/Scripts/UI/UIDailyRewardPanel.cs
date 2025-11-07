using System;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.Tools;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class CollectDailyRewardEvent
    {
        private DailyRewardType _rewardType;
        private int _rewardCount;

        public DailyRewardType RewardType => _rewardType;
        public int RewardCount => _rewardCount;

        public CollectDailyRewardEvent(DailyRewardType rewardType, int rewardCount)
        {
            _rewardType = rewardType;
            _rewardCount = rewardCount;
        }
    }

    public class UIDailyRewardPanel : UIPanelBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _multiplyButton;
        [SerializeField] private Button _collectButton;

        private Tween _tween;
        private Board _board;

        protected override void Awake()
        {
            base.Awake();

            _board = FindAnyObjectByType<Board>();

            void DefaultCollect()
            {
                Collect(1);
            }

            _closeButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_daily_reward_close_button");
                DefaultCollect();
            });
            
            _backgroundButton.onClick.AddListener(() =>
            {
                AnalyticsController.SendEvent("click_daily_reward_close_button");
                DefaultCollect();
            });

            _collectButton.onClick.AddListener(() =>
            {
                AnalyticsController.SendEvent("click_daily_reward_collect_button");
                DefaultCollect();
            });

            _multiplyButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_daily_reward_multiply_button");

                AdsController.ShowRewarded((success) =>
                {
                    if (success)
                        Collect(2);
                });
            });
        }

        private void Collect(int multiplier)
        {
            Debug.Log($"Collect daily reward x{multiplier}");

            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;
            var dateTime = DateTimeOffset.Now;
            var currentTimestamp = dateTime.ToUnixTimeSeconds();

            gameState.LastDailyRewardTimestamp = currentTimestamp;

            var dailyRewardData = GameAssets.GetDailyRewardData(gameModeState.Type, dateTime.DayOfWeek);

            switch (dailyRewardData.RewardType)
            {
                case DailyRewardType.Coins:
                    EventsManager.Publish(new AddedScoreEvent(dailyRewardData.RewardCount * multiplier));
                    break;
                case DailyRewardType.CrystalFlowers:
                    var tile = _board.GetTile(3, 3);

                    for (int i = 0; i < dailyRewardData.RewardCount * multiplier; i++)
                    {
                        var plant = SpawnerManager.BoardObjectsSpawner.SpawnObject("flower_crystal") as Plant;

                        plant.RectTransform.SetParent(tile.RectTransform, false);
                        plant.RectTransform.localScale = Vector3.zero;
                        plant.RectTransform.localPosition = Vector3.zero;

                        EventsManager.Publish(new CutPlantEvent(tile, plant));
                    }
                    break;
            }

            EventsManager.Publish(new CollectDailyRewardEvent(dailyRewardData.RewardType, dailyRewardData.RewardCount * multiplier));

            SaveManager.Save(SaveType.Game);
            SaveManager.Save(SaveType.GameMode);
            SaveManager.Sync();

            Close();
        }

        protected override void Open()
        {
            base.Open();

            var gameModeState = SaveManager.GameModeState;
            var dateTime = DateTimeOffset.Now;
            var dailyRewardData = GameAssets.GetDailyRewardData(gameModeState.Type, dateTime.DayOfWeek);

            _collectButton.image.sprite = dailyRewardData.Icon;
#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStop();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStop();
#endif
        }

        private void TryShowDailyReward()
        {
            if (!TutorialController.IsCompleted(TutorialIds.FIRST_GAME))
                return;

            var gameState = SaveManager.GameState;
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var diff = Mathf.Abs(gameState.LastDailyRewardTimestamp - currentTimestamp);

            if(diff >= 86400)
                EventsManager.Publish(new OpenPanelEvent(UIPanelType.DailyReward));
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            TryShowDailyReward();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);

            var rectTransform = _collectButton.image.rectTransform;

            rectTransform.localScale = Vector3.one * 0.98f;
            _tween = DOTween.Sequence()
                .Append(rectTransform.DOScale(1.01f, 0.5f).SetEase(Ease.InSine))
                .Append(rectTransform.DOScale(0.98f, 0.5f).SetEase(Ease.OutSine))
                .SetLoops(-1);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);

            _tween?.Kill();
            _tween = null;
        }

        protected override void Close()
        {
            base.Close();
#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStart();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStart();
#endif
        }
    }
}