using System.Collections;
using UnityEngine;

namespace BloomLines.Controllers
{
    public class PlaytimeController : MonoBehaviour
    {
        private int _minute;

        private void Start()
        {            
            StartCoroutine(Wait(5));
            DontDestroyOnLoad(gameObject);
        }  

        private IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
            TrySendPlayTime(_minute);
            _minute++;
            StartCoroutine(Wait(60));
        }

        private void TrySendPlayTime(int value)
        {
            if (value <= 20 && value % 2 != 0)
                return;

            if (value > 20 && value % 5 != 0)
                return;

            if (value > 50 && value % 10 != 0)
                return;

            if (value > 100 && value % 20 != 0)
                return;

            if (value > 150)
                return;

            AnalyticsController.SendEvent("PlayTime_" + value.ToString());            
        }
    }
}
