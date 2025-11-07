using BloomLines.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BloomLines.Managers
{
    public enum ScreenType
    {
        Vertical,
        Horizontal,
    }

    // Ивент который отсылается при обновлении экрана
    public class ScreenInfoEvent
    {
        public Vector2Int ScreenSize { get; private set; }
        public float AspectRatio { get; private set; }
        public ScreenType ScreenType { get; private set; }

        public ScreenInfoEvent(Vector2Int screenSize, float aspectRatio, ScreenType screenType)
        {
            ScreenSize = screenSize;
            AspectRatio = aspectRatio;
            ScreenType = screenType;
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ScreenManager
    {
        private static int _lastWidth;
        private static int _lastHeight;
        private static ScreenInfoEvent _lastScreenInfoEvent;

        private const float TRANSLITION_ASPECT = 1.3f / 1f; // Переход соотношений сторон с горизонтального в вертикальное
        private static Coroutine _coroutine;

#if UNITY_EDITOR
        static ScreenManager()
        {
            EditorApplication.update += Update;
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var updateHelper = UpdateHelper.Get();
            updateHelper.OnUpdate += Update;

            Application.quitting += OnDestroy;
            SceneManager.sceneLoaded += OnSceneLoaded;

#if UNITY_ANDROID
            Application.targetFrameRate = 60;
#endif
        }

        private static void OnDestroy()
        {
            Application.quitting -= OnDestroy;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // При загрузке сцены обновляем экран чтобы обьекты на новой сцене приняли нужный размер
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            var updateHelper = UpdateHelper.Get();
            updateHelper.StartCoroutine(OnSceneLoadedDelay());
        }

        private static IEnumerator OnSceneLoadedDelay()
        {
            var waitFrame = new WaitForEndOfFrame();

            yield return waitFrame;

            // Выставляем стандартное разрешение 1920х1080, т.к при резко переходе например из 21:9 на 9:21, юнити элементы ловят баг в размерах
            // Поэтому нужен плавный переход, ставим стандартное разрешение
            _lastWidth = 1920;
            _lastHeight = 1080;
            var aspectRation = (float)_lastWidth / _lastHeight;
            var screenType = aspectRation > TRANSLITION_ASPECT ? ScreenType.Vertical : ScreenType.Horizontal;
            var screenSize = new Vector2Int(_lastWidth, _lastHeight);

            if (_lastScreenInfoEvent != null && _lastScreenInfoEvent.ScreenType != screenType)
                SaveManager.Save(SaveType.GameMode);

            _lastScreenInfoEvent = new ScreenInfoEvent(screenSize, aspectRation, screenType); // Отсылаем ивент что экран обновился
            EventsManager.Publish(_lastScreenInfoEvent);

            yield return waitFrame;

            _lastWidth = 1080;
            _lastHeight = 1920;
            aspectRation = (float)_lastWidth / _lastHeight;
            screenType = aspectRation > TRANSLITION_ASPECT ? ScreenType.Vertical : ScreenType.Horizontal;
            screenSize = new Vector2Int(_lastWidth, _lastHeight);

            if (_lastScreenInfoEvent != null && _lastScreenInfoEvent.ScreenType != screenType)
                SaveManager.Save(SaveType.GameMode);

            _lastScreenInfoEvent = new ScreenInfoEvent(screenSize, aspectRation, screenType); // Отсылаем ивент что экран обновился
            EventsManager.Publish(_lastScreenInfoEvent);

            yield return waitFrame;

            _lastWidth = 0;
            _lastHeight = 0; // Чистим текущий размер экрана
            Update(); // Обновляем экран
        }

        private static void Update()
        {
            int width = Screen.width;
            int height = Screen.height;

#if UNITY_EDITOR
            string[] res = UnityStats.screenRes.Split('x');
            width = int.Parse(res[0]);
            height = int.Parse(res[1]);
#endif

            if (_lastWidth != width || _lastHeight != height)
            {
                _lastWidth = width;
                _lastHeight = height;

                var aspectRation = (float)_lastWidth / _lastHeight;
                var screenType = aspectRation > TRANSLITION_ASPECT ? ScreenType.Horizontal : ScreenType.Vertical;
                var screenSize = new Vector2Int(_lastWidth, _lastHeight);

                if (_lastScreenInfoEvent != null && _lastScreenInfoEvent.ScreenType != screenType)
                    SaveManager.Save(SaveType.GameMode);

                _lastScreenInfoEvent = new ScreenInfoEvent(screenSize, aspectRation, screenType);

                if (Application.isPlaying)
                {
                    var updateHelper = UpdateHelper.Get();

                    if (_coroutine != null)
                    {
                        updateHelper.StopCoroutine(_coroutine);
                        _coroutine = null;
                    }

                    _coroutine = updateHelper.StartCoroutine(SendEventDelay());
                }
                else
                {
                    EventsManager.Publish(_lastScreenInfoEvent);
                }
            }
        }

        private static IEnumerator SendEventDelay()
        {
            var waitFrame = new WaitForEndOfFrame();

            for (int i = 0; i < 3; i++) // Ждем немного
                yield return waitFrame;

            for (int i = 0; i < 2; i++) // Обновляем экран два раза подряд. Все эти задержки нужны и повторения потому что юнити не может идеально с первого раза всё обновить
            {
                yield return waitFrame;
                EventsManager.Publish(_lastScreenInfoEvent);
            }
        }

        public static ScreenInfoEvent GetLastScreenInfoEvent() => _lastScreenInfoEvent;
    }
}