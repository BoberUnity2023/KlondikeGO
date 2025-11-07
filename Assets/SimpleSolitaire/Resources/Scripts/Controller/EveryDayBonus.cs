using GameAnalyticsSDK;
using SimpleSolitaire.Controller;
using System;
using System.Collections;
using UnityEngine;
//using YG;

public class EveryDayBonus : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject[] _otherLayers;    
    [SerializeField] private GameObject _window;
    [SerializeField] private BonusDay[] _days;
    [SerializeField] private int[] _prices;
    private int everyday;

    private void Start()
    {
        StartCoroutine(TryShow(4));
    }

    private IEnumerator TryShow(float time)
    {
        yield return new WaitForSeconds(time);

        if (IsOtherLayerActive)
            StartCoroutine(TryShow(3f));
        else
            StartCoroutine(Show(0.5f));
    }

    private IEnumerator Show(float time)
    {
        yield return new WaitForSeconds(time);

        if (HasLastVisit)
        {
            int currentTime = (int)(DateTime.UtcNow - new DateTime(2024, 1, 1)).TotalSeconds;
            int _lastVisitTime = ConvertLastVisitTime();
            bool lastVisitToday = currentTime - DateTime.Now.Hour * 3600 - DateTime.Now.Minute * 60 - DateTime.Now.Second < _lastVisitTime;
            bool lastVisitYesterday = !lastVisitToday && currentTime - DateTime.Now.Hour * 3600 - DateTime.Now.Minute * 60 - DateTime.Now.Second - 24 * 3600 < _lastVisitTime;

            if (lastVisitToday)
            {
                Debug.LogWarning("Прошлый визит: Cегодня ");
                everyday = Mathf.Min(PlayerPrefs.GetInt("EveryDayVisits"), 5);//PlayerPrefs.GetInt("EDB_Evereday");
                GameAnalytics.NewDesignEvent("LastVisit: Today");
            }
            if (lastVisitYesterday)
            {
                Debug.LogWarning("Прошлый визит: Bчера ");
                everyday = Mathf.Min(PlayerPrefs.GetInt("EveryDayVisits") + 1, 5);// PlayerPrefs.GetInt("EDB_Evereday") + 1;
                PlayerPrefs.SetInt("EveryDayVisits", everyday);
                PlayerPrefs.Save();
                //YandexGame.SaveProgress();                
                _gameManager.ShowEveryDayBonusWindow();
                SetDayWindow(everyday);
                GameAnalytics.NewDesignEvent("LastVisit: Yesterday");
            }

            if (!lastVisitToday && !lastVisitYesterday)
            {
                Debug.LogWarning("Прошлый визит: Давнее чем вчера");                
                PlayerPrefs.SetInt("EveryDayVisits", 0);
                PlayerPrefs.Save();
                //YandexGame.savesData.EveryDayVisits = 0;
                //YandexGame.SaveProgress();
                _gameManager.ShowEveryDayBonusWindow();
                SetDayWindow(everyday);
                GameAnalytics.NewDesignEvent("LastVisit: A few das ago");
            }
        }
        else
        {
            Debug.LogWarning("Прошлый визит: Hе найдено");
            _gameManager.ShowEveryDayBonusWindow();            
            SetDayWindow(everyday);
            GameAnalytics.NewDesignEvent("LastVisit: It is First");
        }
        int lastVisitTime = (int)(DateTime.UtcNow - new DateTime(2024, 1, 1)).TotalSeconds;
        //PlayerPrefs.SetString("EDB_LastVisit", lastVisitTime.ToString());//Повесить на выход !!!!!!!!!!!!!!!!!!!
        PlayerPrefs.SetString("LastVisitTime", lastVisitTime.ToString());
        PlayerPrefs.Save();
        //YandexGame.savesData.LastVisitTime = lastVisitTime.ToString();
        //YandexGame.SaveProgress();
    }

    private bool HasLastVisit
    {
        get
        {
            return PlayerPrefs.GetString("LastVisitTime") != "0"; //YandexGame.savesData.LastVisitTime != "0"; }
        }
    }
    private int ConvertLastVisitTime()
    {
        int _output = 0;
        string _value = PlayerPrefs.GetString("LastVisitTime", "0");//YandexGame.savesData.LastVisitTime;        

        bool success = Int32.TryParse(_value, out _output);
        if (!success)
        {
            Debug.LogError("Error Convert YandexGame.savesData.LastVisitTime.ToInt32 failed!");
        }        

        return _output;
    }

    private void SetDayWindow(int everyday) 
    {  
        for (int i = 0; i < _days.Length; i++)
        {            
            if (everyday < i)
                _days[i].SetNext(i, _prices[i]);

            if (everyday == i)
                _days[i].SetCurrent(i, _prices[i]);

            if (everyday > i)
                _days[i].SetLast(i, _prices[i]);
        }
    }

    public void PressGet()
    {
        int gold = _prices[everyday];
        Debug.LogWarning("Ежедневный бонус. Зачислено: " + gold.ToString());
        _gameManager.Gold += gold;        
    }

    public void OnRewardGetX2()
    {
        int gold = _prices[everyday] * 2;
        Debug.LogWarning("Ежедневный бонус (Награда). Зачислено: " + gold.ToString());
        _gameManager.Gold += gold;        
    }

    private bool IsOtherLayerActive
    {
        get
        {
            foreach (GameObject item in _otherLayers)
            {
                if (item.activeSelf)
                    return true;
            }

            return false;
        }
    }
}
