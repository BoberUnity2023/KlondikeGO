using UnityEngine;
using SimpleSolitaire.Controller;

public class Stats
{
    private GameManager _gameManager;
    private SaveController Save => _gameManager.Save;

    public Stats(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public int Experience
    {
        get
        {
            return Save.Experience;
        }

        set
        {
            Save.Experience = value; 
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
            return Save.PlayedGames;
        }

        set
        {            
            Save.PlayedGames = value;
        }
    }

    public int Wins
    {
        get
        {
            return Save.Wins;
        }

        set
        {
            Save.Wins = value;
        }
    }

    public int Losts
    {
        get
        {
            return Save.PlayedGames - Save.Wins;
        }
    }

    public int FastestWinTime
    {
        get
        {
            return Save.FastestWinTime;
        }

        set
        {
            Save.FastestWinTime =  value;
        }
    }

    
    public int LongestPartyTime
    {
        get
        {
            return Save.LongestPartyTime;
        }

        set
        {
            Save.LongestPartyTime = value;
        }
    }

    public int GetAchivementProgress(int id)
    {
        //if (_saveType == SaveType.Yandex)
        //{
        //    return YandexGame.savesData.AchivementProgress[id];
        //}

        //if (_saveType == SaveType.Struct || _saveType == SaveType.Json)
        //{
        //    return
        //        Mathf.Max(Save.AchivementProgress[id],
        //        PlayerPrefs.GetInt(KeyAchivementProgress + id.ToString(), 0));
        //}

        //if (_saveType == SaveType.Prefs)
        //{
        return Save.GetAchivementProgress(id);//  PlayerPrefs.GetInt("AchivementProgress" + id.ToString(), 0);
        //}
        //return 0;
    }

    public void SetAchivementProgress(int id, int value)
    {
        Save.SetAchivementProgress(id, value);
        //if (_saveType == SaveType.Yandex)
        //{
        //    YandexGame.savesData.AchivementProgress[id] = value;
        //    YandexGame.SaveProgress();
        //}

        //if (_saveType == SaveType.Prefs || _saveType == SaveType.Struct)
        //{
        //    PlayerPrefs.SetInt("AchivementProgress" + id.ToString(), value);
        //    PlayerPrefs.Save();
        //}

        
        //if (_saveType == SaveType.Json)
        //{
        //    Save.AchivementProgress[id] = value;
        //    SetSaveToJson();
        //}
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
}
