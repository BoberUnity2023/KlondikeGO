using GameAnalyticsSDK;
using System.Collections;
using UnityEngine;

public class ControllerAnalitycs : MonoBehaviour
{ 
    private int _minute;

    private void Start()
    {
        GameAnalytics.Initialize();
        StartCoroutine(WaitMinute(1));
    }

    private IEnumerator WaitMinute(float time)
    {
        yield return new WaitForSeconds(time);
        SendMinute(_minute);
        _minute++;
        StartCoroutine(WaitMinute(60));
    }

    private void SendMinute(int value)
    {
        if (value > 10 && value % 2 != 0)
            return;

        if (value > 20 && value % 5 != 0)
            return;

        if (value > 150)
            return;

        GameAnalytics.NewDesignEvent("Playtime:" + value.ToString());
    }
}
