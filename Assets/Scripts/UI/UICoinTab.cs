using BloomLines.Controllers;
using BloomLines.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BloomLines.UI
{
    public class UICoinTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _count;

        private float _coins;
        private int _targetCoins;
        private Tween _tween;

        private void OnAddedScore(AddedScoreEvent eventData)
        {
            var stats = SaveManager.GameState.Stats;
            if (_targetCoins <= stats.MaxScore && _targetCoins + eventData.Count > stats.MaxScore)
            {
                EventsManager.Publish(new ShowNotificationEvent("new_record", string.Empty));
            }

            _targetCoins += eventData.Count;

            _tween.Kill();

            var sequence = DOTween.Sequence();

            sequence.Append(DOTween.To(() => _coins, x =>
            {
                _coins = x;
                _count.text = Mathf.RoundToInt(_coins).ToString();
            }, _targetCoins, 0.5f));

            sequence.Join(_count.transform.DOScale(1.1f, 0.1f)).Append(_count.transform.DOScale(1f, 0.1f));

            _tween = sequence;
        }

        private void UpdateVisual()
        {
            var gameModeState = SaveManager.GameModeState;
            _count.text = gameModeState.Score.ToString();

            _coins = gameModeState.Score;
            _targetCoins = gameModeState.Score;
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            UpdateVisual();
        }

        private void OnCollectDailyReward(CollectDailyRewardEvent eventData)
        {
            UpdateVisual();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<AddedScoreEvent>(OnAddedScore);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Subscribe<CollectDailyRewardEvent>(OnCollectDailyReward);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<AddedScoreEvent>(OnAddedScore);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Unsubscribe<CollectDailyRewardEvent>(OnCollectDailyReward);
        }
    }
}