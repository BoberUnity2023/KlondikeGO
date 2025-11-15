using UnityEngine;
using SimpleSolitaire.Controller;

public class Stats
{
    private GameManager _hub;

    public int Experience
    {
        get
        {
            return PlayerPrefs.GetInt("Experience", 0);
        }

        set
        {
            PlayerPrefs.SetInt("Experience", value);
            PlayerPrefs.Save(); 
        }
    }

    public int GoldForAllTime
    {
        get
        {
            return PlayerPrefs.GetInt("GoldForAllTime", 0);
        }

        set
        {
            PlayerPrefs.SetInt("GoldForAllTime", value);
            PlayerPrefs.Save();
        }
    }

    public int PlayedGames
    {
        get
        {
            return PlayerPrefs.GetInt("PlayedGames", 0);
        }

        set
        {
            PlayerPrefs.SetInt("PlayedGames", value);
            PlayerPrefs.Save();
        }
    }

    public int Wins
    {
        get
        {
            return PlayerPrefs.GetInt("Wins", 0);
        }

        set
        {
            PlayerPrefs.SetInt("Wins", value);
            PlayerPrefs.Save();
        }
    }

    public int Losts
    {
        get
        {
            return PlayedGames - Wins;
        }
    }

    public int FastestWinTime
    {
        get
        {
            return PlayerPrefs.GetInt("FastestWinTime", 7200);
        }

        set
        {
            PlayerPrefs.SetInt("FastestWinTime", value);
            PlayerPrefs.Save();
        }
    }

    
    public int LongestWinTime
    {
        get
        {
            return PlayerPrefs.GetInt("LongestWinTime", 0);
        }

        set
        {
            PlayerPrefs.SetInt("LongestWinTime", value);
            PlayerPrefs.Save();
        }
    }
    //public int Experience
    //{
    //    get
    //    {
    //        return _hub.Saves.Experience;
    //    }
    //    set
    //    {
    //        _hub.Saves.Experience = value;
    //        if (_hub.Game.Platform == Platform.Yandex)
    //        {             
    //            YandexGame.NewLeaderboardScores("Advanced", value);
    //            YandexGame.SaveProgress();                
    //        }

    //        _hub.UI.WindowRating.TabProgress.SetExperience(value);            
    //    }
    //}

    public Stats (GameManager hub)
    {
        _hub = hub;   
    }
}
