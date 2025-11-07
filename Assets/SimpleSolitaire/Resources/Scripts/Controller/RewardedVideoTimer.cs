using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public class AdsTimer
{
    [SerializeField] private Button _rewardedVideoButton;
    [SerializeField] private Text _timerIndicator;

    public Button RewardedVideoButton => _rewardedVideoButton;
    public Text TimerIndicator => _timerIndicator;
}

public class RewardedVideoTimer : MonoBehaviour
{
    [SerializeField] private AdsTimer[] _asdTimers;    
    private int _seconds;

    public void PressRewardedVideoButton()
    {        
        foreach (AdsTimer adsTimer in _asdTimers)
        {
            adsTimer.RewardedVideoButton.interactable = false;
        }
        StartTimer();
    }

    private void StartTimer()
    {        
        _seconds = 60;
        TimerShow();
        StartCoroutine(WaitSecond());
    }

    private IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(1);
        _seconds--;
        if (_seconds >= 0)
        {
            TimerShow();
            StartCoroutine(WaitSecond());            
        }
        else
        {
            foreach (AdsTimer adsTimer in _asdTimers)
            {
                adsTimer.RewardedVideoButton.interactable = true;
            }
            TimerHide();
        }
    }

    private void TimerShow()
    {        
        TimeSpan result = TimeSpan.FromSeconds(_seconds);
        string fromTimeString = result.ToString("m':'ss");
        foreach (AdsTimer adsTimer in _asdTimers)
        {
            adsTimer.TimerIndicator.gameObject.SetActive(true);
            adsTimer.TimerIndicator.text = fromTimeString;
        }
    }

    private void TimerHide()
    {
        foreach (AdsTimer adsTimer in _asdTimers)
        {
            adsTimer.TimerIndicator.gameObject.SetActive(false);            
        }
    }
}
