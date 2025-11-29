#if Yandex
using System;
using BloomLines.Controllers;
using UnityEngine;
using YG;
using YG.Utils.LB;

namespace BloomLines.Leaderboard
{
    // Адаптер под яндекс лидерборд
    public class YandexAdapter : ILeaderboardAdapter
    {
        private Action<LeaderboardData> _onComplete;

        public void Initialize()
        {
            YG2.onGetLeaderboard += OnGetLeaderboard;
        }

        public void GetLeaderboard(Action<LeaderboardData> onComplete)
        {
            Debug.Log("Yandex GetLeaderboard");

            _onComplete = onComplete;

            YG2.GetLeaderboard("score", 50, 50, "128x128");
        }

        public void SetScore(int score)
        {
            Debug.Log("Yandex SetLeaderboardScore: " + score);

            YG2.SetLeaderboard("score", score);
        }

        private void OnGetLeaderboard(LBData lbData)
        {
            Debug.Log("Yandex OnLeaderboardLoaded");

            var data = new LeaderboardData();
            data.Players = new LeaderboardPlayerData[lbData.players.Length];

            for(int i = 0; i < data.Players.Length; i++)
            {
                var name = lbData.players[i].name;
                var position = lbData.players[i].rank;
                var score = lbData.players[i].score;
                var photo = lbData.players[i].photo;

                data.Players[i] = new LeaderboardPlayerData(name, position, score, photo);
            }

            _onComplete?.Invoke(data);
        }
    }
}
#endif