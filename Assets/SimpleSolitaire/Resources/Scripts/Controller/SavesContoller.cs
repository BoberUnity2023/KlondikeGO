using SimpleSolitaire.Controller;
using System.Collections;
using UnityEngine;
//using YG;

public class SavesContoller : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    // Подписываемся на событие GetDataEvent в OnEnable
    //private void OnEnable() => YandexGame.GetDataEvent += GetData;

    // Отписываемся от события GetDataEvent в OnDisable
    //private void OnDisable() => YandexGame.GetDataEvent -= GetData;

    public void LoadGold()//Awake
    {
        //if (_gameManager.Platform == Platform.Ok ||
        //    _gameManager.Platform == Platform.VK ||            
        //    _gameManager.Platform == Platform.GD)
        //{
        //    _gameManager.Gold = PlayerPrefs.GetInt("Gold");
        //}

        //if (_gameManager.Platform == Platform.Yandex)
        //{
        //    // Проверяем запустился ли плагин
        //    //if (YandexGame.SDKEnabled == true)
        //    //{
        //    //    // Если запустился, то запускаем Ваш метод
        //    //    GetData();

        //    //    // Если плагин еще не прогрузился, то метод не запуститься в методе Start,
        //    //    // но он запустится при вызове события GetDataEvent, после прогрузки плагина
        //    //}
        //}        
    }

    // Ваш метод, который будет запускаться в старте
    public void GetData()
    {
        // Получаем данные из плагина и делаем с ними что хотим
        //_gameManager.Gold = YandexGame.savesData.Gold;
    }
}
