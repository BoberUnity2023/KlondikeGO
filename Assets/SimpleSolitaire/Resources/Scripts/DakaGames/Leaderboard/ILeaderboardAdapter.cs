using System;
using BloomLines.Controllers;

namespace BloomLines.Leaderboard
{
    // Адаптер для разной логики лидербордов
    public interface ILeaderboardAdapter
    {
        void Initialize();
        void SetScore(int score);
        void GetLeaderboard(Action<LeaderboardData> onComplete);
    }
}