using GamePush;
using SimpleSolitaire.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Yandex
using YG;
#endif
using Platform = SimpleSolitaire.Controller.Platform;

public enum SaveType
{
    Yandex,
    Prefs,    
    Json
}

[Serializable] public class Save
{
    public bool NoAds;
    public int Score;
    public int Gold;
    public int Experience;
    public int PlayedGames;
    public int Wins;
    public int Losts;
    public int FastestWinTime;
    public int FastestPartyTime;
    public int LongestPartyTime;
    public string LastVisitTime;
    public bool[] TakenDayBonuses = new bool[5];
    public int[] AchivementProgress = new int[12];
    public bool[] CardBacks = new bool[60];
    public bool[] Cards = new bool[20];
    public bool[] Backgrounds = new bool[25];
}

public class SaveController : MonoBehaviour
{   
    [SerializeField] private GameManager _game;
    /*[HideInInspector] */public Save Save = new Save();
    private SaveType _saveType = SaveType.Yandex;

    public string KeyNoAds => "NoAds";

    public string KeyGold => "Gold";

    public string KeyExperience => "Experience";

    public string KeyPlayedGames => "PlayedGames";

    public string KeyWins => "Wins";

    public string KeyLosts => "Losts";

    public string KeyFastestWinTime => "FastestWinTime";

    public string KeyFastestPartyTime => "FastestPartyTime";

    public string KeyLongestPartyTime => "LongestPartyTime";

    public string KeyLastVisitTime => "LastVisitTime";

    public string KeyTakenDayBonuses => "TakenDayBonuses";

    public string KeyAchivementProgress => "AchivementProgress";

    public string KeyCards => "cards";
    public string KeyCardBacks => "cardbacks";
    public string KeyBackgrounds => "backgrounds";

    public string KeyJson => "json";

    public bool IsStorageReceived { get; set; }

    private string _json;

    public bool NoAds
    {
        get
        {
#if Yandex
            if (_game.Platform == Platform.Yandex)
            {
                return YG2.saves.NoAds;
            }
#endif
            if (_saveType == SaveType.Prefs)
            {
                return PlayerPrefs.GetInt(KeyNoAds) == 1;
            }

            if (_saveType == SaveType.Json)
            {
                return Save.NoAds || PlayerPrefs.GetInt(KeyNoAds) == 1;
            }            

            return false;
        }

        set
        {
            PlayerPrefs.SetInt(KeyNoAds, value ? 1 : 0);
            PlayerPrefs.Save();
#if Yandex
            if (_game.Platform == Platform.Yandex)
            {
                YG2.saves.NoAds = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.NoAds = value;
                SetSaveToJson();
            }
        }
    }

    public string LastVisitTime
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                return YG2.saves.LastVisitTime;
            }
#endif
            if (_saveType == SaveType.Prefs)
            {
                return PlayerPrefs.GetString(KeyLastVisitTime, "0");
            }

            if (_saveType == SaveType.Json)
            {
                return Save.LastVisitTime;//TODO: PlayerPrefs
            }
            
            return "0";
        }

        set
        {
            PlayerPrefs.SetString(KeyLastVisitTime, value);
            PlayerPrefs.Save();

#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.LastVisitTime = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            { 
                Save.LastVisitTime = value;
                SetSaveToJson(); 
            }
        }
    }

    public bool GetTakenDayBonuses(int day)
    {
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            return YG2.saves.TakenDayBonuses[day];
        }
#endif
        if (_saveType == SaveType.Json)
        {
            return
                Save.TakenDayBonuses[day] ||
                PlayerPrefs.GetInt(KeyTakenDayBonuses + day.ToString(), 0) == 1;
        }

        if (_saveType == SaveType.Prefs)
        {
            return PlayerPrefs.GetInt(KeyTakenDayBonuses + day.ToString(), 0) == 1;
        }
        return false;
    }

    public void SetTakenDayBonuses(int day, bool value)
    {
        PlayerPrefs.SetInt(KeyTakenDayBonuses + day.ToString(), value ? 1 : 0);
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.TakenDayBonuses[day] = value;
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        {            
            Save.TakenDayBonuses[day] = value;
            SetSaveToJson();
        }
    }
    
    public int GetAchivementProgress(int id)
    {
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            return YG2.saves.AchivementProgress[id];
        }
