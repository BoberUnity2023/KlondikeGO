using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    // Базовый скрипт от которого наследуются все обьекты которые будут адаптированны под разные экраны

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(ScreenChangeNotifier))]
    public abstract class AdaptationObjectBase : MonoBehaviour
    {
        private ScreenChangeNotifier _screenChangeNotifier;
        private bool _isInitialized = false;

        // Реализуем эти методы в скриптах которые наследуют
        protected abstract void OnScreenTypeChanged(ScreenType screenType);

        protected abstract void OnScreenUpdate(ScreenInfoEvent eventData);

        protected virtual void OnEnable()
        {
            if (_isInitialized)
                return;

            _screenChangeNotifier = GetComponent<ScreenChangeNotifier>();

            _screenChangeNotifier.OnScreenUpdate += OnScreenUpdate;
            _screenChangeNotifier.OnScreenTypeChanged += OnScreenTypeChanged;

            _isInitialized = true;
        }

        protected virtual void OnDisable()
        {
            _screenChangeNotifier.OnScreenUpdate -= OnScreenUpdate;
            _screenChangeNotifier.OnScreenTypeChanged -= OnScreenTypeChanged;

            _isInitialized = false;
        }
    }
}