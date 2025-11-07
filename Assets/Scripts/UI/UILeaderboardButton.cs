using BloomLines.Controllers;
using BloomLines.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    [RequireComponent(typeof(Button))]
    public class UILeaderboardButton : MonoBehaviour
    {
        private Button _btn;

        private void Awake()
        {            
#if RuStore
            //gameObject.SetActive(false);//TODO: Нужно для RuStore
#endif
            StartCoroutine(AfterAwake());
        }

        private IEnumerator AfterAwake()
        {
            yield return new WaitForSeconds(1);
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(() =>
            {
                EventsManager.Publish(new OpenPanelEvent(UIPanelType.Leaderboard));

                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_leaderboard_button");
            });
        }
    }
}