#endif
        if (_saveType == SaveType.Json)
        {
            return
                Mathf.Max(Save.AchivementProgress[id], 
                PlayerPrefs.GetInt(KeyAchivementProgress + id.ToString(), 0));
        }

        if (_saveType == SaveType.Prefs)
        {
            return PlayerPrefs.GetInt(KeyAchivementProgress + id.ToString(), 0);
        }
        return 0;
    }

    public void SetAchivementProgress(int id, int value)
    {
        PlayerPrefs.SetInt(KeyAchivementProgress + id.ToString(), value);
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.AchivementProgress[id] = value;
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        {
            Save.AchivementProgress[id] = value;
            SetSaveToJson();
        }
    }

    public bool GetCards(int id)
    {
        string key = KeyCards + id.ToString();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            return
                YG2.saves.Cards[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }
#endif
        if (_saveType == SaveType.Prefs)
        {
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        if (_saveType == SaveType.Json)
        {
            return
                Save.Cards[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }

        return false;
    }

    public void SetCards(int id, bool value)
    {
        string key = KeyCards + id.ToString();
        PlayerPrefs.SetInt(key, value ? 1 : 0);
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.Cards[id] = value;
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        { 
            Save.Cards[id] = value;
            SetSaveToJson();
        }
    }

    public bool GetCardBacks(int id)
    {
        string key = KeyCardBacks + id.ToString();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            return
                YG2.saves.CardBacks[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }
#endif
        if (_saveType == SaveType.Json)
        {
            return
                Save.CardBacks[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }

        return false;
    }

    public void SetCardBacks(int id, bool value)
    {
        string key = KeyCardBacks + id.ToString();
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.CardBacks[id] = value;
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        {   
            Save.CardBacks[id] = value;
            SetSaveToJson();
        }
    }

    public bool GetBackgrounds(int id)
    {
        string key = KeyBackgrounds + id.ToString();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            return
                YG2.saves.Backgrounds[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }
#endif
        if (_saveType == SaveType.Json)
        {
            return
                Save.Backgrounds[id] ||
                PlayerPrefs.GetInt(key, 0) == 1;
        }

        return false;
    }

    public void SetBackgrounds(int id, bool value)
    {
        string key = KeyBackgrounds + id.ToString();

        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.Backgrounds[id] = value;
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        {   
            Save.Backgrounds[id] = value;
            SetSaveToJson();
        }
    }

    public void ResetTakenDayBonuses()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt(KeyTakenDayBonuses + i.ToString(), 0);
        }
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.saves.TakenDayBonuses = new bool[5] { false, false, false, false, false };
            YG2.SaveProgress();
        }
#endif
        if (_saveType == SaveType.Json)
        {
            for (int i = 0; i < 5; i++)
            {
                Save.TakenDayBonuses[i] = false;
            }            
            SetSaveToJson();
        }
    }

    public int Gold
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                return YG2.saves.Gold;
            }
#endif
            if (_saveType == SaveType.Prefs)
            {
                return PlayerPrefs.GetInt(KeyGold, 20000);
            }

            if (_saveType == SaveType.Json)
            {
                return Mathf.Max(Save.Gold, PlayerPrefs.GetInt(KeyGold, 0));
            }            
            
            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyGold, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.Gold = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            { 
                Save.Gold = value;
                SetSaveToJson();
            }
        }
    }

    public int Experience
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)            
                return YG2.saves.Experience;
#endif
            int fromPrefs = PlayerPrefs.GetInt(KeyExperience);
            
            if (_saveType == SaveType.Prefs)            
                return fromPrefs;            

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.Experience, fromPrefs);

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyExperience, value);
            PlayerPrefs.Save();
#if GAME_PUSH
            GP_Player.SetScore(value);
            GP_Player.Sync(SyncStorageType.cloud);
#endif
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.Experience = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.Experience = value;
                SetSaveToJson();
            }
        }
    }

    public int PlayedGames
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                return YG2.saves.PlayedGames;
            }
#endif
            if (_saveType == SaveType.Prefs)
            {
                return PlayerPrefs.GetInt(KeyPlayedGames, 0);
            }

            if (_saveType == SaveType.Json)
            {
                return Mathf.Max(Save.PlayedGames, PlayerPrefs.GetInt(KeyPlayedGames, 0));
            }

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyPlayedGames, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.PlayedGames = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.PlayedGames = value;
                SetSaveToJson();
            }
        }
    }

    public int Wins
    {
        get
        {
            int wins = PlayerPrefs.GetInt(KeyWins, 0);
#if Yandex
            if (_saveType == SaveType.Yandex)            
                return YG2.saves.Wins;
#endif
            if (_saveType == SaveType.Prefs)            
                return wins;            

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.Wins, wins);            

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyWins, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.Wins = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {
                Save.Wins = value;
                SetSaveToJson();
            }
        }
    }

    public int Losts
    {
        get
        {
            int losts = PlayerPrefs.GetInt(KeyLosts, 0);
#if Yandex
            if (_saveType == SaveType.Yandex)
                return YG2.saves.Losts;
#endif
            if (_saveType == SaveType.Prefs)
                return losts;

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.Losts, losts);            

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyLosts, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.Losts = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.Losts = value;
                SetSaveToJson();
            }
        }
    }

    public int FastestWinTime
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)            
                return YG2.saves.FastestWinTime;
