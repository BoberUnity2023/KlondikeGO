using System;
using BloomLines.Leaderboard;
using UnityEngine;

namespace BloomLines.Controllers
{
    // Данные лидерборда
    public class LeaderboardData
    {
        public LeaderboardPlayerData[] Players;
    }

    // Данные игрока в лидерборде
    public class LeaderboardPlayerData
    {
        public string Name { get; private set; }
        public int Position { get; private set; }
        public int Score { get; private set; }
        public string Photo { get; private set; }

        public LeaderboardPlayerData(string name, int position, int score, string photo)
        {
            Name = name;
            Position = position;
            Score = score;
            Photo = photo;
        }
    }

    public static class LeaderboardController
    {
        private static ILeaderboardAdapter _adapter; // Адаптер лидерборда

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            return;
#endif

            // Загружает текущий адаптер лидерборда под нужную платформу

#if Yandex
            _adapter = new YandexAdapter();
            if (_adapter == null)
                Debug.LogError("YandexAdapter = null");
#endif

#if GAME_PUSH
            _adapter = new GamePushAdapter();
            if (_adapter == null)
                Debug.LogError("GamePushAdapter = null");
#endif
            if (_adapter == null)            
                Debug.LogError("LeaderboardAdapter = null");
            
            if (_adapter != null)
                _adapter.Initialize();
        }

        // Получить лидерборда
        public static void GetLeaderboard(Action<LeaderboardData> onComplete)
        {
#if UNITY_EDITOR
            var data = new LeaderboardData();
            data.Players = new LeaderboardPlayerData[10];
            data.Players[0] = new LeaderboardPlayerData("FirstName", 1, 5006000, string.Empty);
            data.Players[1] = new LeaderboardPlayerData("Jaw", 2, 456750, string.Empty);
            data.Players[2] = new LeaderboardPlayerData("Test", 3, 1122, string.Empty);
            data.Players[3] = new LeaderboardPlayerData("BlaBla", 4, 999, string.Empty);
            data.Players[4] = new LeaderboardPlayerData("Simple", 5, 300, string.Empty);
            data.Players[5] = new LeaderboardPlayerData("HaH", 6, 200, string.Empty);
            data.Players[6] = new LeaderboardPlayerData("Null", 7, 78, string.Empty);
            data.Players[7] = new LeaderboardPlayerData("RandomName", 8, 55, string.Empty);
            data.Players[8] = new LeaderboardPlayerData("UniqueName", 9, 0, string.Empty);
            data.Players[9] = new LeaderboardPlayerData("LastName", 10, 0, string.Empty);
            onComplete?.Invoke(data);
            return;
#endif

            _adapter.GetLeaderboard(onComplete);
        }

        public static void SetScore(int score)
        {
#if UNITY_EDITOR
            return;
#endif

            _adapter.SetScore(score);
        }
    }
}