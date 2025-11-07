using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.UI;
using DG.Tweening;
using I2.Loc;
using UnityEngine;

namespace BloomLines.Controllers
{
    // Ивент для вызова отображения уведомления
    public class ShowNotificationEvent
    {
        public string Id { get; private set; }
        public string Data { get; private set; }

        public ShowNotificationEvent(string id, string data)
        {
            Id = id;
            Data = data;
        }
    }

    public class NotificationsController : MonoBehaviour
    {
        [SerializeField] private UINotificationTabBase[] _tabs; 

        private UINotificationTabBase _currentTab; // Текущее отображаемое уведомление
        private bool _haveActiveNotification;

        private CanvasGroup _canvasGroup;
        private float _timeAfterShowNotification;
        private bool _canShowNotification;
        private Queue<ShowNotificationEvent> _upcomingNotifications = new Queue<ShowNotificationEvent>(); // Очередь требуемых к отображению уведомлений

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            HideCurrentNotification();

            foreach (var tab in _tabs)
                tab.Initialize(CloseCurrentNotification);
        }

        private void Update()
        {
            if (_haveActiveNotification)
            {
                _timeAfterShowNotification += Time.deltaTime;
                if (_timeAfterShowNotification >= 5f)
                {
                    HideCurrentNotification();
                }
            }
        }

        // Закрыть текущее уведомление
        private void CloseCurrentNotification()
        {
            _canvasGroup.interactable = false;
            _currentTab.Hide(HideCurrentNotification);
        }

        // Спрятать текущее уведомление
        private void HideCurrentNotification()
        {
            _haveActiveNotification = false;

            if (_currentTab != null)
            {
                // Анимированно прячем
                _currentTab.Hide(() =>
                {
                    _currentTab = null;
                    _canShowNotification = true;
                    TryShowNotification();
                });
            }
            else
            {
                _canShowNotification = true;
            }
        }

        // Пытаемся показать уведомление
        private void TryShowNotification()
        {
            // Если можно показать уведомление и нет текущего активного
            if (_canShowNotification && !_haveActiveNotification && _upcomingNotifications.Count > 0)
            {
                AudioController.Play("notification");
                Vibration.Vibrate(30);

                var eventData = _upcomingNotifications.Dequeue(); // Берем следующее уведомление
                var notificationData = GameAssets.GetNotificationData(eventData.Id); // Берем его данные

                _canShowNotification = false;
                _canvasGroup.interactable = true;
                _haveActiveNotification = true;

                _currentTab = _tabs.FirstOrDefault(e => e.Type == notificationData.Type); // Берем его таб 
                _currentTab.Show(eventData); // Показываем его

                _timeAfterShowNotification = 0f;
            }
        }

        // Вызвали отобразить уведомление
        private void OnShowNotificationEvent(ShowNotificationEvent eventData)
        {
            var notificationData = GameAssets.GetNotificationData(eventData.Id); // Данные уведомления
            if(notificationData != null)
            {
                int upcomingCount = GetUpcomingNotificationCount(eventData.Id);
                if (_currentTab != null && _currentTab.Id == eventData.Id)
                    upcomingCount++;

                if (upcomingCount < notificationData.MaxCount) // Если не достигнут лимит данного уведомления
                {
                    _upcomingNotifications.Enqueue(eventData); // Добавляем его в очередь
                    TryShowNotification();
                }
            }
        }

        // Количество уведомлений в очереди с нужным айди
        private int GetUpcomingNotificationCount(string id)
        {
            int count = 0;
            foreach(var notification in _upcomingNotifications)
            {
                if (notification.Id == id)
                    count++;
            }

            return count;
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<ShowNotificationEvent>(OnShowNotificationEvent);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<ShowNotificationEvent>(OnShowNotificationEvent);
        }
    }
}