#endif
            if (_saveType == SaveType.Prefs)            
                return PlayerPrefs.GetInt(KeyFastestWinTime, 7200);            

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.FastestWinTime, PlayerPrefs.GetInt(KeyFastestWinTime, 7200));            

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyFastestWinTime, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.FastestWinTime = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.FastestWinTime = value;
                SetSaveToJson();
            }
        }
    }

    public int FastestPartyTime
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)            
                return YG2.saves.FastestPartyTime;
#endif
            if (_saveType == SaveType.Prefs)            
                return PlayerPrefs.GetInt(KeyFastestPartyTime);            

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.FastestPartyTime, PlayerPrefs.GetInt(KeyFastestPartyTime));            

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyFastestPartyTime, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.FastestPartyTime = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.FastestPartyTime = value;
                SetSaveToJson();
            }
        }
    }

    public int LongestPartyTime
    {
        get
        {
#if Yandex
            if (_saveType == SaveType.Yandex)            
                return YG2.saves.LongestPartyTime;
#endif
            if (_saveType == SaveType.Prefs)            
                return PlayerPrefs.GetInt(KeyLongestPartyTime);            

            if (_saveType == SaveType.Json)            
                return Mathf.Max(Save.LongestPartyTime, PlayerPrefs.GetInt(KeyLongestPartyTime));            

            return 0;
        }

        set
        {
            PlayerPrefs.SetInt(KeyLongestPartyTime, value);
            PlayerPrefs.Save();
#if Yandex
            if (_saveType == SaveType.Yandex)
            {
                YG2.saves.LongestPartyTime = value;
                YG2.SaveProgress();
            }
#endif
            if (_saveType == SaveType.Json)
            {                
                Save.LongestPartyTime = value;
                SetSaveToJson();
            }
        }
    }

    public int ConvertStringToInt(string value)
    {
        int _output;

        bool success = Int32.TryParse(value, out _output);
        if (!success)
        {
            Debug.LogWarning("Error ConvertStringToInt failed! Value:" + value);
            return 0;
        }

        return _output;
    }

    // Подписываемся на событие GetDataEvent в OnEnable
    private void OnEnable()
    {
        if (_saveType == SaveType.Yandex)
        {
            //YG2.GetDataEvent += GetData;
        }
    }

    // Отписываемся от события GetDataEvent в OnDisable
    private void OnDisable()
    {
        if (_saveType == SaveType.Yandex)
        {
            //YandexGame.GetDataEvent -= GetData;
        }
    }

    private void Awake()
    {
        SetSaveType();

        if (_saveType == SaveType.Yandex)
        {
            // Проверяем запустился ли плагин
            //if (YG2.SDKEnabled == true)
            //{
            //    // Если запустился, то запускаем Ваш метод
            //    GetData();

            //    // Если плагин еще не прогрузился, то метод не запуститься в методе Start,
            //    // но он запустится при вызове события GetDataEvent, после прогрузки плагина
            //}
        }

        if (_saveType == SaveType.Json)
            FillSaveFromPlayerPrefs();
    }

    // Ваш метод, который будет запускаться в старте
    public void GetData()
    {
        // Получаем данные из плагина и делаем с ними что хотим
        //_gameManager.Gold = YandexGame.savesData.Gold;
        //_game.
        //Debug.Log("SavesContoller.GetData");
#if Yandex
        YG2.SaveProgress();
        //YandexGame.savesData.Gold;
#endif
    }


    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
#if Yandex
        if (_saveType == SaveType.Yandex)
        {
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            YG2.SetLeaderboard("Exp", 0);//TODO:
        }
