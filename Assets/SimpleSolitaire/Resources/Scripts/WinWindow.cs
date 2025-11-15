using I2.Loc;
using SimpleSolitaire.Controller;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using YG;

public class WinWindow : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    //[SerializeField] private YandexGame _yandexGame;
    [SerializeField] private GameObject _buttonNewGame;
    [SerializeField] private Text _timeTopIndicator;
    [SerializeField] private Text _timeBonusIndicator;
    [SerializeField] private Text _bestTimeIndicator;
    [SerializeField] private Text _myRecordIndicator;
    [SerializeField] private Text _playedGamesIndicator;
    [SerializeField] private Text _goldIndicator;
    [SerializeField] private Text _x;
    [SerializeField] private Text _difficultyIndicator;
    [SerializeField] private LocalizedString _localizedEasy;
    [SerializeField] private LocalizedString _localizedMiddle;
    [SerializeField] private LocalizedString _localizedHard;

    private void OnEnable()
    {
        int timeBonus = _gameManager.TimeBonus;
        _timeBonusIndicator.text = timeBonus.ToString();

        long bestGameTime = StatisticsController.Instance.BestGameTime;        
        
        if (bestGameTime > _gameManager.TimeCount)
            bestGameTime = _gameManager.TimeCount;

        _bestTimeIndicator.text = StatisticsController.Instance.ConvertLongToTimeFormat(bestGameTime);

        int myRecordScore = /*YandexGame.savesData.BestScore;*/PlayerPrefs.GetInt("BestScore");
        _myRecordIndicator.text = myRecordScore.ToString();

        int playedGamesAmount = _gameManager.Stats.PlayedGames;//StatisticsController.Instance.PlayedGamesAmount;
        _playedGamesIndicator.text = playedGamesAmount.ToString();

        _goldIndicator.text = _gameManager.GoldForParty.ToString();

        SetDifficultyIndicator();
        //_buttonNewGame.SetActive(false);
        //StartCoroutine(AfterOnEnable());
    }

    private void SetDifficultyIndicator()
    {
        if (_gameManager.Level == 0)
        { 
            _difficultyIndicator.text = _localizedEasy;//"À≈√ »…"
            _x.text = "x1";
        }
        if (_gameManager.Level == 1)
        { 
            _difficultyIndicator.text = _localizedMiddle; //"C–≈ƒÕ»…";
            _x.text = "x2";
        }
        if (_gameManager.Level == 2)
        { 
            _difficultyIndicator.text = _localizedHard; //"Ã¿—“≈–";
            _x.text = "x3";
        }
    }

    private IEnumerator AfterOnEnable()
    {
        yield return new WaitForSeconds(2);
        //_yandexGame._FullscreenShow();
    }

    public void OnFullScreenAdClose()
    {
        _buttonNewGame.SetActive(true);
    }
}
