using System;
using BloomLines.Controllers;
using TMPro;
using UnityEngine;

namespace BloomLines.UI
{
    public class UITaskCompletedNotificationTab : UINotificationTabBase
    {
        [SerializeField] private TextMeshProUGUI _coins;

        public override void Show(ShowNotificationEvent eventData)
        {
            base.Show(eventData);

            _coins.text = $"{eventData.Data}";
        }
    }
}