#endif
        if (_saveType == SaveType.Json)
        {
            FillSaveReset();
        }
    }    

    private void SetSaveType()
    {
        switch (_game.Platform)
        {
            case Platform.Yandex:
                {
                    _saveType = SaveType.Yandex;
                    break;
                }

            case Platform.VK:
                {
                    _saveType = SaveType.Json;
                    break;
                }

            case Platform.Ok:
                {
                    _saveType = SaveType.Json;
                    break;
                }
        }
    }

    private void FillSaveFromPlayerPrefs()
    {

        Save.TakenDayBonuses = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            Save.TakenDayBonuses[i] = PlayerPrefs.GetInt(KeyTakenDayBonuses + i.ToString()) == 1;
        }

        Save.AchivementProgress = new int[10];
        for (int i = 0; i < 10; i++)
        {
            Save.AchivementProgress[i] = PlayerPrefs.GetInt(KeyAchivementProgress + i.ToString()) ;
        }

        Save.LastVisitTime = PlayerPrefs.GetString(KeyLastVisitTime, "0");
        Save.Score = PlayerPrefs.GetInt(KeyGold, 0);
        Save.Experience = PlayerPrefs.GetInt(KeyExperience, 0);
        Save.Wins = PlayerPrefs.GetInt(KeyWins, 0);
        Save.Losts = PlayerPrefs.GetInt(KeyLosts, 0);
        Save.FastestWinTime = PlayerPrefs.GetInt(KeyFastestWinTime, 0);
        Save.FastestPartyTime = PlayerPrefs.GetInt(KeyFastestPartyTime, 0);
        Save.LongestPartyTime = PlayerPrefs.GetInt(KeyLongestPartyTime, 0);
        Save.NoAds = PlayerPrefs.GetInt(KeyNoAds) == 1;

        //VKManager.Instance.StorageSave();
    }

    public void FillSaveFromPlayerPrefsOrStorage(Save save)
    {
        Save.TakenDayBonuses = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            Save.TakenDayBonuses[i] = PlayerPrefs.GetInt(KeyTakenDayBonuses + i.ToString()) == 1 || save.TakenDayBonuses[i];
        }

        Save.AchivementProgress = new int[10];
        for (int i = 0; i < 10; i++)
        {
            Save.AchivementProgress[i] = Mathf.Max(PlayerPrefs.GetInt(KeyAchivementProgress + i.ToString()), save.AchivementProgress[i]);
        }

        int fromPlayerPrefs = ConvertStringToInt(PlayerPrefs.GetString(KeyLastVisitTime, "0"));
        int fromStorage = ConvertStringToInt(save.LastVisitTime);

        Save.LastVisitTime = Mathf.Max(fromPlayerPrefs, fromStorage).ToString();
        Save.Gold = Mathf.Max(PlayerPrefs.GetInt(KeyGold, 0), save.Gold);
        Save.Score = Mathf.Max(PlayerPrefs.GetInt(KeyGold, 0), save.Score);
        Save.Experience = Mathf.Max(PlayerPrefs.GetInt(KeyExperience, 0), save.Experience);
        Save.Wins = Mathf.Max(PlayerPrefs.GetInt(KeyWins, 0), save.Wins);
        Save.Losts = Mathf.Max(PlayerPrefs.GetInt(KeyLosts, 0), save.Losts);
        Save.FastestWinTime = Mathf.Max(PlayerPrefs.GetInt(KeyFastestWinTime, 0), save.FastestWinTime);
        Save.FastestPartyTime = Mathf.Max(PlayerPrefs.GetInt(KeyFastestPartyTime, 0), save.FastestPartyTime);
        Save.LongestPartyTime = Mathf.Max(PlayerPrefs.GetInt(KeyLongestPartyTime, 0), save.LongestPartyTime);
        Save.NoAds = PlayerPrefs.GetInt(KeyNoAds) == 1 || save.NoAds;
        //VKManager.Instance.StorageSave();
    }

    private void FillSaveReset()
    {
        Save.TakenDayBonuses = new bool[5];
        Save.LastVisitTime = "0";
        Save.Gold = 0;
        Save.Score = 0;
        Save.NoAds = false;        

        if (_saveType == SaveType.Json)
            SetSaveToJson();
    }

    public void OnGamePushInit()//GP
    {
#if GAME_PUSH
        Debug.Log("GetTING JSON...*GamePush*");        
        _json = GP_Player.GetString(KeyJson); 
        Save = JsonUtility.FromJson<Save>(_json);
        FillSaveFromPlayerPrefsOrStorage(Save);
        Debug.Log("Get JSON: " + _json);
#endif
    }

    private void SetSaveToJson()//GP
    {
       // Debug.Log("Setting json...");
        _json = JsonUtility.ToJson(Save);
#if GAME_PUSH
        GP_Player.Set(KeyJson, _json);
        GP_Player.Sync();
# endif
        //Debug.Log("Set JSON: " + _json);
    }
}
