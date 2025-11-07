using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    [System.Serializable]
    public class RectTransformPreset
    {
        [SerializeField] private Vector2 _anchoredPosition;
        [SerializeField] private Vector2 _sizeDelta;
        [SerializeField] private Vector2 _offsetMin;
        [SerializeField] private Vector2 _offsetMax;
        [SerializeField] private Vector2 _anchorMin;
        [SerializeField] private Vector2 _anchorMax;
        [SerializeField] private Vector2 _pivot;

        public Vector2 AnchoredPosition => _anchoredPosition;
        public Vector2 SizeDelta => _sizeDelta;
        public Vector2 OffsetMin => _offsetMin;
        public Vector2 OffsetMax => _offsetMax;
        public Vector2 AnchorMin => _anchorMin;
        public Vector2 AnchorMax => _anchorMax;
        public Vector2 Pivot => _pivot;
    }

    [RequireComponent(typeof(RectTransform))]
    public class RectTransformAdaptation : AdaptationObjectBase
    {
        [SerializeField] private RectTransformPreset _verticalPreset;
        [SerializeField] private RectTransformPreset _horizontalPreset;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
            bool standartOffset = screenType == ScreenType.Horizontal ? (_horizontalPreset.OffsetMin == Vector2.zero && _horizontalPreset.OffsetMax == Vector2.zero) : (_verticalPreset.OffsetMin == Vector2.zero && _verticalPreset.OffsetMax == Vector2.zero);

            if(!standartOffset)
                _rectTransform.sizeDelta = screenType == ScreenType.Vertical ? _verticalPreset.SizeDelta : _horizontalPreset.SizeDelta;

            _rectTransform.pivot = screenType == ScreenType.Vertical ? _verticalPreset.Pivot : _horizontalPreset.Pivot;
            _rectTransform.anchorMin = screenType == ScreenType.Vertical ? _verticalPreset.AnchorMin : _horizontalPreset.AnchorMin;
            _rectTransform.anchorMax = screenType == ScreenType.Vertical ? _verticalPreset.AnchorMax : _horizontalPreset.AnchorMax;
            _rectTransform.offsetMin = screenType == ScreenType.Vertical ? _verticalPreset.OffsetMin : _horizontalPreset.OffsetMin;
            _rectTransform.offsetMax = screenType == ScreenType.Vertical ? _verticalPreset.OffsetMax : _horizontalPreset.OffsetMax;

            if (standartOffset)
                _rectTransform.sizeDelta = screenType == ScreenType.Vertical ? _verticalPreset.SizeDelta : _horizontalPreset.SizeDelta;

            _rectTransform.anchoredPosition = screenType == ScreenType.Vertical ? _verticalPreset.AnchoredPosition : _horizontalPreset.AnchoredPosition;
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
        }
    }
}