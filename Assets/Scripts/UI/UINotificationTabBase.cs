using System;
using BloomLines.Assets;
using BloomLines.Controllers;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public abstract class UINotificationTabBase : MonoBehaviour
    {
        [SerializeField] private NotificationType _type;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _okayButton;

        private RectTransform _rectTransform;

        public string Id { get; private set; }
        public NotificationType Type => _type;

        public void Initialize(Action onClose)
        {
            _rectTransform = GetComponent<RectTransform>();

            _okayButton.onClick.AddListener(() =>
            {
                onClose?.Invoke();

                AnalyticsController.SendEvent("close_notification");
            });
        }

        public virtual void Show(ShowNotificationEvent eventData)
        {
            var notificationData = GameAssets.GetNotificationData(eventData.Id);

            Id = eventData.Id;

            _icon.sprite = notificationData.Icon;
            _title.text = LocalizationManager.GetTranslation($"Main/notification_{notificationData.Id}");

            _rectTransform.DOAnchorPosY(-45f, 0.3f).SetEase(Ease.InOutSine);
        }

        public void Hide(Action onComplete)
        {
            DOTween.Sequence().Append(_rectTransform.DOAnchorPosY(115f, 0.3f).SetEase(Ease.InOutSine)).AppendCallback(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}