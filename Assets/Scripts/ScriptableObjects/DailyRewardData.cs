using System;
using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.Assets
{
    public enum DailyRewardType
    {
        Coins,
        CrystalFlowers,
    }

    [CreateAssetMenu(fileName = "DailyRewardData", menuName = "BloomLines/DailyRewardData")]
    public class DailyRewardData : ScriptableObject
    {
        [SerializeField] private GameModeType _gameModeType;
        [SerializeField] private DayOfWeek _dayOfWeek;
        [SerializeField] private DailyRewardType _rewardType;
        [SerializeField] private int _rewardCount;
        [SerializeField] private Sprite _icon;

        public DailyRewardType RewardType => _rewardType;
        public int RewardCount => _rewardCount;
        public GameModeType GameModeType => _gameModeType;
        public DayOfWeek DayOfWeek => _dayOfWeek;
        public Sprite Icon => _icon;
    }
}