using System;
using BloomLines.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UIGamemodeTab : MonoBehaviour
    {
        [SerializeField] private GameModeType _gameModeType;
        [SerializeField] private Toggle _toggle;

        public Toggle Toggle => _toggle;
        public GameModeType GameModeType => _gameModeType;

        public void Initialize(Action<GameModeType> onSelect)
        {
            _toggle.onValueChanged.AddListener((value) =>
            {
                AnalyticsController.SendEvent($"click_gamemode_tab_{_gameModeType.ToString().ToLowerInvariant()}");

                onSelect?.Invoke(_gameModeType);
            });
        }
    }
}
