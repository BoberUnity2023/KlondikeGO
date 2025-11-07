using BloomLines.Controllers;
using BloomLines.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    [RequireComponent(typeof(Button))]
    public class UISettingsButton : MonoBehaviour
    {
        private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(() =>
            {
                EventsManager.Publish(new OpenPanelEvent(UIPanelType.Settings));

                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_settings_button");
            });
        }
    }
}