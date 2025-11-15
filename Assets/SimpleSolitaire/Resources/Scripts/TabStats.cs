using SimpleSolitaire.Controller;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TabStats : MonoBehaviour
{
    [SerializeField] private GameManager _hub;
    [SerializeField] private Text _indicatorExperience;
    [SerializeField] private Text _indicatorCoins;
    [SerializeField] private Text _indicatorGames;
    [SerializeField] private Text _indicatorWins;
    [SerializeField] private Text _indicatorLosts;
    [SerializeField] private Text _indicatorPercentWins;
    [SerializeField] private Text _indicatorFastestWinTime;    
    [SerializeField] private Text _indicatorLongestPartyTime;

    private void OnEnable()
    {
        _indicatorExperience.text = _hub.Stats.Experience.ToString();
        _indicatorCoins.text = _hub.Stats.GoldForAllTime.ToString();
        _indicatorGames.text = _hub.Stats.PlayedGames.ToString();
        _indicatorWins.text = _hub.Stats.Wins.ToString();
        _indicatorLosts.text = _hub.Stats.Losts.ToString();
        float percentWins = _hub.Stats.Wins * 100 / Mathf.Max(_hub.Stats.PlayedGames, 1);
        _indicatorPercentWins.text = percentWins.ToString("f2") + "%";        

        _indicatorFastestWinTime.text = TimeString(_hub.Stats.FastestWinTime);        
        _indicatorLongestPartyTime.text = TimeString(_hub.Stats.LongestWinTime);
    }

    private string TimeString(int sec)
    {
        if (sec == 0)
            return "--:--";

        TimeSpan result = TimeSpan.FromSeconds(sec);
        return result.ToString("mm':'ss");
    }
}
