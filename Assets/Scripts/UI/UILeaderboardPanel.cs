using System;
using System.Collections.Generic;
using BloomLines.Controllers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UILeaderboardPanel : UIPanelBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private UILeaderboardItem _itemPrefab;
        [SerializeField] private GameObject _noItemsTitle;

        private List<UILeaderboardItem> _items;
        private long _lastUpdateTimestamp;

        protected override void Awake()
        {
            base.Awake();

            _items = new List<UILeaderboardItem>();

            _closeButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                Close();
            });

            _backgroundButton.onClick.AddListener(Close);
        }

        private void TryLoadLeaderboard()
        {
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (Mathf.Abs(currentTimestamp - _lastUpdateTimestamp) < 60f)
                return;

            LeaderboardController.GetLeaderboard(OnGetLeaderboard);

            _lastUpdateTimestamp = currentTimestamp;
        }

        private void OnGetLeaderboard(LeaderboardData data)
        {
            if(data.Players.Length > 0)
                _noItemsTitle.SetActive(false);

            foreach (var item in _items)
                item.gameObject.SetActive(false);

            for(int i = 0; i < data.Players.Length; i++)
            {
                if (_items.Count <= i)
                {
                    var newItem = Instantiate(_itemPrefab, _itemsParent);
                    _items.Add(newItem);
                }

                _items[i].Set(data.Players[i]);
                _items[i].gameObject.SetActive(true);
            }
        }

        protected override void Open()
        {
            base.Open();

            TryLoadLeaderboard();
#if CRAZY_GAMES
            CrazyGames.CrazySDK.Game.GameplayStop();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStop();
#endif
        }

        public void OnClickClose()
        {
            AudioController.Play("click_button");
            Close();
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
