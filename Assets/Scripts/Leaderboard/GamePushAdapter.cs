#if GAME_PUSH
using System;
using BloomLines.Controllers;
using GamePush;
using UnityEngine;

namespace BloomLines.Leaderboard
{
    [System.Serializable]
    public class GPLeaderboardPlayer
    {
        public int id;
        public string name;
        public string avatar;
        public int position;
        public int score;
    }

    // Адаптер под лидерборд от GamePush
    public class GamePushAdapter : ILeaderboardAdapter
    {
        private Action<LeaderboardData> _onComplete;

        public void Initialize()
        {
            GP_Leaderboard.OnFetchSuccess += OnFetchSuccess;
        }

        public void GetLeaderboard(Action<LeaderboardData> onComplete)
        {
            Debug.Log("GamePush GetLeaderboard");

            _onComplete = onComplete;
            GP_Leaderboard.Fetch("25182", "score", Order.DESC, 50, 0, WithMe.none, "score");
        }

        public void SetScore(int score)
        {
            Debug.Log("GamePush SetLeaderboardScore: " + score);

            GP_Player.SetScore(score);
            GP_Player.Sync();
        }

        private void OnFetchSuccess(string tag, GP_Data lbData)
        {
            Debug.Log("GamePush OnLeaderboardLoaded");

            var players = lbData.GetList<GPLeaderboardPlayer>();

            var data = new LeaderboardData();
            data.Players = new LeaderboardPlayerData[players.Count];

            for(int i = 0; i < data.Players.Length; i++)
            {
                var name = players[i].name;
                var position = players[i].position;
                var score = players[i].score;
                var photo = players[i].avatar;

                data.Players[i] = new LeaderboardPlayerData(name, position, score, photo);
            }

            _onComplete?.Invoke(data);
        }
    }
}
#endif