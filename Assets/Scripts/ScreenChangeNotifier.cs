using System;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    // Cкрипт который слушает и информирует если размер экрана изменился
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ScreenChangeNotifier : MonoBehaviour
    {
        public Action<ScreenInfoEvent> OnScreenUpdate;
        public Action<ScreenType> OnScreenTypeChanged;

        private ScreenType _lastScreenType;

        private void OnScreenUpdateEvent(ScreenInfoEvent eventData)
        {
            OnScreenUpdate?.Invoke(eventData);

            if (_lastScreenType != eventData.ScreenType)
            {
                _lastScreenType = eventData.ScreenType;
                OnScreenTypeChanged?.Invoke(_lastScreenType);
            }       
        }

        private void OnEnable()
        {
            var screenInfoEvent = ScreenManager.GetLastScreenInfoEvent(); // когда обьект включается загружаем текущие данные экрана
            if (screenInfoEvent != null)
            {
                // ставим противоположный формат экрана от текуще, чтобы в методе OnScreenUpdateEvent условие прошло
                _lastScreenType = screenInfoEvent.ScreenType == ScreenType.Vertical ? ScreenType.Horizontal : ScreenType.Vertical;
                OnScreenUpdateEvent(screenInfoEvent);
            }

            EventsManager.Subscribe<ScreenInfoEvent>(OnScreenUpdateEvent);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<ScreenInfoEvent>(OnScreenUpdate);
        }
    }
}