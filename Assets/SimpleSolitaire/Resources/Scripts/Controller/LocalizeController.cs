using System;
using System.Collections.Generic;
using UnityEngine;
//using YG;

public class LocalizeController : MonoBehaviour
{
    [SerializeField] private List<string> _langs;
    public event Action<string> ChangeLanguage;

    private void Start()
    { 
        _langs = I2.Loc.LocalizationManager.GetAllLanguages();

        //YandexGame.SwitchLangEvent += OnChangeLanguage;
        //YandexGame.InitLang();
        string language = SystemLanguageIndex;
        OnChangeLanguage(language);
        
    }

    private void Update()
    {
        Update_HotKeys();
    }

    private void OnDestroy()
    {
        //YandexGame.SwitchLangEvent -= OnChangeLanguage;        
    }

    private void OnChangeLanguage(string lang)
    {
        Debug.Log("Language: " + lang);
        int id = 1;

        switch (lang)
        {
            case "ru":
                id = 0; 
                break;

            case "en":
                id = 1; 
                break;

            case "tr":
                id = 2;
                break;

            case "es":
                id = 3;
                break;

            case "de":
                id = 4;
                break;

            default:
                break;
        }

        if (I2.Loc.LocalizationManager.HasLanguage(_langs[id]))
            I2.Loc.LocalizationManager.CurrentLanguage = _langs[id];
        else
            Debug.LogError("Language " + lang + " was not founded!");
        ChangeLanguage?.Invoke(lang);
    }

    /*public void GetSystemInfo()
    {
        CrazyGames.SystemInfo systemInfo = CrazySDK.User.SystemInfo;
        //Debug.Log($"All system info: {systemInfo}");
        //Debug.Log($"Browser: {systemInfo.browser}");
        //Debug.Log($"OS: {systemInfo.os}");
        Debug.Log($"Country code: {systemInfo.countryCode}");
        //Debug.Log($"Device type: {systemInfo.device.type}");
    }

    private string CountryCode
    {
        get
        {
            CrazyGames.SystemInfo systemInfo = CrazySDK.User.SystemInfo;
            return systemInfo.countryCode;
        }        
    }*/

    private string SystemLanguageIndex
    {
        get
        {
            string language = "en";
            if (Application.systemLanguage == SystemLanguage.Turkish)
            {
                language = "tr";
            }

            if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                language = "es";
            }

            if (Application.systemLanguage == SystemLanguage.German)
            {
                language = "de";
            }

            if (Application.systemLanguage == SystemLanguage.Russian ||
                Application.systemLanguage == SystemLanguage.Ukrainian ||
                Application.systemLanguage == SystemLanguage.Belarusian ||
                Application.systemLanguage == SystemLanguage.Estonian ||
                Application.systemLanguage == SystemLanguage.Latvian ||
                Application.systemLanguage == SystemLanguage.Lithuanian)
            {
                language = "ru";
            }

            return language;
        }
    }

    private void Update_HotKeys()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.K))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                OnChangeLanguage("ru");

            if (Input.GetKeyDown(KeyCode.Alpha2))
                OnChangeLanguage("en");

            if (Input.GetKeyDown(KeyCode.Alpha3))
                OnChangeLanguage("tr");

            if (Input.GetKeyDown(KeyCode.Alpha5))
                OnChangeLanguage("es");

            if (Input.GetKeyDown(KeyCode.Alpha6))
                OnChangeLanguage("de");
        }
    }
}
