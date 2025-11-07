using BloomLines.Analytics;
using UnityEngine;

namespace BloomLines.Controllers
{
    public static class AnalyticsController
    {
        private static IAnalyticsAdapter _analyticsAdapter; // Аналитический адаптер

        // Инициализируем при загрузке сцены
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            return;
#endif

            // Загружаем нужный аналитический адаптер и инициализируем его
            _analyticsAdapter = new GameAnalyticsAdapter();
            _analyticsAdapter.Initialize();
        }

        public static void SendEvent(string id)
        {
#if UNITY_EDITOR
            return;
#endif

            _analyticsAdapter.SendEvent(id); // Отправляем ивент через текущий адаптер
        }
    }
}